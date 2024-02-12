global using MongoDB.Bson;
global using MongoDB.Bson.Serialization.Attributes;
using Dapper;
using Microsoft.Data.SqlClient;
using MongoDB.Driver;
using MongoDB.Models;

#region Connection
const string sqlConnectionString = "Server=localhost;Database=warsztat;Trusted_Connection=True;TrustServerCertificate=True;";

const string connectionString = "mongodb://localhost:27017/";
const string dbName = "warsztat";

const string klienciCollectionName = "klienci";
const string pojazdyCollectionName = "pojazdy";
const string naprawyCollectionName = "naprawy";
const string pracownicyCollectionName = "pracownicy";
const string zamowieniaCollectionName = "zamowienia";
const string magazynCollectionName = "czesci";

MongoClient client = new(connectionString);
IMongoDatabase db = client.GetDatabase(dbName);

var klienciCollection = db.GetCollection<Klient>(klienciCollectionName);
var pojazdyCollection = db.GetCollection<Pojazd>(pojazdyCollectionName);
var naprawyCollection = db.GetCollection<Naprawa>(naprawyCollectionName);
var pracownicyCollection = db.GetCollection<Pracownik>(pracownicyCollectionName);
var zamowieniaCollection = db.GetCollection<Zamowienie>(zamowieniaCollectionName);
var magazynCollection = db.GetCollection<ElementMagazynu>(magazynCollectionName);
#endregion

string query =
    $"""
    select  k.id as {nameof(Klient.SqlId)}, k.imie as {nameof(Klient.Imie)}, k.nazwisko as {nameof(Klient.Nazwisko)}, 
    k.nip as {nameof(KlientFirma.Nip)}, k.nazwa_firmy as {nameof(KlientFirma.NazwaFirmy)}, 
    k.login as {nameof(KlientFirma.Login)}, CONVERT(VARCHAR(1000), k.haslo, 2) as {nameof(KlientFirma.Haslo)}, 
    k.numer_telefonu as {nameof(KlientFirma.NumerTelefonu)}, 
    a.numer_budynku as {nameof(KlientFirma.NumerDomuMieszkania)}, a.kod_pocztowy as {nameof(KlientFirma.KodPocztowy)}, 
    a.miejscowosc as {nameof(KlientFirma.Miejscowosc)}, a.ulica as {nameof(KlientFirma.Ulica)}
    from klienci as k
    join adresy as a
    on k.id_adresu = a.id
    where k.nip is not null
    """;


var klienci = new List<Klient>();
var pojazdy = new List<Pojazd>();
var pracownicy = new List<Pracownik>();
var zamowienia = new List<Zamowienie>();
var magazyn = new List<ElementMagazynu>();
var naprawy = new List<Naprawa>();

await using var connection = new SqlConnection(sqlConnectionString);
klienci.AddRange(connection.Query<KlientFirma>(query));

query =
   $"""
    select k.id as {nameof(Klient.SqlId)}, k.imie as {nameof(Klient.Imie)}, k.nazwisko as {nameof(Klient.Nazwisko)},
    k.login as {nameof(Klient.Login)}, CONVERT(VARCHAR(1000), k.haslo, 2) as {nameof(Klient.Haslo)}, 
    k.numer_telefonu as {nameof(KlientFirma.NumerTelefonu)}, 
    a.numer_budynku as {nameof(Klient.NumerDomuMieszkania)}, a.kod_pocztowy as {nameof(Klient.KodPocztowy)}, 
    a.miejscowosc as {nameof(Klient.Miejscowosc)}, a.ulica as {nameof(Klient.Ulica)}
    from klienci as k
    join adresy as a
    on k.id_adresu = a.id
    where k.nip is null
    """;

klienci.AddRange(connection.Query<Klient>(query));
await klienciCollection.InsertManyAsync(klienci);

query =
   $"""
    select pk.id as {nameof(Pojazd.SqlId)}, gp.id_klienta as {nameof(Pojazd.SqlIdKlienta)}, p.marka as {nameof(Pojazd.Marka)}, 
    p.model as {nameof(Pojazd.Model)}, p.typ as {nameof(Pojazd.Typ)}, wp.nazwa as {nameof(Pojazd.Wersja)}, 
    wp.pojemnosc_silnika_cm3 as {nameof(Pojazd.PojemnoscSilnikaCm3)},
    wp.moc_silnika_km as {nameof(Pojazd.MocSilnikaKm)}, pk.opis as {nameof(Pojazd.Opis)}, pk.numer_vin as {nameof(Pojazd.NumerVin)}
    from pojazdy as p
    join wersja_pojazdu as wp
    on wp.id_pojazdu = p.id
    join pojazdy_klientow as pk
    on wp.id = pk.id_wersji_pojazdu
    left join grupa_pojazdow_klienta as gp
    on gp.id_pojazdu = pk.id
    """;

pojazdy.AddRange(connection.Query<Pojazd>(query));
foreach (var pojazd in pojazdy)
{
    var klient = klienci.FirstOrDefault(k => k.SqlId == pojazd.SqlIdKlienta);
    if(klient != null)
    {
        pojazd.Id = ObjectId.GenerateNewId().ToString();
        pojazd.IdKlienta = klient.Id;
        klient.Pojazdy.Add(pojazd.Id);
        await klienciCollection.ReplaceOneAsync(k => k.Id.Equals(klient.Id), klient);
    }
}
await pojazdyCollection.InsertManyAsync(pojazdy);

query =
   $"""
    select p.id as {nameof(Pracownik.SqlId)}, p.imie as {nameof(Pracownik.Imie)}, p.nazwisko as {nameof(Pracownik.Nazwisko)}, p.data_zatrudnienia as {nameof(Pracownik.DataZatrudnienia)},
    s.nazwa as {nameof(Pracownik.NazwaStanowiska)}, s.poziom_dostepu as {nameof(Pracownik.PoziomDostepu)}, a.kod_pocztowy as {nameof(Pracownik.KodPocztowy)}, a.miejscowosc as {nameof(Pracownik.Miejscowosc)}, 
    a.ulica as {nameof(Pracownik.Ulica)}, a.numer_budynku  as {nameof(Pracownik.NumerDomuMieszkania)}
    from pracownicy as p
    join stanowiska as s
    on s.id = p.id_stanowiska
    join adresy as a
    on a.id = p.id_adresu
    """;

pracownicy.AddRange(connection.Query<Pracownik>(query));
await pracownicyCollection.InsertManyAsync(pracownicy);

query =
   $"""
    select n.id as {nameof(Naprawa.SqlId)}, n.id_pojazdu as {nameof(Naprawa.SqlIdPojazdu)}, n.opis_zlecenia as {nameof(Naprawa.Opis)}, n.data_rozpoczecia as {nameof(Naprawa.DataRozpoczecia)}, 
    n.data_zakończenia as {nameof(Naprawa.DataZakonczenia)}, sn.nazwa as {nameof(Naprawa.Status)},
    f.id as {nameof(Faktura.SqlIdFaktury)}, f.termin_platnosci as {nameof(Faktura.TerminPlatnosci)}, 
    f.data_wystawienia as {nameof(Faktura.DataWystawienia)}, f.numer as {nameof(Faktura.Numer)}, 
    mp.nazwa as {nameof(Faktura.MetodaPlatnosci)}
    from naprawy as n
    join status_naprawy as sn
    on sn.id = n.id_statusu
    left join faktury as f
    on f.id_naprawy = n.id
    left join metody_platnosci as mp
    on mp.id = f.id_metody_platnosci
    """;

naprawy.AddRange(connection.Query<Naprawa, Faktura?, Naprawa>(query, (naprawa, faktura) => { naprawa.Faktura = faktura; return naprawa; }, splitOn:nameof(Faktura.SqlIdFaktury)));
foreach (var naprawa in naprawy)
{
    if (naprawa.Faktura is not null && naprawa.Faktura.SqlIdFaktury is not null)
    {
        var p = new
        {
            Param = naprawa.Faktura.SqlIdFaktury
        };

        query =
        $"""
         select e.cena_netto as {nameof(ElementFaktury.Cena)}, e.stawka_vat as {nameof(ElementFaktury.StawkaVat)}, 
         c.nazwa as {nameof(ElementFaktury.Nazwa)}, c.opis  as {nameof(ElementFaktury.Opis)}, c.liczba_sztuk as {nameof(ElementFaktury.Liczba)}
         from elementy_faktury as e
         join czesci as c
         on c.id = e.id_czesci
         where id_faktury = @Param
         """;

        naprawa.Faktura.ElementyFaktury.AddRange(connection.Query<ElementFaktury>(query, p));
    }

    Pojazd? pojazd = pojazdy.FirstOrDefault(p => p.SqlId is not null && p.SqlId == naprawa.SqlIdPojazdu);
    if(pojazd != null)
    {
        naprawa.Id = ObjectId.GenerateNewId().ToString();
        naprawa.IdPojazdu = pojazd.Id;
        pojazd.Naprawy.Add(naprawa.Id);
        await pojazdyCollection.ReplaceOneAsync(p => p.Id.Equals(pojazd.Id), pojazd);

        if(pojazd.IdKlienta != null)
        {
            var klient = klienci.FirstOrDefault(k => !string.IsNullOrEmpty(k.Id) && k.Id.Equals(pojazd.IdKlienta));
            if(klient != null)
            {
                naprawa.IdKlienta = pojazd.IdKlienta;
                klient.Naprawy.Add(naprawa.Id);
                await klienciCollection.ReplaceOneAsync(k => k.Id.Equals(klient.Id), klient);
            }
        }
    }

    if(naprawa.SqlId != null)
    {
        var p = new
        {
            Param = naprawa.SqlId
        };

        query =
        $"""
         select id_pracownika from naprawy as n
         join czlonek_zespolu as c
         on c.id_naprawy = n.id
         where id_naprawy = @Param
         """;

        List<int> idPracownikowSql = new(connection.Query<int>(query, p));
        foreach (var id in idPracownikowSql)
        {
            var pracownik = pracownicy.FirstOrDefault(p => p.SqlId == id);
            if(pracownik != null)
            {
                naprawa.Pracownicy.Add(pracownik.Id);
                if(string.IsNullOrEmpty(naprawa.Id))
                    naprawa.Id = ObjectId.GenerateNewId().ToString();

                pracownik.Naprawy.Add(naprawa.Id);
                await pracownicyCollection.ReplaceOneAsync(p => p.Id.Equals(pracownik.Id), pracownik);
            }
        }
    }
}

await naprawyCollection.InsertManyAsync(naprawy);

query =
   $"""
    select c.id as {nameof(ElementMagazynu.SqlId)}, c.nazwa as {nameof(ElementMagazynu.Nazwa)}, c.opis as {nameof(ElementMagazynu.Opis)}, 
    c.liczba_sztuk as {nameof(ElementMagazynu.Liczba)}, c.masa_g as {nameof(ElementMagazynu.MasaG)}, 
    p.nazwa  as {nameof(ElementMagazynu.NazwaProducenta)}, p.kod  as {nameof(ElementMagazynu.KodProducenta)}, 
    p.kraj_pochodzenia  as {nameof(ElementMagazynu.KrajPochodzenia)} 
    from czesci as c
    join producenci as p
    on p.id = c.id_producenta
    """;

magazyn.AddRange(connection.Query<ElementMagazynu>(query));
await magazynCollection.InsertManyAsync(magazyn);

query =
   $"""
    select z.id as {nameof(Zamowienie.SqlId)}, z.data_zlozenia as {nameof(Zamowienie.DataZlozenia)}, sz.nazwa as {nameof(Zamowienie.Status)},
    sz.opis as {nameof(Zamowienie.Opis)}
    from zamowienia as z
    join status_zamowienia as sz
    on z.id_status_zamowienia = sz.id
    """;

zamowienia.AddRange(connection.Query<Zamowienie>(query));
foreach (var zamowienie in zamowienia)
{
    if(zamowienie.SqlId is not null)
    {
        var p = new
        {
            Param = zamowienie.SqlId
        };

        query =
        $"""
         select e.id_czesci from elementy_zamowienia as e
         join zamowienia as z
         on e.id_zamowienia = z.id
         where e.id_zamowienia = @Param
         """;

        List<int> idElementowSql = new(connection.Query<int>(query, p));
        foreach (var item in idElementowSql)
        {
            var em = magazyn.FirstOrDefault(m => m.SqlId == item);
            if(em != null)
            {
                zamowienie.ElementyZamowienia.Add(em.Id);
            }
        }
    }
}
await zamowieniaCollection.InsertManyAsync(zamowienia);

Console.WriteLine("\nEnd");
Console.ReadLine();