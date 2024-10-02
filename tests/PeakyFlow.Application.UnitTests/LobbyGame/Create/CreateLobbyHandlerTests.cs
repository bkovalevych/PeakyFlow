using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using PeakyFlow.Abstractions.LobbyAggregate;
using PeakyFlow.Abstractions.LobbyAggregate.Events;
using PeakyFlow.Application.Common.Interfaces;
using PeakyFlow.Application.Common.Mappings;
using PeakyFlow.Application.LobbyGame.Create;

namespace PeakyFlow.Application.UnitTests.LobbyGame.Create
{
    public class CreateLobbyHandlerTests
    {
        private readonly Mock<IMediator> _fakeMediator;
        private readonly Mock<IRepository<Lobby>> _fakeLobbyRepository;
        private readonly Mock<ILogger<CreateLobbyHandler>> _fakeLogger;
        private readonly Mock<IGuid> _fakeGuid;
        private readonly Mock<IDateProvider> _fakeDateTime;

        public CreateLobbyHandlerTests()
        {
            _fakeMediator = new Mock<IMediator>();
            _fakeLobbyRepository = new Mock<IRepository<Lobby>>();
            _fakeLogger = new Mock<ILogger<CreateLobbyHandler>>();
            _fakeGuid = new Mock<IGuid>();
            _fakeDateTime = new Mock<IDateProvider>();
        }


        [Fact]
        public async Task Handle_WithValidData()
        {
            //Assert

            _fakeGuid.Setup(x => x.NewId()).Returns("1");
            var created = new DateTime(2024, 1, 1);
            _fakeDateTime.SetupGet(x => x.Now).Returns(created);

            var command = new CreateLobbyCommand("Bohdan", "Lobby1", 1, null);
            var lobby = new Lobby()
            {
                Id = "1",
                OwnerId = "Bohdan",
                Name = "Lobby1",
                Created = created
            };
            lobby.SetTeamSize(1);

            var mapper = new Mapper(new MapperConfiguration(x =>
            {
                x.AddProfile<LobbyMapperProfies>();
            }));

            var handler = new CreateLobbyHandler(_fakeMediator.Object, _fakeLobbyRepository.Object, _fakeGuid.Object, _fakeDateTime.Object, mapper, _fakeLogger.Object);
            _fakeLobbyRepository.Setup(x => x.AddAsync(It.IsAny<Lobby>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(lobby);

            //Act

            var result = await handler.Handle(command, default);

            // Assert

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal("1", result.Value.Id);
            _fakeMediator.Verify(x => x.Publish(It.IsAny<LobbyCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
