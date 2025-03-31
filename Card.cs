using System.Globalization;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Blackjack
{
    static class CardInfo
    {
        static readonly Dictionary<CardId,int> values = new Dictionary<CardId, int>
        {
            {CardId._2,2},
            {CardId._3,3},
            {CardId._4,4},
            {CardId._5,5},
            {CardId._6,6},
            {CardId._7,7},
            {CardId._8,8},
            {CardId._9,9},
            {CardId._10,10},
            {CardId._J,10},
            {CardId._Q,10},
            {CardId._K,10},
            {CardId._A,11},
        };
        static readonly Dictionary<CardId,string> names = new Dictionary<CardId, string>
        {
            {CardId._2,"2"},
            {CardId._3,"3"},
            {CardId._4,"4"},
            {CardId._5,"5"},
            {CardId._6,"6"},
            {CardId._7,"7"},
            {CardId._8,"8"},
            {CardId._9,"9"},
            {CardId._10,"10"},
            {CardId._J,"J"},
            {CardId._Q,"Q"},
            {CardId._K,"K"},
            {CardId._A,"A"},
        };
        public static int GetValue(CardId id)
        {
            return values[id];
        }
        public static string GetName(CardId id)
        {
            return names[id];
        }
    }
    enum CardId
    {
        _2,
        _3,
        _4,
        _5,
        _6,
        _7,
        _8,
        _9,
        _10,
        _J,
        _Q,
        _K,
        _A
    }
    class Card(CardId val)
    {
        public readonly CardId Id = val;
        public readonly int Value = CardInfo.GetValue(val);
        [JsonInclude]
        public readonly string Name = CardInfo.GetName(val);
    }

    class Deck(Dealer? d)
    {
        protected List<Card> cards = [];

        private readonly Dealer? dealer = d;

        public Card? Hit()
        {
            var card = dealer?.GetCard();
            if(card != null)
            {
                cards.Add(card);
            }
            return card;
        }
        public string PrintDeck()
        {
            return JsonSerializer.Serialize(cards);
        }
        public int Points()
        {   
            var normally = cards.Sum(val => val.Value);
            var low = normally - 10 * cards.FindAll(c => c.Id == CardId._A).Count;
            if(normally <= 21)
            {
                return normally;
            }
            else
            {
                return low;
            }
        }
        public bool HasBlackjack()
        {
            return cards.Any(c => c.Id == CardId._A) && cards.Any(c => (c.Id == CardId._K || c.Id == CardId._Q || c.Id == CardId._J || c.Id == CardId._10) ) && cards.Count == 2;
        }
    }

    class FullDeck : Deck
    {
        readonly Random random = new();
        public FullDeck() : base(null)
        {
            foreach (CardId i in Enum.GetValues(typeof(CardId)))
            {
                foreach (int j in Enumerable.Range(1, 4))
                {
                    cards.Add(new Card(i));
                }
            }
        }
        public Card? GetCard()
        {
            if (cards.Count > 0)
            {
                var choosen = random.GetItems(cards.ToArray(), 1);
                cards.Remove(choosen.First());
                try
                {
                    return choosen.First();
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }
        public bool HasCards()
        {
            return cards.Count > 0;
        }
    }
    class Dealer
    {
        readonly FullDeck fullDeck = new FullDeck();
        readonly Random random = new();
        public Card? GetCard()
        {
            return fullDeck.GetCard();
        }
        public bool HasCards()
        {
            return fullDeck.HasCards();
        }
    }
    class Player(Dealer dealer, int start_tokens)
    {
        protected readonly Deck deck = new(dealer);

        public bool HasFolded = false;
        int tokens = start_tokens;

        int current_bet = 0;

        bool _IsPlaying = true;

        public bool IsPlayling() { return _IsPlaying; }

        public void Hit()
        {
            if(_IsPlaying && current_bet > 0)
            {
                deck.Hit();
            }
        }
        public void Bet(int bet)
        {
            if(bet > tokens)
            {
                current_bet = tokens;
                tokens = 0;
            }
            else
            {
                tokens -= bet;
                current_bet += bet;
            }
        }
        public int Tokens()
        {
            return tokens;
        }
        public void Fold()
        {
            _IsPlaying = false;
            HasFolded = true;
        }
        public void Stand()
        {
            _IsPlaying = false;
        }
        public string PrintDeck()
        {
            return deck.PrintDeck();
        }
        public int Score()
        {
            return deck.Points();
        }
        public void Won()
        {
            if(deck.HasBlackjack())
            {
                tokens += (int)(current_bet * 2.5);
            }
            else
            {
                tokens += current_bet * 2;
            }
        }
        public void Tie()
        {
            tokens += current_bet;
        }
        public void MakeBet()
        {
            Console.WriteLine("How much do you want to bet?");
            Console.WriteLine("You have {0} tokens",tokens);
            try
            {
                int bet = Convert.ToInt32(Console.ReadLine());
                Bet(bet);
            }
            catch
            {
                Console.WriteLine("Input value wasn't a number, please try again.");
                MakeBet();
            }
        
        }
    }
    class DealerPlayer(Dealer dealer) : Player(dealer,0)
    {
        public void InsuranceCheck()
        {
            if(deck.Hit()?.Id == CardId._A)
            {
                //todo insurance;
            }
            Console.WriteLine("Dealer's hand: {0}",deck.PrintDeck());
            if(deck.Hit()?.Value == 10)
            {
                //dealer has blackjack
            }
        }
        public void Play()
        {
            while(deck.Points() < 17)
            {
                deck.Hit();
            }
        }
    }
}