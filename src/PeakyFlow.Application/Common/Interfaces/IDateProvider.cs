namespace PeakyFlow.Application.Common.Interfaces
{
    public interface IDateProvider
    {
        DateTimeOffset Now { get; }
    }
}
