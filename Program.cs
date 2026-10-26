using System;

namespace Blackjack
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Blackjack ===");
            Console.WriteLine();

            Game game = new Game();
            game.PlayRound();
        }
    }
}
