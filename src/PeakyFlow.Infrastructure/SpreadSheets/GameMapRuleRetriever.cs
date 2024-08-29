using PeakyFlow.Abstractions;
using PeakyFlow.Abstractions.GameMapRuleAggregate;

namespace PeakyFlow.Infrastructure.SpreadSheets
{
    internal class GameMapRuleRetriever : ISheetsRetriever<GameMapRule>
    {
        public List<string> Ranges => ["'GameMap'!A1:B26"];
        private const string ColumnName = "StepType";

        public List<GameMapRule> Retrieve(IList<IList<IList<object>>> ranges)
        {
            var gameMaps = new List<GameMapRule>()
            {
                new()
                {
                    Id = "1"
                }
            };

            var objects = ranges[0];

            var indexForStepType = objects[0]
                .Select((raw, x) => (index: x, name: raw.ToString()))
                .Where(x => x.name == ColumnName)
                .Select(x => x.index)
                .FirstOrDefault();

            var steps = new List<StepType>();

            foreach (var row in objects.Skip(1))
            {
                var item = row[indexForStepType];
                if (item?.ToString() is string val)
                {
                    var stepType = Enum.Parse<StepType>(val, true);
                    steps.Add(stepType);
                }
            }

            gameMaps[0].Steps = [.. steps];

            return gameMaps;
        }
    }
}
