using System;

namespace Blackjack
{
    class Program
    {
        static void Main(string[] args)
        {
            // Stage 1 check: build a deck and deal a few cards off it so we can
            // see the shuffle actually working before anything else gets added.
            Deck deck = new Deck();

            Console.WriteLine("Deck built with " + deck.CardsLeft + " cards.");
            Console.WriteLine();

            for (int i = 0; i < 5; i++)
            {
                Card card = deck.Deal();
                Console.WriteLine(card + "  (worth " + card.Value + ")");
            }

            Console.WriteLine();
            Console.WriteLine(deck.CardsLeft + " cards left in the deck.");
        }
    }
}
