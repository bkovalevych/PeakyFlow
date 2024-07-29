using PeakyFlow.Abstractions.GameRoleAggregate;

namespace PeakyFlow.Abstractions.RoomStateAggregate
{
    public class RoomState : Entity, IAggregateRoot
    {
        public Dictionary<CardType, int> Indeces { get; set; } = [];

        public ILookup<CardType, string> Cards { get; set; } = Enumerable.Empty<Card>().ToLookup(x => x.CardType, x => x.Id);

        public string GetCardIdByType(CardType cardType)
        {
            if (!Indeces.ContainsKey(cardType))
            {
                Indeces[cardType] = 0;
            }

            var count = Cards[cardType].Count();

            var index = Indeces[cardType];
            var item = Cards[cardType].ElementAt(index);

            Indeces[cardType] = (index + 1) % count;

            return item;
        }

        public PlayerState? Borrow(string playerId, int money)
        {
            return WithExistingPlayer(playerId, player =>
            {
                player.PercentableLiabilities.Add(new PercentableLiabilityItem(
                    PercentableLiabilityItem.Loan,
                    FinancialType.Loan,
                    money,
                    PercentableLiabilityItem.LoanPercent));
            });
        }

        public PlayerState? Repair(string playerId, IEnumerable<string> liabilityNames, IEnumerable<int> money)
        {
            return WithExistingPlayer(playerId, player =>
            {
                var percentable = new List<(PercentableLiabilityItem, int)>();
                var countable = new List<(CountableLiabilityItem, int)>();
                var financable = new List<(FinancialItem, int)>();

                foreach (var (name, moneyForName) in liabilityNames.Zip(money))
                {
                    var addPercentable = player.PercentableLiabilities.Where(x => x.LiabilityAmount != 0 && name == x.Name)
                        .Select(x => (x, moneyForName));

                    percentable.AddRange(addPercentable);

                    var addCountable = player.CountableLiabilities.Where(x => x.LiabilityAmount != 0 && name == x.Name)
                        .Select(x => (x, moneyForName));

                    countable.AddRange(addCountable);


                    var addFinancable = player.FinancialItems.Where(x => x.LiabilityAmount == moneyForName && name == x.Name)
                        .Select(x => (x, moneyForName));

                    financable.AddRange(addFinancable);

                }

                var liabilitiesSum = percentable.Sum(x => x.Item2) + countable.Sum(x => x.Item2);

                if (player.Savings < liabilitiesSum)
                {
                    return;
                }

                player.Savings -= liabilitiesSum;

                foreach (var (item, moneyForName) in countable)
                {
                    player.CountableLiabilities.Remove(item);

                    if (item.LiabilityAmount != moneyForName)
                    {
                        var newCount = item.Count - moneyForName / item.LiabilityForOne;
                        player.CountableLiabilities.Add(new CountableLiabilityItem(item.Name, 
                            item.FinancialType,
                            newCount,
                            item.PriceForOne,
                            item.LiabilityForOne));
                    }
                }

                foreach (var (item, moneyForName) in percentable)
                {
                    player.PercentableLiabilities.Remove(item);

                    if (item.LiabilityAmount != moneyForName)
                    {
                        var newLiability = item.LiabilityAmount - moneyForName;

                        player.PercentableLiabilities.Add(new PercentableLiabilityItem(
                            item.Name, 
                            item.FinancialType, 
                            newLiability,
                            item.Percent));
                    }
                }

                foreach (var (item, moneyForName) in financable)
                {
                    player.FinancialItems.Remove(item);
                }
            });
        }

        public (bool Acceptable, PlayerState? PlayerState) AcceptCard(Card card, string playerId)
        {
            // todo add functionality
            return (true, null);
        }

        public (bool Successfuly, bool Acceptable, int HowMuchToBorrow) IsCardAcceptable(Card card, string playerId, int? Count)
        {
            var acceptable = true;
            var howMuchToBorrow = 0;

            var successfuly = WithExistingPlayer(playerId, player =>
            {
                acceptable = string.IsNullOrEmpty(card.Condition) 
                || player.FinancialItems.Any(x => x.Name == card.Condition || x.Group == card.Condition) 
                || player.Stocks.Any(x => x.Name == card.Condition || x.Group == card.Condition);
                
                if (!acceptable)
                {
                    return;
                }

                if (card.IsStock)
                {
                    if (Count == null)
                    {
                        throw new ArgumentNullException(nameof(Count));
                    }

                    var downPay = card.DownPay * Count.Value;
                    
                    acceptable = player.Savings >= downPay;
                    howMuchToBorrow = Math.Max(0, downPay - player.Savings);
                }
                else if (card.CardType != CardType.Market || card.DownPay > 0) 
                {
                    var downPay = card.DownPay;
                    acceptable = player.Savings >= downPay;
                    howMuchToBorrow = Math.Max(0, downPay - player.Savings);
                }
            }) != null;

            return (successfuly, acceptable, howMuchToBorrow);
        }

        private PlayerState? WithExistingPlayer(string playerId, Action<PlayerState> action)
        {
            var player = PlayerStates.FirstOrDefault(x => x.Id == playerId);

            if (player == null)
            {
                return null;
            }

            action.Invoke(player);

            return player;
        }

        public IEnumerable<PlayerState> PlayerStates { get; set; } = [];
    }
}
