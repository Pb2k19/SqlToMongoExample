namespace MongoDB.Models;

public class ElementFaktury
{
    public required string Nazwa { get; set; }
    public required string Opis { get; set; }
    public required int Liczba { get; set; }
    public required decimal Cena { get; set; }
    public required int StawkaVat { get; set; }
}