namespace PeakyFlow.Application.Common.Interfaces
{
    public interface ICacheService
    {
        void SetFor<TData>(string key, TData data, TimeSpan expiration);

        bool TryGetValue<TData>(string key, out TData result);
    }
}
