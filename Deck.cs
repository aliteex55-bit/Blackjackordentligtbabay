namespace Blackjack;

class Deck
{
    private readonly List<Card> cards = new();
    private readonly Random random;
    private static readonly string[] Ranks = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
    private static readonly string[] Suits = { "♥", "♦", "♣", "♠" };
    public Deck(Random random) { this.random = random; Rebuild(); }
    public int CardsLeft => cards.Count;
    public bool RebuiltDuringRound { get; private set; }
    public void PrepareRound()
    {
        RebuiltDuringRound = false;
        // The specified cutoff is checked between rounds, not after every card.
        if (CardsLeft < 15) Rebuild();
    }
    private void Rebuild()
    {
        cards.Clear();
        foreach (string suit in Suits)
            foreach (string rank in Ranks) cards.Add(new Card(rank, suit));
        // Fisher–Yates chooses from 0 through i, then swaps that card into position i.
        for (int i = cards.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (cards[i], cards[j]) = (cards[j], cards[i]);
        }
    }
    public Card Deal()
    {
        // A long round may use all remaining cards; report this exceptional refill.
        if (cards.Count == 0) { Rebuild(); RebuiltDuringRound = true; }
        Card card = cards[^1];
        cards.RemoveAt(cards.Count - 1);
        return card;
    }
}
