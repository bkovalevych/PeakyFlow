using PeakyFlow.Abstractions;
using PeakyFlow.Abstractions.GameMapRuleAggregate;

namespace PeakyFlow.Infrastructure.SpreadSheets
{
    internal class GameMapRuleRetriever : ISheetsRetriever<GameMapRule>
    {
        public string Range => "'GameMap'!A1:B25";
        private const string ColumnName = "StepType";

        public List<GameMapRule> Retrieve(IList<IList<object>> objects)
        {
            var gameMaps = new List<GameMapRule>()
            {
                new()
                {
                    Id = "1"
                }
            };

            var indexForStepType = objects[0]
                .Select((raw, x) => (index: x, name: raw.ToString()))
                .Where(x => x.name == ColumnName)
                .Select(x => x.index)
                .FirstOrDefault();

            var steps = new List<StepType>();

            foreach (var item in objects[indexForStepType].Skip(1))
            {
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
