namespace Blackjack;

// The hand decides how an ace is counted.
class Card
{
    public string Rank { get; }
    public string Suit { get; }
    public Card(string rank, string suit) { Rank = rank; Suit = suit; }
    public bool IsAce => Rank == "A";
    public int Value => IsAce ? 11 : Rank is "J" or "Q" or "K" ? 10 : int.Parse(Rank);
    public override string ToString() => Suit + Rank;
}
