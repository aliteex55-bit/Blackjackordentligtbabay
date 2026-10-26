using System;

namespace Blackjack
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Blackjack ===");
            Console.WriteLine("Dealer stands on 17. Blackjack pays 3 to 2.");
            Console.WriteLine("You can double down or split a pair on your first two cards.");
            Console.WriteLine();

            Game game = new Game();

            while (true)
            {
                game.PlayRound();

                // Out of chips means the game is over whether they wanted to
                // keep going or not.
                if (!game.CanPlayAnotherRound)
                {
                    Console.WriteLine("You're out of chips. That's the end of that.");
                    break;
                }

                if (!AskPlayAgain())
                {
                    Console.WriteLine("You walk away with " + game.Chips + " chips. Thanks for playing!");
                    break;
                }

                Console.WriteLine("------------------------------");
                Console.WriteLine();
            }
        }

        // Returns true if the player wants another round.
        static bool AskPlayAgain()
        {
            while (true)
            {
                Console.Write("Play again? (y/n) ");
                string? answer = Console.ReadLine();

                // Ran out of input, so stop rather than asking again forever.
                if (answer == null)
                {
                    return false;
                }

                answer = answer.Trim().ToLower();

                if (answer == "y" || answer == "yes")
                {
                    return true;
                }

                if (answer == "n" || answer == "no")
                {
                    return false;
                }

                Console.WriteLine("Type y or n.");
            }
        }
    }
}
