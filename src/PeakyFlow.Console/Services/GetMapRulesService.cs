using PeakyFlow.Abstractions;
using PeakyFlow.Abstractions.GameMapRuleAggregate;
using PeakyFlow.Application.GameMapRules.GetMapRulesForRoom;

namespace PeakyFlow.Console.Services
{
    internal class GetMapRulesService : IGetMapRulesForRoomService
    {
        public Task<GameMapRule?> Get(CancellationToken ct)
        {
            return Task.FromResult<GameMapRule?>(new GameMapRule()
            {
                Id = "1",
                Steps = [
                    StepType.Start,
                    StepType.Market,
                    StepType.Deal,
                    StepType.Salary,
                    StepType.Charity,
                    StepType.Deal,
                    StepType.Salary,
                    StepType.MoneyToTheWind,
                    StepType.Deal,
                    StepType.Children,
                    StepType.Salary,
                    StepType.Deal,
                    StepType.Downsize,
                    StepType.Salary
                ]
            });
        }
    }
}
