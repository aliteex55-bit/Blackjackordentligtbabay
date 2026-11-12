using System.Text.Json;
namespace Blackjack;
class GameStorage
{
    private readonly string path = Path.Combine(AppContext.BaseDirectory, "spelare.json");
    public string FilePath => path;
    public List<PlayerRecord> Load()
    {
        if (!File.Exists(path)) return new List<PlayerRecord>();
        List<PlayerRecord>? records = JsonSerializer.Deserialize<List<PlayerRecord>>(File.ReadAllText(path));
        // Reject damaged or ambiguous records so the existing file is not overwritten.
        if (records is null || records.Any(record => record is null || string.IsNullOrWhiteSpace(record.Name) || record.Wins < 0 || record.Losses < 0 || record.Draws < 0) ||
            records.Select(record => record.Name).Distinct(StringComparer.OrdinalIgnoreCase).Count() != records.Count)
            throw new JsonException("Invalid player records.");
        return records;
    }
    public void Save(List<PlayerRecord> records)
    {
        // Write a complete temporary file before replacing the previous save.
        string temporary = path + ".tmp";
        File.WriteAllText(temporary, JsonSerializer.Serialize(records, new JsonSerializerOptions { WriteIndented = true }));
        File.Move(temporary, path, true);
    }
}
