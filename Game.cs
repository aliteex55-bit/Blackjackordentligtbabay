namespace Blackjack;

enum Outcome { Win, Lose, Push }

// Game contains rules and state only. ConsoleUi handles presentation.
class Game
{
    private readonly Deck deck;
    public IReadOnlyList<Player> Players { get; }
    public Hand Dealer { get; } = new();
    public bool RebuiltDuringRound => deck.RebuiltDuringRound;
    public Game(IReadOnlyList<Player> players, Random random)
    {
        if (players.Count is < 1 or > 5) throw new ArgumentOutOfRangeException(nameof(players));
        Players = players;
        deck = new Deck(random);
    }
    public void StartRound()
    {
        deck.PrepareRound();
        Dealer.Clear();
        foreach (Player player in Players) player.Hand.Clear();
        // Deal one card to each participant, then repeat for the second card.
        for (int i = 0; i < 2; i++)
        {
            foreach (Player player in Players) player.Hand.Add(deck.Deal());
            Dealer.Add(deck.Deal());
        }
    }
    public Card Hit(Player player)
    {
        Card card = deck.Deal();
        player.Hand.Add(card);
        return card;
    }
    public Card DealerHit()
    {
        Card card = deck.Deal();
        Dealer.Add(card);
        return card;
    }
    public bool DealerShouldHit => Dealer.Total <= 16;
    public Outcome GetOutcome(Hand player)
    {
        // A player who busts loses even if the dealer later busts too.
        if (player.IsBust) return Outcome.Lose;
        if (Dealer.IsBust) return Outcome.Win;
        // A two-card natural beats a 21 made with three or more cards.
        if (player.Total == 21 && Dealer.Total == 21 && player.IsBlackjack != Dealer.IsBlackjack)
            return player.IsBlackjack ? Outcome.Win : Outcome.Lose;
        return player.Total > Dealer.Total ? Outcome.Win : player.Total < Dealer.Total ? Outcome.Lose : Outcome.Push;
    }
}
