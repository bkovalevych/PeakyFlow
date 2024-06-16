using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using PeakyFlow.Abstractions.LobbyAggregate;
using PeakyFlow.Application.Common.Extensions;
using PeakyFlow.Application.Common.Interfaces;
using PeakyFlow.Application.LobbyGame.Create;

namespace PeakyFlow.Application.UnitTests.Common.Bahaviors
{
    public class ValidationBehaviorTests
    {
        public ValidationBehaviorTests()
        {

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

            var provider = services
                .AddApplication()
                .AddTransient(typeof(IRepository<Lobby>), x => fakeRepository.Object)
                .AddSingleton(x => fakeLogger.Object)
                .AddTransient<IGuid, GuidImpl>()

                .BuildServiceProvider();

            var m = provider.GetRequiredService<IMediator>();


            // Act

            var result = await m.Send(new CreateLobbyCommand("Bo", "Na", 0, null));
        }
    }
}
