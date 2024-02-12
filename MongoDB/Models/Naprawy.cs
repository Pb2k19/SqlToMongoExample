namespace MongoDB.Models;

public class Naprawa
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.ObjectId)]
    public string IdPojazdu { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.ObjectId)]
    public string? IdKlienta { get; set; } = null;

    public DateTime DataRozpoczecia { get; set; }
    public DateTime? DataZakonczenia { get; set; }
    public required string Opis { get; set; }
    public required string Status { get; set; }
    public Faktura? Faktura { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public List<string> Pracownicy { get; set; } = new();


    [BsonIgnore]
    public int? SqlIdPojazdu { get; set; }

    [BsonIgnore]
    public int? SqlId { get; set; }
}
