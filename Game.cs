using System;

namespace Blackjack
{
    // Runs one round of blackjack: deal, player's turn, dealer's turn, compare.
    class Game
    {
        private Deck deck = new Deck();
        private Hand player = new Hand();
        private Hand dealer = new Hand();

        public void PlayRound()
        {
            player.Clear();
            dealer.Clear();

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

            ShowResult();
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

        // Work out who actually won. Order matters here - busts get checked
        // first, then blackjacks, then it's just the higher total.
        private void ShowResult()
        {
            Console.WriteLine("You:    " + player);
            Console.WriteLine("Dealer: " + dealer);
            Console.WriteLine();

            if (player.IsBust)
            {
                Console.WriteLine("You went over 21. Dealer wins.");
            }
            else if (dealer.IsBust)
            {
                Console.WriteLine("Dealer went over 21. You win!");
            }
            else if (player.IsBlackjack && !dealer.IsBlackjack)
            {
                Console.WriteLine("Blackjack! You win.");
            }
            else if (dealer.IsBlackjack && !player.IsBlackjack)
            {
                Console.WriteLine("Dealer had blackjack. Dealer wins.");
            }
            else if (player.Total > dealer.Total)
            {
                Console.WriteLine("You win with " + player.Total + "!");
            }
            else if (player.Total < dealer.Total)
            {
                Console.WriteLine("Dealer wins with " + dealer.Total + ".");
            }
            else
            {
                Console.WriteLine("Push - same total, nobody wins.");
            }

            Console.WriteLine();
        }
    }
}
