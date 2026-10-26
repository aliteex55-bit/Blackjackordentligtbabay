using System;

namespace Blackjack
{
    // Runs the rounds and keeps track of the player's chips.
    class Game
    {
        private const int StartingChips = 100;
        private const int Bet = 10;

        private Deck deck = new Deck();
        private Hand player = new Hand();
        private Hand dealer = new Hand();
        private int chips = StartingChips;

        public int Chips
        {
            get { return chips; }
        }

        // Can't bet what you haven't got.
        public bool CanPlayAnotherRound
        {
            get { return chips >= Bet; }
        }

        public void PlayRound()
        {
            player.Clear();
            dealer.Clear();

            Console.WriteLine("Betting " + Bet + " chips. You have " + chips + ".");
            Console.WriteLine();

            // Two cards each, dealt one at a time like a real table.
            player.Add(deck.Deal());
            dealer.Add(deck.Deal());
            player.Add(deck.Deal());
            dealer.Add(deck.Deal());

            Console.WriteLine("Dealer has: " + dealer.ToStringHidden());
            Console.WriteLine("You have:   " + player);
            Console.WriteLine();

            // If the player got dealt 21 there's nothing to decide, so skip
            // straight to working out who won.
            if (!player.IsBlackjack)
            {
                PlayerTurn();
            }

            // No point the dealer playing on if the player already went over.
            if (!player.IsBust)
            {
                DealerTurn();
            }

            Settle();
        }

        // Keep asking hit or stand until the player stands or busts.
        private void PlayerTurn()
        {
            while (true)
            {
                Console.Write("Hit or stand? ");
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
                    player.Add(drawn);

                    Console.WriteLine("You drew the " + drawn + ".");
                    Console.WriteLine("You have: " + player);
                    Console.WriteLine();

                    if (player.IsBust)
                    {
                        Console.WriteLine("That's a bust.");
                        Console.WriteLine();
                        return;
                    }
                }
                else if (answer == "s" || answer == "stand")
                {
                    Console.WriteLine("You stand on " + player.Total + ".");
                    Console.WriteLine();
                    return;
                }
                else
                {
                    Console.WriteLine("Type h to hit or s to stand.");
                }
            }
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

        // Work out who won, say so, and move the chips.
        //
        // Order matters here - busts get checked first, then blackjacks, then
        // it's just the higher total.
        private void Settle()
        {
            Console.WriteLine("You:    " + player);
            Console.WriteLine("Dealer: " + dealer);
            Console.WriteLine();

            if (player.IsBust)
            {
                Console.WriteLine("You went over 21. Dealer wins.");
                chips -= Bet;
            }
            else if (dealer.IsBust)
            {
                Console.WriteLine("Dealer went over 21. You win!");
                chips += Bet;
            }
            else if (player.IsBlackjack && !dealer.IsBlackjack)
            {
                // A natural blackjack pays one and a half times the bet, which
                // is the usual house rule. Integer division is fine while the
                // bet is an even number.
                int payout = Bet + (Bet / 2);

                Console.WriteLine("Blackjack! That pays " + payout + ".");
                chips += payout;
            }
            else if (dealer.IsBlackjack && !player.IsBlackjack)
            {
                Console.WriteLine("Dealer had blackjack. Dealer wins.");
                chips -= Bet;
            }
            else if (player.Total > dealer.Total)
            {
                Console.WriteLine("You win with " + player.Total + "!");
                chips += Bet;
            }
            else if (player.Total < dealer.Total)
            {
                Console.WriteLine("Dealer wins with " + dealer.Total + ".");
                chips -= Bet;
            }
            else
            {
                Console.WriteLine("Push - same total, your bet comes back.");
            }

            Console.WriteLine("You now have " + chips + " chips.");
            Console.WriteLine();
        }
    }
}
