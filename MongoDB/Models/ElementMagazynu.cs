namespace MongoDB.Models;

public class ElementMagazynu
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public string Nazwa { get; set; } = string.Empty;
    public string Opis { get; set; } = string.Empty;
    public string NazwaProducenta { get; set; } = string.Empty;
    public string KodProducenta { get; set; } = string.Empty;
    public string KrajPochodzenia { get; set; } = string.Empty;
    public required int Liczba { get; set; }
    public required decimal Cena { get; set; } = Random.Shared.Next(20, 1500);
    public required int StawkaVat { get; set; } = 23;
    public int MasaG { get; set; }

    [BsonIgnore]
    public int? SqlId { get; set; }
}
