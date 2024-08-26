using PeakyFlow.Abstractions;
using PeakyFlow.Abstractions.GameCardRuleAggregate;

namespace PeakyFlow.Infrastructure.SpreadSheets
{
    public class GameCardRuleRetriever : ISheetsRetriever<GameCardRule>
    {
        public string Range => "'BigDeals'!A1:J41";

        private static readonly IReadOnlyDictionary<string, Action<string, Card[]>> Retrievers = new Dictionary<string, Action<string, Card[]>>()
        {
            ["Id"] = (val, card) => card[0] = card[0] with {  Id = val },
            ["Type"] = (val, card) => card[0] = card[0] with { CardType = Enum.Parse<CardType>(val.Replace(" ", ""), true) },
            ["Name"] = (val, card) => card[0] = card[0] with { Name = val },
            ["Header"] = (val, card) => card[0] = card[0] with { Header = val },
            ["Description"] = (val, card) => card[0] = card[0] with { Description = val },
            ["Footer"] = (val, card) => card[0] = card[0] with { Footer = val },
            ["Cost"] = (val, card) => 
            {
                if (int.TryParse(val, out var intVal))
                {
                    card[0] = card[0] with { Cost = intVal };
                }
            },
            ["Liability"] = (val, card) =>
            {
                if (int.TryParse(val, out var intVal))
                {
                    card[0] = card[0] with { Mortgage = intVal };
                }
            },
            ["Down pay"] = (val, card) =>
            {
                if (int.TryParse(val, out var intVal))
                {
                    card[0] = card[0] with { DownPay = intVal };
                }
            },
            ["Cash flow"] = (val, card) =>
            {
                if (int.TryParse(val, out var intVal))
                {
                    card[0] = card[0] with { CashFlow = intVal };
                }
            },
        };

        public List<GameCardRule> Retrieve(IList<IList<object>> objects)
        {
            var rule = new GameCardRule() 
            {
                Id = "b"
            };

            var cards = new List<Card>();

            if (objects.Count <= 1)
            {
                throw new ArgumentException("");
            }

            var headers = objects[0].Select((x, i) => (x.ToString(), i))
                .ToList();

            for (var row = 1; row < objects.Count; row++) 
            {
                Card[] card = [new Card("", "", "", CardType.Default, false, "", "", "", "", "", 0, 0, 0, 0, StockAction.Default, false, false, false)];

                foreach (var (name, col) in headers)
                {
                    if (name == null)
                    {
                        continue;
                    }
                    
                    var val = objects[row][col].ToString();
                    
                    if (val == null) 
                    {
                        continue;
                    }

                    Retrievers[name].Invoke(val, card);
                }

                cards.Add(card[0]);

            }

            rule.Cards = [.. cards];

            return [rule];
        }
    }
}
