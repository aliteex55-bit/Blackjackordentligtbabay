namespace Blackjack;
class Player
{
    public string Name => Record.Name;
    public Hand Hand { get; } = new();
    public PlayerRecord Record { get; }
    public Player(PlayerRecord record) { Record = record; }
}
class PlayerRecord
{
    public string Name { get; set; } = "";
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Draws { get; set; }
}
