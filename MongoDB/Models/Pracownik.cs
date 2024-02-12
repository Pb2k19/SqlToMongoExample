namespace MongoDB.Models;

internal class Pracownik
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.ObjectId)]
    public List<string> Naprawy { get; set; } = new();

    public required string Imie { get; set; }
    public required string Nazwisko { get; set; }
    public required string NumerTelefonu { get; set; }
    public required string Miejscowosc { get; set; }
    public required string KodPocztowy { get; set; }
    public required string Ulica { get; set; }
    public required string NumerDomuMieszkania { get; set; }
    public required string NazwaStanowiska { get; set; }
    public int PoziomDostepu { get; set; }
    public DateTime DataZatrudnienia { get; set; }

    [BsonIgnore]
    public int? SqlId { get; set; }
}
