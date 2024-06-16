using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using PeakyFlow.Abstractions.LobbyAggregate;
using PeakyFlow.Abstractions.LobbyAggregate.Events;
using PeakyFlow.Application.Common.Interfaces;
using PeakyFlow.Application.LobbyGame.Create;

namespace PeakyFlow.Application.UnitTests.LobbyGame.Create
{
    public class CreateLobbyHandlerTests
    {
        private readonly Mock<IMediator> _fakeMediator;
        private readonly Mock<IRepository<Lobby>> _fakeLobbyRepository;
        private readonly Mock<ILogger<CreateLobbyHandler>> _fakeLogger;
        private readonly Mock<IGuid> _fakeGuid;

        public CreateLobbyHandlerTests()
        {
            _fakeMediator = new Mock<IMediator>();
            _fakeLobbyRepository = new Mock<IRepository<Lobby>>();
            _fakeLogger = new Mock<ILogger<CreateLobbyHandler>>();
            _fakeGuid = new Mock<IGuid>();
        }


        [Fact]
        public async Task Handle_WithValidData()
        {
            //Assert

            _fakeGuid.Setup(x => x.NewId()).Returns("1");
            var command = new CreateLobbyCommand("Bohdan", "Lobby1", 1, null);
            var lobby = new Lobby(new LobbyInfo("1", "Bohdan", "Lobby1", null));
            lobby.SetTeamSize(1);

            var handler = new CreateLobbyHandler(_fakeMediator.Object, _fakeLobbyRepository.Object, _fakeGuid.Object, _fakeLogger.Object);
            _fakeLobbyRepository.Setup(x => x.AddAsync(It.IsAny<Lobby>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(lobby);

            //Act

            var result = await handler.Handle(command, default);

            // Assert

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal("1", result.Value);
            _fakeMediator.Verify(x => x.Publish(It.IsAny<LobbyCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
