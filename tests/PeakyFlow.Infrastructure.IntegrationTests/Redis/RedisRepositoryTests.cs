using Ardalis.Specification;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.Extensions.Logging;
using Moq;
using PeakyFlow.Abstractions.RoomAggregate;
using PeakyFlow.Infrastructure.Redis;
using PeakyFlow.Infrastructure.Redis.Models;
using PeakyFlow.Infrastructure.Redis.RedisMappings;
using PeakyFlow.Infrastructure.Services;
using Redis.OM;
using StackExchange.Redis;
using System.Net;

namespace PeakyFlow.Infrastructure.IntegrationTests.Redis
{
    public class RedisRepositoryTests
    {
        private readonly Mock<ILogger<RedisRepository<Room, RoomM>>> _fakeLogger;
        private readonly IMapper _mapper;

        public RedisRepositoryTests()
        {
            _fakeLogger = new Mock<ILogger<RedisRepository<Room, RoomM>>>();
            _mapper = new Mapper(new MapperConfiguration(x =>
            {
                x.AddExpressionMapping();
                x.AddProfile<RoomProfile>();
            }));
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

            var rep = new RedisRepository<Room, RoomM>(redisConnection, _mapper, _fakeLogger.Object);

            var result = await rep.AddAsync(new Room()
            {
                Id = "b5bcf98a-db17-44da-b2d9-ad15e07cdb35",
                Name = "Room1",
                Players =
                [
                    new PlayerInRoom()
                    {
                        Id = "46402f13-98ba-4596-b5ba-cdebaa83fbc0",
                        Name = "BohdanChyk",
                        Status = Abstractions.RoomAggregate.PlayerInRoomStatus.Active
                    },
                    new PlayerInRoom()
                    {
                        Id = "46402f13-98ba-4596-b5ba-cdebaa83fbc1",
                        Name = "Dan",
                        Status = Abstractions.RoomAggregate.PlayerInRoomStatus.Lost
                    }
                ]
            });
            await rep.DeleteAsync(result);
            await hosted.StopAsync(default);

            // assert

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GivenRoom_WhenSearch_ThenResult()
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

            var rep = new RedisRepository<Room, RoomM>(redisConnection, _mapper, _fakeLogger.Object);

            var result = await rep.AddAsync(new Room()
            {
                Id = "b5bcf98a-db17-44da-b2d9-ad15e07cdb35",
                Name = "Room1",
                Players = [
                    new PlayerInRoom()
                    {
                        Id = "46402f13-98ba-4596-b5ba-cdebaa83fbc0",
                        Name = "BohdanChyk",
                        Status = Abstractions.RoomAggregate.PlayerInRoomStatus.Active
                    },
                    new PlayerInRoom()
                    {
                        Id = "46402f13-98ba-4596-b5ba-cdebaa83fbc1",
                        Name = "Dan",
                        Status = Abstractions.RoomAggregate.PlayerInRoomStatus.Lost
                    }
                ]
            });


            var rooms = await rep.ListAsync(new RoomMFindById("46402f13-98ba-4596-b5ba-cdebaa83fbc1"), default);

            await rep.DeleteAsync(result);
            await hosted.StopAsync(default);

            // assert

            Assert.NotNull(result);
            Assert.Single(rooms);
        }

        [Fact]
        public async Task GivenRoom_WhenUpdate_ThenResult()
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

            var rep = new RedisRepository<Room, RoomM>(redisConnection, _mapper, _fakeLogger.Object);

            var result = await rep.AddAsync(new Room()
            {
                Id = "b5bcf98a-db17-44da-b2d9-ad15e07cdb35",
                Name = "Room1",
                Players = [
                    new PlayerInRoom()
                    {
                        Id = "46402f13-98ba-4596-b5ba-cdebaa83fbc0",
                        Name = "BohdanChyk",
                        Status = Abstractions.RoomAggregate.PlayerInRoomStatus.Active
                    },
                    new PlayerInRoom()
                    {
                        Id = "46402f13-98ba-4596-b5ba-cdebaa83fbc1",
                        Name = "Dan",
                        Status = Abstractions.RoomAggregate.PlayerInRoomStatus.Lost
                    }
                ]
            });


            var rooms = await rep.ListAsync(new RoomMFindById("46402f13-98ba-4596-b5ba-cdebaa83fbc1"), default);

            rooms[0].Name = "Changed Name";
            rooms[0].Players.First().Status = Abstractions.RoomAggregate.PlayerInRoomStatus.Unknown;
            rooms[0].Players = rooms[0].Players.Append(new PlayerInRoom()
            {
                Id = "46402f13-98ba-4596-b5ba-cdebaa83fbc2",
                Name = "Liuda",
                Status = Abstractions.RoomAggregate.PlayerInRoomStatus.Out
            });

            await rep.UpdateAsync(rooms[0]);
            await rep.SaveChangesAsync();

            await rep.DeleteAsync(result);
            await hosted.StopAsync(default);

            // assert

            Assert.NotNull(result);
        }

        private class RoomMFindById : Specification<Room>
        {
            public RoomMFindById(string id)
            {
                Query.Where(x => x.Players.Any(x => x.Id == id));
            }
        }
    }
}
