namespace Blackjack;
// Presentation feature: returns rows without using Console.
class CardRenderer
{
    public string[] Render(Card? card)
    {
        // A missing card represents the dealer's face-down card.
        if (card is null)
            return new[] { "┌───────┐", "│▒▒▒▒▒▒▒│", "│▒▒▒▒▒▒▒│", "│▒▒▒▒▒▒▒│", "│▒▒▒▒▒▒▒│", "│▒▒▒▒▒▒▒│", "└───────┘" };
        return new[] { "┌───────┐", "│" + card.Rank.PadRight(7) + "│", "│       │", "│   " + card.Suit + "   │", "│       │", "│" + card.Rank.PadLeft(7) + "│", "└───────┘" };
    }
}
