using PeakyFlow.GrpcProtocol.Game;
using PeakyFlow.GrpcProtocol.Lobby;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace PeakyFlow.Console.Services
{
    internal class MainService
    {
        private readonly LobbyRpcService.LobbyRpcServiceClient _lobbyRpcService;
        private readonly GameRpcService.GameRpcServiceClient _gameRpcService;
        
        private readonly Layout _layout;
        private Dictionary<int, (string, string)> _stepCords = new Dictionary<int, (string, string)>();

        public MainService(
            GameRpcService.GameRpcServiceClient gameRpcService,
            LobbyRpcService.LobbyRpcServiceClient lobbyRpcService)
        {
            _gameRpcService = gameRpcService;
            _lobbyRpcService = lobbyRpcService;
            _layout = new Layout("Root");
        }

        public void StartMap()
        {
            var lobbies = _lobbyRpcService.ListLobbies(new ListLobbyMessage());
            InitMap();

            AnsiConsole.Live(_layout)
                .Cropping(VerticalOverflowCropping.Top)
                .StartAsync(async ctx =>
                {
                    UpdatePlayers();
                    ctx.Refresh();
                    var key = System.Console.ReadKey();
                    var random = new Random();

                    while (key.KeyChar != 'c' && (key.Modifiers & ConsoleModifiers.Control) == 0)
                    {
                        

                        key = System.Console.ReadKey();
                    }
                })
                .GetAwaiter()
                .GetResult();
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
