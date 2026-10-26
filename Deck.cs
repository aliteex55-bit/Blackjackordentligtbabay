using System;
using System.Collections.Generic;

namespace Blackjack
{
    // A standard 52 card deck that we deal off the top of.
    class Deck
    {
        private List<Card> cards = new List<Card>();
        private Random rng = new Random();

        private static readonly string[] Ranks =
            { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

        private static readonly string[] Suits =
            { "Hearts", "Diamonds", "Clubs", "Spades" };

        public Deck()
        {
            Build();
            Shuffle();
        }

        public int CardsLeft
        {
            get { return cards.Count; }
        }

        // Fill the deck with all 52 combinations.
        private void Build()
        {
            cards.Clear();

            foreach (string suit in Suits)
            {
                foreach (string rank in Ranks)
                {
                    cards.Add(new Card(rank, suit));
                }
            }
        }

        // Fisher-Yates shuffle. Walk backwards through the list and swap each
        // card with a random one at or before it.
        public void Shuffle()
        {
            for (int i = cards.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);

                Card temp = cards[i];
                cards[i] = cards[j];
                cards[j] = temp;
            }
        }

        // Take the top card off the deck and hand it over.
        public Card Deal()
        {
            // If we somehow run dry mid game, rebuild and reshuffle so the
            // game doesn't just crash on the player.
            if (cards.Count == 0)
            {
                Build();
                Shuffle();
            }

            Card top = cards[0];
            cards.RemoveAt(0);
            return top;
        }
    }
}
