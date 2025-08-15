using Ardalis.Result;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using PeakyFlow.Abstractions.LobbyAggregate;
using PeakyFlow.Application.Common.Extensions;
using PeakyFlow.Application.Common.Interfaces;
using PeakyFlow.Application.LobbyGame.Create;
using PeakyFlow.Application.LobbyGame.List;
using System.Reflection;

namespace PeakyFlow.Application.UnitTests.Common.Bahaviors
{
    public class CacheBahaviorTests
    {
        [Fact]
        public async Task MediatorSendWithCache()
        {
            var services = new ServiceCollection();

            var fakeRepository = new Mock<IRepository<Lobby>>();
            var fakeLogger = new Mock<ILogger<CreateLobbyHandler>>();
            var fakeCacheSevice = new Mock<ICacheService>();
            var fakeDateProvider = new Mock<IDateProvider>();
            fakeDateProvider.SetupGet(x => x.Now).Returns(new DateTimeOffset());

            var expectedValue = Result<IEnumerable<LobbyListResponse>>.Success(new List<LobbyListResponse>()
            {
                new LobbyListResponse()
                {
                    Created = new DateTimeOffset(),
                    Id = "id1",
                    IsFree = true,
                    Owner = "bohdan",
                    Name = "lobby1"
                },
                new LobbyListResponse()
                {
                    Created = new DateTimeOffset(),
                    Id = "id2",
                    IsFree = false,
                    Owner = "bohdan",
                    Name = "lobby2"
                }
            });

            fakeCacheSevice.Setup(x =>
                x.TryGetValue(It.IsAny<string>(), out expectedValue))
                .Returns(true);

            var provider = services
                .AddApplication()
                .AddAutoMapper(Assembly.GetAssembly(typeof(DependencyInjection)))
                .AddTransient(typeof(IRepository<Lobby>), x => fakeRepository.Object)
                .AddTransient(x => fakeDateProvider.Object)
                .AddTransient(x => fakeCacheSevice.Object)
                .AddSingleton(x => fakeLogger.Object)
                .BuildServiceProvider();

            var m = provider.GetRequiredService<IMediator>();


            // Act

            var result = await m.Send(new LobbyListQuery()
            {
                PaginationCount = 20,
                PaginationSkip = 0
            });
        }

        [Fact]
        public async Task MediatorSendWithSaveCache()
        {
            var services = new ServiceCollection();

            var fakeRepository = new Mock<IRepository<Lobby>>();
            var fakeLogger = new Mock<ILogger<CreateLobbyHandler>>();
            var fakeCacheSevice = new Mock<ICacheService>();
            var fakeDateProvider = new Mock<IDateProvider>();
            fakeDateProvider.SetupGet(x => x.Now).Returns(new DateTimeOffset());

            Result<IEnumerable<LobbyListResponse>>? cacheValue = null;

            var expectedValue = new List<Lobby>()
            {
                new Lobby()
                {
                    Id = "id1",
                    Created = new DateTimeOffset(),
                    Name = "lobby1",
                    OwnerId = "bohdan"
                },
                new Lobby()
                {
                    Id = "id2",
                    Created = new DateTimeOffset(),
                    Name = "lobby2",
                    OwnerId = "bohdan"
                }
            };

            fakeRepository.Setup(x => x.ListAsync(It.IsAny<LobbyListSpecification>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedValue);

            fakeCacheSevice.Setup(x =>
                x.TryGetValue(It.IsAny<string>(), out cacheValue))
                .Returns(false);

            var provider = services
                .AddApplication()
                .AddAutoMapper(Assembly.GetAssembly(typeof(DependencyInjection)))
                .AddTransient(typeof(IRepository<Lobby>), x => fakeRepository.Object)
                .AddTransient(x => fakeDateProvider.Object)
                .AddTransient(x => fakeCacheSevice.Object)
                .AddSingleton(x => fakeLogger.Object)
                .BuildServiceProvider();

            var m = provider.GetRequiredService<IMediator>();


            // Act

            var result = await m.Send(new LobbyListQuery()
            {
                PaginationCount = 20,
                PaginationSkip = 0
            });
        }
    }
}
