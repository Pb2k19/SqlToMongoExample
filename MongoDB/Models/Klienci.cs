namespace MongoDB.Models;

[BsonKnownTypes(typeof(KlientFirma))]
public class Klient
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.ObjectId)]
    public List<string> Pojazdy { get; set; } = new();

    [BsonRepresentation(BsonType.ObjectId)]
    public List<string> Naprawy { get; set; } = new();

    public required string Imie { get; set; }
    public required string Nazwisko { get; set; }
    public required string NumerTelefonu { get; set; }
    public required string Miejscowosc { get; set; }
    public required string KodPocztowy { get; set; }
    public required string Ulica { get; set; }
    public required string NumerDomuMieszkania { get; set; }
    public required string Haslo { get; set; }
    public required string Login { get; set; }

    [BsonIgnore]
    public int? SqlId { get; set; }
}

public class KlientFirma : Klient
{
    public required string Nip { get; set; }
    public required string NazwaFirmy { get; set; }
}
