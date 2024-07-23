using PeakyFlow.Abstractions;
using PeakyFlow.Abstractions.GameMapAggregate;
using PeakyFlow.Abstractions.GameMapRuleAggregate;
using PeakyFlow.Application.GameMapRules.GetMapRulesForRoom;
using Spectre.Console;
using Spectre.Console.Rendering;
using static System.Net.Mime.MediaTypeNames;

namespace PeakyFlow.Console.Services
{
    internal class MainService
    {
        private readonly IGetMapRulesForRoomService _mapGetter;
        private Layout _layout;
        private readonly GameMap _gameMap;
        private Dictionary<int, (string, string)> _stepCords = new Dictionary<int, (string, string)>();

        public MainService(IGetMapRulesForRoomService mapGetter)
        {
            _mapGetter = mapGetter;
            _layout = new Layout("Root");
            _gameMap = new GameMap()
            {
                Id = "1",
                GameMapPlayers = [ new GameMapPlayer() { Id = "1", Name = "Bohdan" }],
                Steps = _mapGetter.Get(default).Result!.Steps
            };
        }

        

        public void StartMap()
        {
            InitMap();

            AnsiConsole.Live(_layout)
                .Cropping(VerticalOverflowCropping.Top)
                .StartAsync(async ctx => 
                {
                    UpdatePlayers();
                    ctx.Refresh();
                    var key = System.Console.ReadKey();

                    while (key.KeyChar != 'c' && (key.Modifiers & ConsoleModifiers.Control) == 0)
                    {
                        await _gameMap.TakeTurn(_ => 
                        {
                            ClearPlayers();
                            
                            if ('1' <= key.KeyChar && '9' >= key.KeyChar)
                            {
                                return Task.FromResult(key.KeyChar - '0');
                            }

                            return Task.FromResult(1);
                        }, 
                        _ => Task.CompletedTask);

                        UpdatePlayers();
                        ctx.Refresh();
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
            foreach (var g in _gameMap.GameMapPlayers.GroupBy(x => x.Position))
            {
                var cord = _stepCords[g.Key];
                UpdateStep(cord);
            }
        }

        private void UpdatePlayers()
        {

            foreach (var g in _gameMap.GameMapPlayers.GroupBy(x => x.Position))
            {
                var cord = _stepCords[g.Key];

                UpdateStep(cord, new Rows(g.Select(x => new Markup($"{x.Name}"))));
            }
        }

        private void InitMap() 
        {
            var columns = 4;
            var rows = (_gameMap!.Steps.Length - 2 * columns) / 2 + 2;

            _layout.SplitColumns(Enumerable.Range(0, columns)
                .Select(x => new Layout($"col-{x}")
                    .SplitRows(Enumerable.Range(0, rows)
                        .Select(y => new Layout($"{x}-{y}", new Markup("")))
                        .ToArray()))
                .ToArray());

            var vec = (x: 1, y: 0);
            var p = (x: 0, y: 0);
            var i = 0;
            foreach (var step in _gameMap!.Steps)
            {
                var index = $"{p.x}-{p.y}";
                var wrapper = step switch
                {
                    StepType.Salary => $"[yellow]{step}[/]",
                    StepType.Market => $"[blue]{step}[/]",
                    StepType.Children => $"[grey]{step}[/]",
                    StepType.MoneyToTheWind => $"[red]{step}[/]",
                    StepType.Deal => $"[green]{step}[/]",
                    StepType.Downsize => $"[gray]{step}[/]",
                    StepType.Charity => $"{step}",
                    _ => $"{step}",
                };
                var text = $"{wrapper} {i + 1}";
                var cord = (index, text);
                _stepCords[i] = cord;

                UpdateStep(cord);
                ++i;

                var next = (x: p.x + vec.x, y: p.y + vec.y);

                if (next.x < 0 || next.y < 0 || next.x >= columns || next.y >= rows)
                {
                    vec = RotateVec(vec);
                    p = (x: p.x + vec.x, y: p.y + vec.y);

                    continue;
                }

                p = next;
            }
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
