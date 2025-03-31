namespace Blackjack
{
    class Game
    {
        int tokens = 5000;
        public void Play()
        {
            while(true)
            {
                SingleGame game = new SingleGame(tokens);
                game.Play();
                tokens = game.PlayersTokens();
                if(tokens > 0)
                {

                    Console.WriteLine("If you want to exit press ESC or Q, if you want to play press any key");
                    var read = Console.ReadKey();
                    if(read.Key == ConsoleKey.Q || read.Key == ConsoleKey.Escape)
                    {
                        Console.Clear();
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("GGWP");
                    return;
                }
            }
        }
    }
}