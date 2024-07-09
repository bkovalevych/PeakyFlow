namespace PeakyFlow.Application.Common.Interfaces
{
    public interface ICacheQuery
    {
        string CacheKey { get; }

        TimeSpan MaxAge { get; }
    }
}
