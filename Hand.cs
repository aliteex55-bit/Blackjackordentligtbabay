using System;
using System.Collections.Generic;

namespace Blackjack
{
    // The cards one player is holding, plus the score working out.
    class Hand
    {
        private List<Card> cards = new List<Card>();

        // True if this hand came out of a split. It matters because 21 on a
        // split hand is just 21 - it doesn't pay out as a real blackjack.
        public bool FromSplit { get; set; }

        public int CardCount
        {
            get { return cards.Count; }
        }

        public void Add(Card card)
        {
            cards.Add(card);
        }

        public void Clear()
        {
            cards.Clear();
            FromSplit = false;
        }

        // Add up the cards, then deal with aces.
        //
        // Every ace counts as 11 to start with. If that puts us over 21 we knock
        // one ace back down to 1 (so subtract 10) and check again. Repeat until
        // we're either under 21 or out of aces to demote. That's how A + A + 9
        // comes out as 21 instead of 31.
        public int Total
        {
            get
            {
                int total = 0;
                int aces = 0;

                foreach (Card card in cards)
                {
                    total += card.Value;

                    if (card.IsAce)
                    {
                        aces++;
                    }
                }

                while (total > 21 && aces > 0)
                {
                    total -= 10;
                    aces--;
                }

                return total;
            }
        }

        public bool IsBust
        {
            get { return Total > 21; }
        }

        // A "natural" - 21 on the first two cards. Worth knowing about because
        // it beats a 21 that took three or more cards to get to.
        public bool IsBlackjack
        {
            get { return cards.Count == 2 && Total == 21 && !FromSplit; }
        }

        // Two cards worth the same amount can be split into two hands.
        //
        // Comparing Value rather than rank on purpose - most tables let you
        // split a king and a ten because they're both worth 10.
        public bool CanSplit
        {
            get { return cards.Count == 2 && cards[0].Value == cards[1].Value; }
        }

        // Pulls the second card out of the hand so it can go start a new one.
        // Only used by the split.
        public Card RemoveSecondCard()
        {
            Card card = cards[1];
            cards.RemoveAt(1);
            return card;
        }

        // Prints the whole hand, e.g. "K of Spades, 7 of Hearts (17)".
        public override string ToString()
        {
            string text = "";

            for (int i = 0; i < cards.Count; i++)
            {
                if (i > 0)
                {
                    text += ", ";
                }

                text += cards[i].ToString();
            }

            return text + " (" + Total + ")";
        }

        // Used for the dealer's turn, where we only show the first card and
        // keep the rest face down until it's their turn to play.
        public string ToStringHidden()
        {
            if (cards.Count == 0)
            {
                return "nothing";
            }

            return cards[0] + ", [hidden card]";
        }
    }
}
