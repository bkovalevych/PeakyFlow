using Microsoft.Extensions.Hosting;
using PeakyFlow.Infrastructure.Redis.Models;
using Redis.OM;
using Redis.OM.Modeling;
using System.Reflection;

namespace PeakyFlow.Infrastructure.Services
{
    public class RedisHostedService : IHostedService
    {
        private readonly RedisConnectionProvider _provider;

        public RedisHostedService(RedisConnectionProvider provider)
        {
            _provider = provider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var entities = this.GetType().Assembly.GetTypes()
                .Where(x => x.GetCustomAttributes<DocumentAttribute>().Any());

            foreach (var entity in entities) 
            {
                var existingIndex = await _provider.Connection.GetIndexInfoAsync(entity.Name);
                if (existingIndex == null) 
                {
                    await _provider.Connection.CreateIndexAsync(entity);
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
