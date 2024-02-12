namespace MongoDB.Models;

public class Faktura
{
    public int Numer { get; set; }
    public List<ElementFaktury> ElementyFaktury { get; set; } = new();    
    public required string MetodaPlatnosci { get; set; }
    public DateTime TerminPlatnosci { get; set; }
    public DateTime DataWystawienia { get; set; }

    [BsonIgnore]
    public int? SqlIdFaktury { get; set; }
}
