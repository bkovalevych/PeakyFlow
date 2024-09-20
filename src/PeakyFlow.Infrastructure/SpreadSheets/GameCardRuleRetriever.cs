using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PeakyFlow.Abstractions;
using PeakyFlow.Abstractions.Common.Interfaces;
using PeakyFlow.Abstractions.GameCardRuleAggregate;

namespace PeakyFlow.Infrastructure.SpreadSheets
{
    public class GameCardRuleRetriever(
        IStringConverter stringConverter,
        IOptions<SheetsSettings> delegateSheets,
        ILogger<GameCardRuleRetriever> logger) : ISheetsRetriever<GameCardRule>
    {
        public List<string> Ranges => [
            delegateSheets.Value.BigDealsRange,
            delegateSheets.Value.MoneyToTheWindRange,
            delegateSheets.Value.SmallDealsRange,
            delegateSheets.Value.StocksRange,
            delegateSheets.Value.MarketRange];

        private readonly IReadOnlyDictionary<string, Func<string, Card, Card>> BigDelaRetrievers = new Dictionary<string, Func<string, Card, Card>>()
        {
            ["Id"] = (val, card) => card with { Id = $"bigDeal-{val}" },
            ["Name"] = (val, card) => card with { Name = val },
            ["Header"] = (val, card) => card with { Header = val },
            ["Description"] = (val, card) => card with { Description = val },
            ["Footer"] = (val, card) => card with { Footer = val },
            ["Cost"] = (val, card) => card with { Cost = stringConverter.ToValue(val, 0) },
            ["Liability"] = (val, card) => card with { Mortgage = stringConverter.ToValue(val, 0) },
            ["Down pay"] = (val, card) => card with { DownPay = stringConverter.ToValue(val, 0) },
            ["Cash flow"] = (val, card) => card with { CashFlow = stringConverter.ToValue(val, 0) }
        };

        private readonly IReadOnlyDictionary<string, Func<string, Card, Card>> MoneyToTheWindRetrievers = new Dictionary<string, Func<string, Card, Card>>()
        {
            ["Id"] = (val, card) => card with { Id = $"mtw-{val}" },
            ["Name"] = (val, card) => card with { Name = val },
            ["Header"] = (val, card) => card with { Header = val },
            ["Description"] = (val, card) => card with { Description = val },
            ["Footer"] = (val, card) => card with { Footer = val },
            ["Mortgage"] = (val, card) => card with { Mortgage = stringConverter.ToValue(val, 0) },
            ["DownPay"] = (val, card) => card with { DownPay = stringConverter.ToValue(val, 0) },
            ["CashFlow"] = (val, card) => card with { CashFlow = stringConverter.ToValue(val, 0) }
        };


        private readonly IReadOnlyDictionary<string, Func<string, Card, Card>> SmallDealRetrievers = new Dictionary<string, Func<string, Card, Card>>()
        {
            ["Id"] = (val, card) => card with { Id = $"smallDeal-{val}" },
            ["Name"] = (val, card) => card with { Name = val },
            ["Header"] = (val, card) => card with { Header = val },
            ["Description"] = (val, card) => card with { Description = val },
            ["Footer"] = (val, card) => card with { Footer = val },
            ["Cost"] = (val, card) => card with { Cost = stringConverter.ToValue(val, 0) },
            ["Liability / Mortgage"] = (val, card) => card with { Mortgage = stringConverter.ToValue(val, 0) },
            ["Down pay"] = (val, card) => card with { DownPay = stringConverter.ToValue(val, 0) },
            ["Cash flow"] = (val, card) => card with { CashFlow = stringConverter.ToValue(val, 0) }
        };

        private readonly IReadOnlyDictionary<string, Func<string, Card, Card>> StockRetrievers = new Dictionary<string, Func<string, Card, Card>>()
        {
            ["Id"] = (val, card) => card with { Id = $"smallDeal-{val}" },
            ["Symbol"] = (val, card) => card with { Name = val },
            ["Header"] = (val, card) => card with { Header = val },
            ["Description"] = (val, card) => card with { Description = val },
            ["Footer"] = (val, card) => card with { Footer = val },
            ["Price"] = (val, card) => card with { Cost = stringConverter.ToValue(val, 0), DownPay = stringConverter.ToValue(val, 0) },
            ["Range"] = (val, card) => card with { TradingRange = val },
            ["Divident"] = (val, card) => card with { CashFlow = stringConverter.ToValue(val, 0) },
            ["StockAction"] = (val, card) => card with { StockAction = stringConverter.ToValue(val, StockAction.Default) }
        };
        
        private readonly IReadOnlyDictionary<string, Func<string, Card, Card>> MarketRetrievers = new Dictionary<string, Func<string, Card, Card>>()
        {
            ["Id"] = (val, card) => card with { Id = $"market-{val}" },
            ["Group"] = (val, card) => card with { Group = val },
            ["Header"] = (val, card) => card with { Header = val },
            ["Description"] = (val, card) => card with { Description = val },
            ["Footer"] = (val, card) => card with { Footer = val },
            ["Cost"] = (val, card) => card with { Cost = stringConverter.ToValue(val, 0) },
            ["Liability"] = (val, card) => card with { Mortgage = stringConverter.ToValue(val, 0) },
            ["Down pay"] = (val, card) => card with { DownPay = stringConverter.ToValue(val, 0) },
            ["Cash flow"] = (val, card) => card with { CashFlow = stringConverter.ToValue(val, 0) }
        };

        public List<GameCardRule> Retrieve(IList<IList<IList<object>>> ranges)
        {
            var rule = new GameCardRule() 
            {
                Id = "1"
            };

            rule.Cards = [
            .. ProcessObjects(ranges[0], BigDelaRetrievers,
                card => card with {CardType = CardType.BigDeal }),
            .. ProcessObjects(ranges[1], MoneyToTheWindRetrievers, 
                card => card with { CardType = CardType.MoneyToTheWind, Required = true }),
            .. ProcessObjects(ranges[2], SmallDealRetrievers, 
                card => card with { CardType = CardType.SmallDeal }),
            .. ProcessObjects(ranges[3], StockRetrievers,
                card => card with { CardType = CardType.SmallDeal, IsStock = true }),
            .. ProcessObjects(ranges[4], MarketRetrievers,
                card => card with { CardType = CardType.Market })];

            return [rule];
        }

        

        private List<Card> ProcessObjects(IList<IList<object>> objects, IReadOnlyDictionary<string, Func<string, Card, Card>> retriever, Func<Card, Card>? postProcess = null) 
        {
            if (objects.Count <= 1)
            {
                throw new ArgumentException("");
            }

            var headers = objects[0].Select((x, i) => (x.ToString(), i))
                .ToList();
            
            var cards = new List<Card>();
            try 
            {
                for (var row = 1; row < objects.Count; row++)
                {
                    Card card = Card.GetDefault();

                    foreach (var (name, col) in headers)
                    {
                        if (name == null || !retriever.ContainsKey(name) || objects[row].Count <= col)
                        {
                            continue;
                        }
                        var val = objects[row][col].ToString();

                        if (val == null)
                        {
                            continue;
                        }
             
                        card = retriever[name].Invoke(val, card);
                        card = postProcess?.Invoke(card) ?? card;
                    }

                    cards.Add(card);
                }
            }
            catch (Exception ex) 
            {
                logger.LogWarning(ex, "something went wrong during retrieving cards");
                throw;
            }

            return cards;

        }
    }
}
