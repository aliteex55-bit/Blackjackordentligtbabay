namespace Blackjack;

class Hand
{
    private readonly List<Card> cards = new();
    public IReadOnlyList<Card> Cards => cards;
    public void Add(Card card) => cards.Add(card);
    public void Clear() => cards.Clear();
    public int Total
    {
        get
        {
            // Start with every ace worth 11; reduce aces to 1 until the hand fits.
            int total = cards.Sum(card => card.Value);
            int aces = cards.Count(card => card.IsAce);
            while (total > 21 && aces-- > 0) total -= 10;
            return total;
        }
    }
    // A soft hand can count one ace as 11 without going over 21.
    public bool IsSoft => cards.Any(card => card.IsAce) &&
        cards.Sum(card => card.IsAce ? 1 : card.Value) + 10 <= 21;
    public bool IsBust => Total > 21;
    public bool IsBlackjack => cards.Count == 2 && Total == 21;
}
