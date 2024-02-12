namespace MongoDB.Models;

public class Pojazd
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.ObjectId)]
    public string? IdKlienta { get; set; } = null;

    [BsonRepresentation(BsonType.ObjectId)]
    public List<string> Naprawy { get; set; } = new();

    public required string Marka { get; set; }
    public required string Model { get; set; }
    public required string Opis { get; set; }
    public required string Typ { get; set; }
    public required string Wersja { get; set; }
    public required int MocSilnikaKm { get; set; }
    public required int PojemnoscSilnikaCm3 { get; set; }
    public required string NumerVin { get; set; }

    [BsonIgnore]
    public int? SqlId { get; set; }

    [BsonIgnore]
    public int? SqlIdKlienta { get; set; }
}
