namespace PeakyFlow.Abstractions.GameCardRuleAggregate
{
    public class GameCardRule : Entity, IAggregateRoot
    {
        public Card[] Cards { get; set; } = [];

        public Dictionary<CardType, List<string>> ShuffleForGame()
        {
            var random = new Random();
            
            random.Shuffle(Cards);
            
            return Cards.GroupBy(x => x.CardType).ToDictionary(x => x.Key, x => x.Select(x => x.Id).ToList());
        }
    }
}
