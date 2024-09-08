using Ardalis.Result;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using PeakyFlow.Abstractions.LobbyAggregate;
using PeakyFlow.Application.Common.Interfaces;
using PeakyFlow.Application.LobbyGame.JoinLobby;

namespace PeakyFlow.Application.UnitTests.LobbyGame.JoinLobby
{
    public class JoinLobbyHandlerTests
    {
        private readonly Mock<IRepository<Lobby>> _mockRepository;
        private readonly Mock<ILogger<JoinLobbyHandler>> _mockLogger;
        private readonly Mock<IGuid> _mockGuid;
        private readonly Mock<IMediator> _mockMediator;

        public JoinLobbyHandlerTests()
        {
            _mockRepository = new Mock<IRepository<Lobby>>();
            _mockLogger = new Mock<ILogger<JoinLobbyHandler>>();
            _mockGuid = new Mock<IGuid>();
            _mockMediator = new Mock<IMediator>();
        }


        [Fact]
        public async Task GivenValidJoinLobbyCommand_WhenHandle_ThenJoinLobbyResponse()
        {
            // Assign
            
            var lobby = new Lobby()
            {
                Id = "id1",
                Name = "Lobby",
            };

            _mockRepository.Setup(x => x.GetByIdAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(lobby);

            _mockRepository.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var command = new JoinLobbyCommand("id1", "Dan", null);

            var handler = new JoinLobbyHandler(_mockLogger.Object, _mockRepository.Object, _mockGuid.Object, _mockMediator.Object);



            // Act

            var result = await handler.Handle(command, default);
            
            // Assert

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(1, lobby.PlayersNumber);
        }

        [Fact]
        public async Task GivenInvalidJoinLobbyCommand_WhenHandle_ThenNotFoundResult()
        {
            // Assign

            var command = new JoinLobbyCommand("id1", "Dan", null);

            var handler = new JoinLobbyHandler(_mockLogger.Object, _mockRepository.Object, _mockGuid.Object, _mockMediator.Object);

            // Act

            var result = await handler.Handle(command, default);

            // Assert

            Assert.NotNull(result);
            Assert.True(result.IsNotFound());
        }
    }
}
