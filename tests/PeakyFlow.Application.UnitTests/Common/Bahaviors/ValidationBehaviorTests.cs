using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using PeakyFlow.Abstractions.LobbyAggregate;
using PeakyFlow.Application.Common.Extensions;
using PeakyFlow.Application.Common.Interfaces;
using PeakyFlow.Application.LobbyGame.Create;
using System.Reflection;

namespace PeakyFlow.Application.UnitTests.Common.Bahaviors
{
    public class ValidationBehaviorTests
    {
        private readonly Mock<IDateProvider> _mockDateProvider;

        public ValidationBehaviorTests()
        {
            _mockDateProvider = new Mock<IDateProvider>();
        }

        class GuidImpl : IGuid
        {
            public string NewId()
            {
                return Guid.NewGuid().ToString();
            }
        }

        [Fact]
        public async Task DoValidation()
        {
            //assert
            var services = new ServiceCollection();

            var fakeRepository = new Mock<IRepository<Lobby>>();
            var fakeLogger = new Mock<ILogger<CreateLobbyHandler>>();
            _mockDateProvider.SetupGet(x => x.Now).Returns(new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

            var provider = services
                .AddApplication()
                .AddAutoMapper(Assembly.GetAssembly(typeof(DependencyInjection)))
                .AddTransient(typeof(IRepository<Lobby>), x => fakeRepository.Object)
                .AddSingleton(x => fakeLogger.Object)
                .AddTransient<IGuid, GuidImpl>()
                .AddSingleton(_ => _mockDateProvider.Object)
                .BuildServiceProvider();

            var m = provider.GetRequiredService<IMediator>();


            // Act

            var result = await m.Send(new CreateLobbyCommand("Bo", "Na", 0, null));
        }
    }
}
