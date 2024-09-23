using PeakyFlow.Console.Enums;
using PeakyFlow.GrpcProtocol.Game;
using PeakyFlow.GrpcProtocol.Lobby;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace PeakyFlow.Console.Services
{
    internal class MainService
    {
        private readonly LobbyRpcService.LobbyRpcServiceClient _lobbyRpcService;
        private readonly MenuStateProvider _menu;
        private readonly GameRpcService.GameRpcServiceClient _gameRpcService;
        
        private readonly Layout _layout;
        private Dictionary<int, (string, string)> _stepCords = new Dictionary<int, (string, string)>();
        private int _currentIndex;
        private List<MenuAction> _actionsList;
        private string? _playerId;
        private string? _playerName;
        
        private LobbyItem _currentLobby;
        private List<PlayerJoinedToLobbyMessage> _playersInLobby = new List<PlayerJoinedToLobbyMessage>();
        private Dictionary<string, bool> _playerIsRedy = new Dictionary<string, bool>();

        public MainService(
            GameRpcService.GameRpcServiceClient gameRpcService,
            LobbyRpcService.LobbyRpcServiceClient lobbyRpcService,
            MenuStateProvider menu)
        {
            _gameRpcService = gameRpcService;
            _lobbyRpcService = lobbyRpcService;
            _menu = menu;
            _menu.OnStartMenu += OnStartMenu;
            _menu.OnCreatingLobbyMenu += OnCreatingLobbyMenu;
            _menu.OnEnterListLobbies += OnEnterListLobbies;
            _menu.OnEnterLobbyMenuForPlayer += OnEnterLobbyMenuForPlayer;
            _menu.OnRefreshLobbies += OnRefreshLobbies;
            _layout = new Layout("Root");
        }

        private void OnRefreshLobbies()
        {
            AnsiConsole.Clear();
            
            OnEnterListLobbies();
        }

        private void OnEnterLobbyMenuForPlayer(JoinLobbyMessage obj)
        {
            AnsiConsole.Clear();
            var ready = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title($"User: {obj.PlayerName}. Join lobby?")
                .AddChoices("Yes", "No, go to list lobbies"));
            
            if (ready == "Yes") 
            {
                var resp = _lobbyRpcService.JoinLobby(obj);

                if (resp.Successfully)
                {
                    _playerId =  resp.PlayerId;
                    _playerName = obj.PlayerName;
                    
                    LobbyPlayerMenu();
                }
                else
                {
                    _menu.Fire(MenuAction.GoToListLobbies);
                }
            }
            else
            {
                _menu.Fire(MenuAction.GoToListLobbies);
            }
        }

        private void LobbyPlayerMenu()
        {
            var table = new Table()
                .Expand()
                .Alignment(Justify.Center)
                .AddColumns("Game:", _currentLobby?.Name ?? string.Empty)
                .AddRow("Owner:", _currentLobby?.Owner ?? string.Empty)
                .AddRow("Team size:", _currentLobby?.TeamSize.ToString() ?? string.Empty)
                .AddRow(_currentLobby?.Owner ?? string.Empty, "ready");

            AnsiConsole.Live(table)
                .Cropping(VerticalOverflowCropping.Top)
                .Start(ctx =>
                {
                    var msg = "You have joined the game! Please wait others";
                    for (var i = 1; i <= msg.Length; ++i)
                    {
                        table.Title(msg[..i], new Style(Color.DarkGreen));
                        Task.Delay(100).GetAwaiter().GetResult(); 
                        ctx.Refresh();
                    }

                    var serverCall =_lobbyRpcService.OnLobbyEvent(new LobbyEventMessage()
                    {
                        LobbyId = _currentLobby?.Id,
                    });
                    _lobbyRpcService.PlayerIsReady(new PlayerIsReadyMessage()
                    {
                        IsReady = true,
                        LobbyId = _currentLobby?.Id,
                        PlayerId = _playerId
                    });

                    _playersInLobby.Add(new PlayerJoinedToLobbyMessage()
                    {
                        LobbyId = _currentLobby?.Id,
                        PlayerId= _playerId,
                        PlayerName = _playerName
                    });

                    _playerIsRedy[_playerId ?? string.Empty] = true;

                    table.AddRow(_playerName ?? string.Empty, "ready");
                    ctx.Refresh();

                    while (serverCall.ResponseStream.MoveNext(default).GetAwaiter().GetResult())
                    {
                        if (serverCall.ResponseStream.Current != null)
                        {
                            var lobbyEvent = serverCall.ResponseStream.Current;
                            
                            for (int i = table.Rows.Count - 1; i >= 0; i--)
                            {
                                table.RemoveRow(i);
                            }

                            ctx.Refresh();
                            table
                                .AddRow("Owner:", _currentLobby?.Owner ?? string.Empty)
                                .AddRow("Team size:", _currentLobby?.TeamSize.ToString() ?? string.Empty)
                                .AddRow(_currentLobby?.Owner ?? string.Empty, "ready");

                            if (!string.IsNullOrEmpty(lobbyEvent.LobbyClosedId))
                            {
                                _menu.Fire(MenuAction.GoToListLobbies);
                                break;
                            }

                            if (lobbyEvent.ReasonCase == LobbyEvent.ReasonOneofCase.PlayerJoined)
                            {
                                _playersInLobby.Add(lobbyEvent.PlayerJoined);
                                _playerIsRedy[lobbyEvent.PlayerJoined.PlayerId] = false;
                            }

                            if (lobbyEvent.ReasonCase == LobbyEvent.ReasonOneofCase.PlayerLeftId)
                            {
                                _playerIsRedy.Remove(lobbyEvent.PlayerLeftId);
                                _playersInLobby.RemoveAll(x => x.PlayerId == lobbyEvent.PlayerLeftId);
                            }

                            if (lobbyEvent.ReasonCase == LobbyEvent.ReasonOneofCase.PlayerIsReadyId &&
                                _playersInLobby.FirstOrDefault(x => x.PlayerId == lobbyEvent.PlayerIsReadyId) is PlayerJoinedToLobbyMessage p)
                            {
                                _playerIsRedy[p.PlayerId] = true;
                            }

                            if (lobbyEvent.ReasonCase == LobbyEvent.ReasonOneofCase.PlayerIsNotReadyId &&
                                _playersInLobby.FirstOrDefault(x => x.PlayerId == lobbyEvent.PlayerIsNotReadyId) is PlayerJoinedToLobbyMessage pIsNotReady)
                            {
                                _playerIsRedy[pIsNotReady.PlayerId] = false;
                            }

                            
                            
                            for (int i = 0; i < _playersInLobby.Count; i++)
                            {
                                var player = _playersInLobby[i];
                                var isReady = _playerIsRedy[player.PlayerId];

                                table.AddRow(player.PlayerName, isReady ? "ready" : "not ready");
                            }
                            table.Expand();
                            ctx.Refresh();
                        }
                    }
                });
        }

        

        private void OnEnterListLobbies()
        {
            LobbyListResp? lobbyResult = null;

            _actionsList = _menu.AvailableActions().ToList();
            AnsiConsole.Status()
                .Spinner(Spinner.Known.Arc)
                .Start("Loading...", ctx =>
                {
                    lobbyResult = _lobbyRpcService.ListLobbies(new ListLobbyMessage());
                    ctx.Status("Done");
                });

            AnsiConsole.Clear();

            if (lobbyResult == null)
            {
                AnsiConsole.WriteLine("Error loading lobbies");
            }

            LobbyItem? nullLobby = null;
            MenuAction? nullAction = null;
            
            var (lobby, action) = AnsiConsole.Prompt(new SelectionPrompt<(LobbyItem? lobby, MenuAction? action)>()
                .Title("Lobbies")
                .UseConverter(x => x.lobby == null 
                    ? $"{x.action}"
                    : $"{x.lobby.Name} - {(x.lobby.IsPublic ? "public" : "private")} {x.lobby.PlayersNumber}/{x.lobby.TeamSize}")
                .AddChoices(lobbyResult?.Lobbies.Select(x => (lobby: (LobbyItem?)x, action: nullAction)) ?? [])
                .AddChoices(_actionsList
                    .Where(x => x != MenuAction.JoinLobby)
                    .Select(x => (lobby: nullLobby, action: (MenuAction?)x))));
            
            if (action != null) 
            {
                _menu.Fire(action.Value);
            }
            
            if (lobby != null)
            {
                var name = AnsiConsole.Prompt(new TextPrompt<string>("Your name:"));
                _currentLobby = lobby;
                _menu.Fire(_menu.JoinLobbyTrigger, new JoinLobbyMessage()
                {
                    LobbyId = lobby.Id,
                    PlayerName = name
                });
            }
        }

        private void OnCreatingLobbyMenu()
        {
            AnsiConsole.Clear();

            var name = AnsiConsole.Ask<string>("lobby: ");
            var yourName = AnsiConsole.Ask<string>("your name: ");
            var teamSize = AnsiConsole.Prompt(new SelectionPrompt<int>()
                .Title("Team size")
                .AddChoices(1, 2, 3, 4, 5, 6));
            
            var createLobbyRequest = new CreateLobbyMessage()
            {
                Name = name,
                Owner = yourName,
                TeamSize = teamSize
            };

            var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title($"Are you sure you want to create lobby {name} with {teamSize} people?")
                .AddChoices("Yes", "No, go to list lobbies"));
            
            if (choice == "Yes")
            {
                _menu.Fire(_menu.CreateLobbyTrigger, createLobbyRequest);
            }
            else
            {
                _menu.Fire(MenuAction.GoToListLobbies);
            }
        }

        private void OnStartMenu()
        {
            AnsiConsole.Clear();
            _actionsList = _menu.AvailableActions().ToList();
            var prompt = AnsiConsole.Prompt(new SelectionPrompt<MenuAction>()
                .Title("Peaky flow")
                .AddChoices(_actionsList));
            _menu.Fire(prompt);
        }

        private void RefreshMenu()
        {
            _actionsList = _menu.AvailableActions().ToList();
            _layout
                .SplitRows(_actionsList
                .Select((x, i) => 
                    new Layout(
                        i.ToString(), 
                        new Markup(x.ToString(), new Style(i == 0 ? Color.Blue: Color.White))))
                .ToArray());
            _currentIndex = 0;
        }

        public async Task StartMap()
        {
            _menu.Start();
            //var lobbies = await _lobbyRpcService.ListLobbiesAsync(new ListLobbyMessage());
            //InitMap();

            //await AnsiConsole.Live(_layout)
            //    .Cropping(VerticalOverflowCropping.Top)
            //    .StartAsync(async ctx =>
            //    {
            //        _menu.Start();
            //        ctx.Refresh();

            //        //var key = System.Console.ReadKey();
            //        var random = new Random();

            //        //while (key.KeyChar != 'c' && (key.Modifiers & ConsoleModifiers.Control) == 0)
            //        //{


            //            //key = System.Console.ReadKey();

            //            //if (key.Key == ConsoleKey.UpArrow 
            //            //    && _actionsList != null 
            //            //    && _actionsList.Count > 0)
            //            //{
            //            //    _layout[_currentIndex.ToString()]
            //            //        .Update(new Markup(_actionsList[_currentIndex].ToString(), new Style(Color.White)));

            //            //    _currentIndex = (_actionsList.Count + _currentIndex - 1) % _actionsList.Count;

            //            //    _layout[_currentIndex.ToString()]
            //            //        .Update(new Markup(_actionsList[_currentIndex].ToString(), new Style(Color.Blue)));
            //            //    ctx.Refresh();
            //            //}
            //            //else if (key.Key == ConsoleKey.DownArrow
            //            //    && _actionsList != null
            //            //    && _actionsList.Count > 0)
            //            //{
            //            //    _layout[_currentIndex.ToString()]
            //            //        .Update(new Markup(_actionsList[_currentIndex].ToString(), new Style(Color.White)));

            //            //    _currentIndex = (_currentIndex + 1) % _actionsList.Count;

            //            //    _layout[_currentIndex.ToString()]
            //            //        .Update(new Markup(_actionsList[_currentIndex].ToString(), new Style(Color.Blue)));

            //            //    ctx.Refresh();
            //            //}
            //            //else if (key.Key == ConsoleKey.Enter && _actionsList != null
            //            //    && _actionsList.Count > 0)
            //            //{
            //            //    _menu.Fire(_actionsList[_currentIndex]);
            //            //    RefreshMenu();
            //            //}

            //        //}
            //    });
        }

        private (int x, int y) RotateVec((int x, int y) p)
        {
            var deg = Math.PI / 2;
            var cs = Math.Cos(deg);
            var sn = Math.Sin(deg);
            var x = p.x * cs - p.y * sn;
            var y = p.x * sn + p.y * cs;

            return ((int)x, (int)y);
        }

        private void ClearPlayers()
        {
            
        }

        private void UpdatePlayers()
        {
        }

        private void InitMap()
        {
            var columns = 5;
            var rows = (20 - 2 * columns) / 2 + 2 + 1; // +1 for start

            _layout.SplitColumns(Enumerable.Range(0, columns)
                .Select(x => new Layout($"col-{x}")
                    .SplitRows(Enumerable.Range(0, rows)
                        .Select(y => new Layout($"{x}-{y}", new Markup("")))
                        .ToArray()))
                .ToArray());

            var vec = (x: 1, y: 0);
            var p = (x: 0, y: 0);
            var i = 0;

            //foreach (var step in _gameMap!.Steps)
            //{
            //    var index = $"{p.x}-{p.y}";
            //    var wrapper = step switch
            //    {
            //        StepType.Salary => $"[yellow]{step} {i}[/]",
            //        StepType.Market => $"[blue]{step} {i}[/]",
            //        StepType.Children => $"[grey]{step} {i}[/]",
            //        StepType.MoneyToTheWind => $"[red]{step} {i}[/]",
            //        StepType.Deal => $"[green]{step} {i}[/]",
            //        StepType.Downsize => $"[gray]{step} {i}[/]",
            //        StepType.Charity => $"[#23ee11]{step} {i}[/]",
            //        _ => $"{step}",
            //    };

            //    var text = $"{wrapper}";
            //    var cord = (index, text);
            //    _stepCords[i] = cord;

            //    UpdateStep(cord);

            //    ++i;
            //    var next = (x: p.x + vec.x, y: p.y + vec.y);

            //    if (next.x < 0 || next.y < 0 || next.x >= columns || next.y >= rows)
            //    {
            //        vec = RotateVec(vec);
            //        p = (x: p.x + vec.x, y: p.y + vec.y);

            //        continue;
            //    }

            //    p = next;

            //    if (i == 1)
            //    {
            //        p = (x: 0, y: 1);
            //    }
            //}
        }

        private void UpdateStep((string index, string text) cord, IRenderable? renderable = null)
        {
            _layout[cord.index].Update(
                new Panel(renderable ?? new Markup(""))
                {
                    Header = new PanelHeader(cord.text),
                    Expand = true
                });
        }
    }
}
