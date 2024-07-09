using PeakyFlow.Application.LobbyGame.Create;

namespace PeakyFlow.Application.UnitTests.LobbyGame.Create
{
    public class CreateLobbyValidatorTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(7)]
        public void ValidateWithTeamSize(int teamSize)
        {
            //Assert

            var req = new CreateLobbyCommand("Bohdan", "Lobby1", teamSize, null);

            var sub = new CreateLobbyValidator();


            //Act

            var result = sub.Validate(req);

            //Assert

            Assert.NotNull(result);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.ErrorCode == "InclusiveBetweenValidator");
        }
    }
}
