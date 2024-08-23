using PeakyFlow.Application.Common.Interfaces;

namespace PeakyFlow.Infrastructure.Services
{
    public class DateProvider : IDateProvider
    {
        public DateTimeOffset Now => DateTimeOffset.Now;
    }
}
