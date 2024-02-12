namespace MongoDB.Models;

public class Zamowienie
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.ObjectId)]
    public List<string> ElementyZamowienia { get; set; } = new();

    public DateTime DataZlozenia { get; set; } = DateTime.Now;
    public string Status { get; set; } = string.Empty;
    public string Opis { get; set; } = string.Empty;

    [BsonIgnore]
    public int? SqlId { get; set; }
}