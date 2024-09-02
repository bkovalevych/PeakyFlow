using PeakyFlow.Abstractions.GameRoleAggregate;

namespace PeakyFlow.Abstractions.RoomStateAggregate
{
    public class RoomState : Entity, IAggregateRoot
    {
        public Dictionary<CardType, int> Indeces { get; set; } = [];

        public Dictionary<CardType, List<string>> Cards { get; set; } = [];

        public IEnumerable<PlayerState> PlayerStates { get; set; } = [];


        public string GetCardIdByType(CardType cardType)
        {
            if (!Indeces.TryGetValue(cardType, out int value))
            {
                value = 0;
                Indeces[cardType] = value;
            }

            var count = Cards[cardType].Count;

            var index = value;
            var item = Cards[cardType][index];

            Indeces[cardType] = (index + 1) % count;

            return item;
        }

        public PlayerState? Borrow(string playerId, int money, string id)
        {
            return WithExistingPlayer(playerId, player =>
            {
                player.Savings += money;
                player.PercentableLiabilities.Add(new PercentableLiabilityItem(id,
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

                foreach (var (id, moneyForName) in liabilityNames.Zip(money))
                {
                    var addPercentable = player.PercentableLiabilities.Where(x => x.LiabilityAmount != 0 && id == x.Id)
                        .Select(x => (x, moneyForName));

                    percentable.AddRange(addPercentable);

                    var addCountable = player.CountableLiabilities.Where(x => x.LiabilityAmount != 0 && id == x.Id)
                        .Select(x => (x, moneyForName));

                    countable.AddRange(addCountable);


                    var addFinancable = player.FinancialItems.Where(x => x.LiabilityAmount != 0 && x.LiabilityAmount == moneyForName && id == x.Id)
                        .Select(x => (x, moneyForName));

                    financable.AddRange(addFinancable);

                }

                var liabilitiesSum = percentable.Sum(x => x.Item2) + countable.Sum(x => x.Item2) + financable.Sum(x => x.Item2);

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
                        player.CountableLiabilities.Add(new CountableLiabilityItem(item.Id, item.Name, 
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
                            item.Id,
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

        public (bool Acceptable, PlayerState? PlayerState) AcceptCard(Card card, string playerId, int? count, IEnumerable<string>? financeIds)
        {
            var acceptable = true;

            var resultPlayer = WithExistingPlayer(playerId, player =>
            {
                var check = IsCardAcceptable(card, playerId, count);
                acceptable = check.Acceptable;
                
                if (!acceptable) 
                {
                    return;
                }

                if (card.IsStock)
                {
                    if (count == null)
                    {
                        throw new ArgumentNullException(nameof(count));
                    }

                    var stock = player.Stocks.FirstOrDefault(x => x.Name == card.Name && x.PriceForOne == card.DownPay);

                    if (stock == null)
                    {
                        stock = new StockItem(
                            card.Id,
                            card.Name,
                            FinancialType.Stock,
                            count.Value,
                            card.DownPay,
                            card.CashFlow,
                            card.Group);
                    } 
                    else
                    {
                        player.Stocks.Remove(stock);
                        stock = new StockItem(
                            stock.Id,
                            stock.Name,
                            stock.FinancialType,
                            stock.Count + count.Value,
                            stock.PriceForOne,
                            stock.FlowForOne,
                            stock.Group);
                    }

                    player.Stocks.Add(stock);
                }
                else if (card.CardType == CardType.BigDeal || card.CardType == CardType.SmallDeal 
                    || card.CardType == CardType.MoneyToTheWind && card.CashFlow != 0)
                {
                    var financialType = card.IsBusiness ? FinancialType.Business
                        : card.IsRealEstate
                            ? FinancialType.RealEstate
                            : FinancialType.Others;

                    var deal = new FinancialItem(card.Id, card.Name, financialType, card.Cost, card.Mortgage, 
                        card.CashFlow, card.Group);
                    
                    player.FinancialItems.Add(deal);
                }
                else if (card.CardType == CardType.Market)
                {
                    if (card.StockAction != StockAction.Default)
                    {
                        var multiplier = card.StockAction == StockAction.ReverseSplit1For2 ? 2.0f : 0.5f;

                        var changedStocks = player.Stocks.Where(x => x.Name == card.Name);

                        foreach (var stock in changedStocks) 
                        {
                            player.Stocks.Remove(stock);

                            var newStock = new StockItem(stock.Id, stock.Name, FinancialType.Stock,
                                (int)Math.Max(1, stock.PriceForOne * multiplier),
                                stock.PriceForOne, stock.FlowForOne, stock.Group);

                            player.Stocks.Add(newStock);
                        }
                    }
                    else if (financeIds != null)
                    {
                        var stocks = player.Stocks.Where(x => financeIds.Contains(x.Id)).ToList();
                        
                        foreach (var stock in stocks) 
                        {
                            player.Stocks.Remove(stock);
                            player.Savings += stock.Count * card.Cost;
                        }

                        var financialItems = player.FinancialItems.Where(x => financeIds.Contains(x.Id)).ToList();

                        foreach (var item in financialItems)
                        {
                            player.FinancialItems.Remove(item);
                            player.Savings += card.Cost - item.LiabilityAmount;

                        }
                    }
                }

                player.Savings -= card.DownPay;

            });

            return (acceptable, resultPlayer);
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

        public void CountSalary(string playerId)
        {
            WithExistingPlayer(playerId, player =>
            {
                player.Savings += player.CashFlow;
            });
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
    }
}
