using Microsoft.Extensions.Logging;
using Moq;
using PeakyFlow.Infrastructure.Redis;
using PeakyFlow.Infrastructure.Redis.Models;
using PeakyFlow.Infrastructure.Services;
using Redis.OM;
using StackExchange.Redis;
using System.Net;

namespace PeakyFlow.Infrastructure.IntegrationTests.Redis
{
    public class RedisRepositoryTests
    {
        private readonly Mock<ILogger<RedisRepository<RoomM>>> _fakeLogger;

        public RedisRepositoryTests()
        {
            _fakeLogger = new Mock<ILogger<RedisRepository<RoomM>>>();
            
        }


        [Fact]
        public async Task GivenRoom_WhenAdd_ThenAdded()
        {
            //Assign
            var redisConnectionForIndeces = new RedisConnectionProvider(new ConfigurationOptions() 
            {
                DefaultDatabase = 0,
                EndPoints = { "localhost:6379" },
                Ssl = false,
                ConnectTimeout = 5000,
                AbortOnConnectFail = false
            });

            var hosted = new RedisHostedService(redisConnectionForIndeces);
            await hosted.StartAsync(default);

            var redisConnection = new RedisConnectionProvider(new ConfigurationOptions()
            {
                DefaultDatabase = 1,
                EndPoints = { "localhost:6379" },
                Ssl = false,
                ConnectTimeout = 5000,
                AbortOnConnectFail = false
            });

            var rep = new RedisRepository<RoomM>(redisConnection, _fakeLogger.Object);

            var result = await rep.AddAsync(new RoomM()
            {
                Id = "b5bcf98a-db17-44da-b2d9-ad15e07cdb35",
                Name = "Room1",
                Players = new List<PlayerInRoomM>() 
                {
                    new PlayerInRoomM()
                    {
                        Id = "46402f13-98ba-4596-b5ba-cdebaa83fbc0",
                        Name = "BohdanChyk",
                        Status = Abstractions.RoomAggregate.PlayerInRoomStatus.Active
                    },
                    new PlayerInRoomM()
                    {
                        Id = "46402f13-98ba-4596-b5ba-cdebaa83fbc1",
                        Name = "Dan",
                        Status = Abstractions.RoomAggregate.PlayerInRoomStatus.Lost
                    }
                }
            });

            await hosted.StopAsync(default);

            // assert

            Assert.NotNull(result);
        }
    }
}
