using System;

namespace Blackjack
{
    // A single playing card. Nothing fancy - it just remembers what it is
    // and how much it's worth in blackjack.
    class Card
    {
        public string Rank { get; private set; }   // "A", "2" ... "10", "J", "Q", "K"
        public string Suit { get; private set; }   // "Hearts", "Spades", ...

        public Card(string rank, string suit)
        {
            Rank = rank;
            Suit = suit;
        }

        // Face cards are 10, the ace starts out as 11.
        // The ace can drop back down to 1 later, but that's the hand's job
        // to work out, not the card's.
        public int Value
        {
            get
            {
                if (Rank == "A")
                {
                    return 11;
                }

                if (Rank == "J" || Rank == "Q" || Rank == "K")
                {
                    return 10;
                }

                // Everything that's left is a plain number card.
                return int.Parse(Rank);
            }
        }

        public bool IsAce
        {
            get { return Rank == "A"; }
        }

        public override string ToString()
        {
            return Rank + " of " + Suit;
        }
    }
}
