namespace PeakyFlow.Abstractions.GameCardRuleAggregate
{
    public class GameCardRule : Entity, IAggregateRoot
    {
        public Card[] Cards { get; set; } = [];

        public ILookup<CardType, string> ShuffleForGame()
        {
            var random = new Random();
            
            random.Shuffle(Cards);
            
            return Cards.ToLookup(x => x.CardType, x => x.Id);
        }
    }
}
