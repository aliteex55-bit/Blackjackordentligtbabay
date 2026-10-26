using System;

namespace Blackjack
{
    class Program
    {
        static void Main(string[] args)
        {
            // Stage 2 check: the ace handling is the easiest bit to get wrong,
            // so build a few hands by hand and make sure the totals come out
            // the way they should before wiring up the real game.
            CheckHand("blackjack", "21", new Card("A", "Spades"), new Card("K", "Hearts"));
            CheckHand("two aces plus a nine", "21", new Card("A", "Spades"), new Card("A", "Hearts"), new Card("9", "Clubs"));
            CheckHand("soft 17", "17", new Card("A", "Spades"), new Card("6", "Hearts"));
            CheckHand("three aces", "13", new Card("A", "Spades"), new Card("A", "Hearts"), new Card("A", "Clubs"));
            CheckHand("a bust", "24", new Card("K", "Spades"), new Card("9", "Hearts"), new Card("5", "Clubs"));
        }

        // Little helper so the checks above stay readable.
        static void CheckHand(string label, string expected, params Card[] cards)
        {
            Hand hand = new Hand();

            foreach (Card card in cards)
            {
                hand.Add(card);
            }

            Console.WriteLine(label + ": " + hand + " - expected " + expected
                + (hand.IsBust ? ", bust" : "")
                + (hand.IsBlackjack ? ", blackjack" : ""));
        }
    }
}
