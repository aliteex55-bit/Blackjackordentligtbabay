using System;
using System.Collections.Generic;

namespace Blackjack
{
    // Runs the rounds and keeps track of the player's chips.
    class Game
    {
        private const int StartingChips = 100;
        private const int Bet = 10;
        private const int MaxHands = 4;   // so you can't keep resplitting forever

        private Deck deck = new Deck();
        private Hand dealer = new Hand();
        private int chips = StartingChips;

        // The player normally has one hand, but splitting gives them more, so
        // these are lists. Each hand has its own bet at the matching index -
        // doubling down only raises the bet on the hand you doubled.
        private List<Hand> hands = new List<Hand>();
        private List<int> bets = new List<int>();

        public int Chips
        {
            get { return chips; }
        }

        // Can't bet what you haven't got.
        public bool CanPlayAnotherRound
        {
            get { return chips >= Bet; }
        }

        // Chips that aren't already riding on a hand this round. A split or a
        // double has to come out of this, not out of the full pile.
        private int ChipsFree
        {
            get
            {
                int committed = 0;

                foreach (int bet in bets)
                {
                    committed += bet;
                }

                return chips - committed;
            }
        }

        public void PlayRound()
        {
            hands.Clear();
            bets.Clear();
            dealer.Clear();

            // Start with the one hand and the one bet.
            Hand first = new Hand();
            hands.Add(first);
            bets.Add(Bet);

            Console.WriteLine("Betting " + Bet + " chips. You have " + chips + ".");
            Console.WriteLine();

            // Two cards each, dealt one at a time like a real table.
            first.Add(deck.Deal());
            dealer.Add(deck.Deal());
            first.Add(deck.Deal());
            dealer.Add(deck.Deal());

            Console.WriteLine("Dealer has: " + dealer.ToStringHidden());
            Console.WriteLine("You have:   " + first);
            Console.WriteLine();

            // Play each hand in turn. Note the list can grow while we're going
            // through it - a split drops the new hand in right after the one
            // being played, and the loop picks it up on the way round.
            for (int i = 0; i < hands.Count; i++)
            {
                if (hands.Count > 1)
                {
                    Console.WriteLine("-- Hand " + (i + 1) + " of " + hands.Count + ": " + hands[i]);
                }

                // If the hand got dealt 21 there's nothing to decide.
                if (!hands[i].IsBlackjack)
                {
                    PlayerTurn(i);
                }
            }

            // No point the dealer playing on if every hand already went over.
            if (AnyHandStillIn())
            {
                DealerTurn();
            }

            Settle();
        }

        private bool AnyHandStillIn()
        {
            foreach (Hand hand in hands)
            {
                if (!hand.IsBust)
                {
                    return true;
                }
            }

            return false;
        }

        // Keep asking what to do with this hand until it's finished with.
        private void PlayerTurn(int index)
        {
            Hand hand = hands[index];

            while (true)
            {
                // Doubling and splitting are only on the table while the hand
                // is still just two cards, and only if there are chips spare to
                // cover the extra bet.
                bool canDouble = hand.CardCount == 2 && ChipsFree >= bets[index];
                bool canSplit = hand.CanSplit && hands.Count < MaxHands && ChipsFree >= bets[index];

                Console.Write(BuildPrompt(canDouble, canSplit));
                string? answer = Console.ReadLine();

                if (answer == null)
                {
                    // Nothing left on the input, so treat it as standing
                    // rather than looping forever.
                    return;
                }

                answer = answer.Trim().ToLower();

                if (answer == "h" || answer == "hit")
                {
                    Card drawn = deck.Deal();
                    hand.Add(drawn);

                    Console.WriteLine("You drew the " + drawn + ".");
                    Console.WriteLine("You have: " + hand);
                    Console.WriteLine();

                    if (hand.IsBust)
                    {
                        Console.WriteLine("That's a bust.");
                        Console.WriteLine();
                        return;
                    }
                }
                else if (answer == "s" || answer == "stand")
                {
                    Console.WriteLine("You stand on " + hand.Total + ".");
                    Console.WriteLine();
                    return;
                }
                else if ((answer == "d" || answer == "double") && canDouble)
                {
                    DoubleDown(index);
                    return;
                }
                else if ((answer == "p" || answer == "split") && canSplit)
                {
                    // After the split we carry on playing this same hand - it
                    // has a new second card now, so it might even split again.
                    Split(index);
                }
                else
                {
                    Console.WriteLine("Didn't catch that. " + BuildHelp(canDouble, canSplit));
                }
            }
        }

        // Only offer the moves that are actually legal right now.
        private string BuildPrompt(bool canDouble, bool canSplit)
        {
            string prompt = "Hit (h), stand (s)";

            if (canDouble)
            {
                prompt += ", double (d)";
            }

            if (canSplit)
            {
                prompt += ", split (p)";
            }

            return prompt + "? ";
        }

        private string BuildHelp(bool canDouble, bool canSplit)
        {
            string help = "Type h to hit or s to stand";

            if (canDouble)
            {
                help += ", d to double";
            }

            if (canSplit)
            {
                help += ", p to split";
            }

            return help + ".";
        }

        // Double the bet on this hand, take exactly one more card, and that's
        // the hand over with - win or lose.
        private void DoubleDown(int index)
        {
            Hand hand = hands[index];

            bets[index] = bets[index] * 2;

            Card drawn = deck.Deal();
            hand.Add(drawn);

            Console.WriteLine("Bet doubled to " + bets[index] + ". You drew the " + drawn + ".");
            Console.WriteLine("You have: " + hand);

            if (hand.IsBust)
            {
                Console.WriteLine("That's a bust.");
            }
            else
            {
                Console.WriteLine("Standing on " + hand.Total + " - a double is one card only.");
            }

            Console.WriteLine();
        }

        // Split the pair into two hands, each getting a fresh second card and
        // its own bet of the same size.
        private void Split(int index)
        {
            Hand original = hands[index];
            Hand moved = new Hand();

            // Both halves are split hands now, so neither can score a payout
            // as a natural blackjack.
            original.FromSplit = true;
            moved.FromSplit = true;

            moved.Add(original.RemoveSecondCard());

            original.Add(deck.Deal());
            moved.Add(deck.Deal());

            // Slot the new hand in directly after this one so it gets played
            // next rather than at the end.
            hands.Insert(index + 1, moved);
            bets.Insert(index + 1, bets[index]);

            Console.WriteLine("Split - that's " + bets[index] + " more chips on the table.");
            Console.WriteLine("This hand:  " + original);
            Console.WriteLine("Other hand: " + moved);
            Console.WriteLine();

            // Note: proper tables only give you one card on split aces and
            // won't let you hit them again. Not bothering with that here -
            // split aces play like any other hand.
        }

        // The dealer doesn't get a choice - they hit on 16 or less and stand
        // on anything 17 or over.
        private void DealerTurn()
        {
            Console.WriteLine("Dealer turns over: " + dealer);

            while (dealer.Total < 17)
            {
                Card drawn = deck.Deal();
                dealer.Add(drawn);

                Console.WriteLine("Dealer draws the " + drawn + ". Now on " + dealer.Total + ".");
            }

            if (dealer.IsBust)
            {
                Console.WriteLine("Dealer busts.");
            }
            else
            {
                Console.WriteLine("Dealer stands on " + dealer.Total + ".");
            }

            Console.WriteLine();
        }

        // Work out what each hand did against the dealer and move the chips.
        private void Settle()
        {
            Console.WriteLine("Dealer: " + dealer);
            Console.WriteLine();

            for (int i = 0; i < hands.Count; i++)
            {
                string label = hands.Count > 1 ? "Hand " + (i + 1) + ": " : "You: ";

                Console.WriteLine(label + hands[i]);
                chips += SettleHand(hands[i], bets[i]);
            }

            Console.WriteLine();
            Console.WriteLine("You now have " + chips + " chips.");
            Console.WriteLine();
        }

        // Says what happened to one hand and returns what it did to the chip
        // pile - positive for a win, negative for a loss, zero for a push.
        //
        // Order matters here - busts get checked first, then blackjacks, then
        // it's just the higher total.
        private int SettleHand(Hand hand, int bet)
        {
            if (hand.IsBust)
            {
                Console.WriteLine("  Over 21. Lost " + bet + ".");
                return -bet;
            }

            if (dealer.IsBust)
            {
                Console.WriteLine("  Dealer busted. Won " + bet + "!");
                return bet;
            }

            if (hand.IsBlackjack && !dealer.IsBlackjack)
            {
                // A natural blackjack pays one and a half times the bet, which
                // is the usual house rule. Integer division is fine while the
                // bet is an even number.
                int payout = bet + (bet / 2);

                Console.WriteLine("  Blackjack! That pays " + payout + ".");
                return payout;
            }

            if (dealer.IsBlackjack && !hand.IsBlackjack)
            {
                Console.WriteLine("  Dealer had blackjack. Lost " + bet + ".");
                return -bet;
            }

            if (hand.Total > dealer.Total)
            {
                Console.WriteLine("  Won " + bet + " with " + hand.Total + "!");
                return bet;
            }

            if (hand.Total < dealer.Total)
            {
                Console.WriteLine("  Lost " + bet + " to the dealer's " + dealer.Total + ".");
                return -bet;
            }

            Console.WriteLine("  Push on " + hand.Total + " - bet comes back.");
            return 0;
        }
    }
}
