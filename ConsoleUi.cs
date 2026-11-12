using System.Text.Json;
using System.Text;

namespace Blackjack;

class ConsoleUi
{
    private readonly GameStorage storage = new();
    private readonly CardRenderer renderer = new();
    private bool inputEnded;

    public void Run()
    {
        Console.OutputEncoding = Encoding.UTF8;
        List<PlayerRecord> records;
        try { records = storage.Load(); }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException or JsonException)
        {
            Console.WriteLine("Kunde inte läsa spelarfilen. Den har inte ändrats. Kontrollera filen och försök igen.");
            Console.WriteLine(storage.FilePath);
            return;
        }

        Console.WriteLine("=== Tjugoett ===");
        int count = AskCount();
        if (inputEnded) return;
        List<Player> players = new();
        for (int i = 0; i < count; i++)
        {
            while (true)
            {
                Console.Write($"Namn på spelare {i + 1}: ");
                string? name = Console.ReadLine()?.Trim();
                if (name is null) return;
                if (name.Length == 0) { Console.WriteLine("Ange ett namn."); continue; }
                if (players.Any(player => string.Equals(player.Name, name, StringComparison.OrdinalIgnoreCase)))
                { Console.WriteLine("Välj ett annat namn; spelarna behöver olika namn."); continue; }
                // Reuse the saved record when this name has played before.
                PlayerRecord? record = records.Find(item => string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase));
                if (record is null) { record = new PlayerRecord { Name = name }; records.Add(record); }
                players.Add(new Player(record));
                break;
            }
        }

        Game game = new(players, new Random());
        do
        {
            game.StartRound();
            Console.WriteLine("\nGivarens kort:");
            ShowHand(game.Dealer, true);
            foreach (Player player in players)
            {
                Console.WriteLine($"\n{player.Name}s tur:");
                ShowHand(player.Hand, false);
                if (player.Hand.IsBlackjack) Console.WriteLine("Naturlig tjugoett!");
                while (!player.Hand.IsBust && !player.Hand.IsBlackjack && !inputEnded)
                {
                    Console.Write("Köp (k) eller stanna (s): ");
                    string? answer = Console.ReadLine();
                    // An interrupted round has no result; leave the previous save untouched.
                    if (answer is null)
                    {
                        Console.WriteLine("\nInmatningen avslutades. Rundan avbröts och inga resultat sparades.");
                        return;
                    }
                    answer = answer.Trim().ToLowerInvariant();
                    if (answer is "s" or "stanna") break;
                    if (answer is not ("k" or "köp" or "kop"))
                    { Console.WriteLine("Skriv k för köp eller s för stanna."); continue; }
                    Card card = game.Hit(player);
                    Console.WriteLine($"Du fick {card}.");
                    ShowHand(player.Hand, false);
                }
                if (player.Hand.IsBust) Console.WriteLine("Spräckt!");
            }

            // The dealer completes the round even when every player has busted.
            Console.WriteLine("\nGivaren vänder sitt dolda kort:");
            ShowHand(game.Dealer, false);
            while (game.DealerShouldHit)
            {
                Card card = game.DealerHit();
                Console.WriteLine($"Givaren köper {card}.");
                ShowHand(game.Dealer, false);
            }
            Console.WriteLine(game.Dealer.IsBust ? "Givaren spräckte." : "Givaren stannar.");
            if (game.RebuiltDuringRound)
                Console.WriteLine("Kortleken tog slut under rundan; en ny blandad lek användes.");

            foreach (Player player in players)
            {
                Outcome outcome = game.GetOutcome(player.Hand);
                string result;
                switch (outcome)
                {
                    case Outcome.Win: player.Record.Wins++; result = "vinst"; break;
                    case Outcome.Lose: player.Record.Losses++; result = "förlust"; break;
                    default: player.Record.Draws++; result = "lika"; break;
                }
                Console.WriteLine($"{player.Name}: {result}. Totalt: {player.Record.Wins} vinster, {player.Record.Losses} förluster, {player.Record.Draws} lika.");
            }
            try { storage.Save(records); }
            catch (Exception error) when (error is IOException or UnauthorizedAccessException)
            { Console.WriteLine("Resultaten kunde inte sparas. Kontrollera skrivbehörighet och ledigt utrymme."); }
        } while (!inputEnded && AskAgain());
        Console.WriteLine("Tack för spelet!");
    }

    private int AskCount()
    {
        while (true)
        {
            Console.Write("Hur många spelare (1–5)? ");
            string? answer = Console.ReadLine();
            if (answer is null) { inputEnded = true; return 0; }
            if (int.TryParse(answer, out int number) && number is >= 1 and <= 5) return number;
            Console.WriteLine("Ange en siffra mellan 1 och 5.");
        }
    }

    private bool AskAgain()
    {
        while (true)
        {
            Console.Write("Spela en runda till? (j/n): ");
            string? answer = Console.ReadLine()?.Trim().ToLowerInvariant();
            if (answer is null or "n" or "nej") return false;
            if (answer is "j" or "ja") return true;
            Console.WriteLine("Skriv j eller n.");
        }
    }

    private void ShowHand(Hand hand, bool hideSecondCard)
    {
        // Keep the second dealer card hidden until the dealer's turn.
        List<(Card? card, string[] rows)> rendered = hand.Cards.Select((card, index) =>
        {
            Card? visible = hideSecondCard && index == 1 ? null : card;
            return (visible, renderer.Render(visible));
        }).ToList();
        ConsoleColor oldForeground = Console.ForegroundColor;
        ConsoleColor oldBackground = Console.BackgroundColor;
        // Print the same row of every card before moving to the next row.
        for (int row = 0; row < 7; row++)
        {
            foreach (var (card, rows) in rendered)
            {
                if (!Console.IsOutputRedirected)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = card?.Suit is "♥" or "♦" ? ConsoleColor.Red : ConsoleColor.Black;
                }
                Console.Write(rows[row]);
                // Restore terminal colors after each card rather than changing later text.
                if (!Console.IsOutputRedirected) { Console.ForegroundColor = oldForeground; Console.BackgroundColor = oldBackground; }
                Console.Write("  ");
            }
            Console.WriteLine();
        }
        if (!hideSecondCard) Console.WriteLine($"Värde: {hand.Total}{(hand.IsSoft ? " (mjuk)" : "")}");
    }
}
