using System;

namespace Blackjack
{
    public class SingleGame
    {
        Dealer dealer;
        DealerPlayer dealerPlayer;
        Player player;
        public SingleGame()
        {
            dealer = new Dealer();
            dealerPlayer = new DealerPlayer(dealer);
            player = new(dealer,5000);
        }
        public SingleGame(int tokens)
        {
            dealer = new Dealer();
            dealerPlayer = new DealerPlayer(dealer);
            player = new(dealer,tokens);
        }
        public int PlayersTokens()
        {
            return player.Tokens();
        }
        public void Deal()
        {
            Player[] players = {
                new Player(dealer,5000),
                new Player(dealer,5000),
                new Player(dealer,5000),
                new Player(dealer,5000)
            };
            while(dealer.HasCards())
            {
                foreach(var p in players)
                {
                    p.Hit();
                }
            }
            for(int i = 0; i<4; ++i)
            {
                Console.WriteLine("id: {0}, cards: {1}",i,players[i].PrintDeck());
            }
        }
        public void Play()
        {
            player.MakeBet();
            Console.Clear();
            dealerPlayer.InsuranceCheck();
            player.Hit();
            player.Hit();
            Console.WriteLine(player.PrintDeck());
            while(player.Score() < 22 && !player.HasFolded && player.IsPlayling())
            {
                PlayerRound();
            }
            if(player.HasFolded || player.Score() > 21)
            {
                Console.WriteLine("Player has lost");
                Console.WriteLine("Player's cards: {0}", player.PrintDeck());
                Console.WriteLine("Dealer's cards: {0}",dealerPlayer.PrintDeck());
                return;
            }
            dealerPlayer.Play();
            if(dealerPlayer.Score() > 21 || player.Score() > dealerPlayer.Score())
            {
                Console.WriteLine("Player has won");
                Console.WriteLine("Player's cards: {0}", player.PrintDeck());
                Console.WriteLine("Dealer's cards: {0}",dealerPlayer.PrintDeck());
                player.Won();
            }
            else if (player.Score() == dealerPlayer.Score())
            {
                Console.WriteLine("Tie");
                Console.WriteLine("Player's cards: {0}", player.PrintDeck());
                Console.WriteLine("Dealer's cards: {0}",dealerPlayer.PrintDeck());
                player.Tie();
            }
            else
            {
                Console.WriteLine("Player has lost");
                Console.WriteLine("Player's cards: {0}", player.PrintDeck());
                Console.WriteLine("Dealer's cards: {0}",dealerPlayer.PrintDeck());
            }
        }
        private void PlayerRound()
        {
            var read = Console.ReadKey();
            switch(read.Key)
            {
                case ConsoleKey.H:
                    player.Hit();
                    break;
                case ConsoleKey.S:
                    player.Stand();
                    break;
                case ConsoleKey.F:
                    player.Fold();
                    break;
            }
            Console.WriteLine(player.PrintDeck());
        }
    }
}