### Projektovanje softvera, 2025/2026

#### Fakultet tehničkih nauka, Univerzitet u Novom Sadu

# Projekat za niže ocene

## Osnovna ideja

Projektni zadatak je namenjen studentima koji žele da rade sažetiji projekat za ocene 6 i 7.  
Projekat se radi **individualno**.

### Obavezno je dodati asistentski nalog psw-ftn na GitLab repozitorijum gde se

**smešta izvorni kod, kao i na Trello board (psw_ftn).**

## Zahtevi

1. Web aplikacija implementirana upotrebom .NET Core radnog okvira koja omogućuje  
    korisnicima rad sa sistemom. Za front-end deo aplikacije koristiti Angular. Za mape  
    koristiti Leaflet biblioteku.
2. Implementirano rešenje treba da poštuje _Clean code_ principe.
3. Potrebno je implementirati rešenje po principu _Scrum_ metodologije, koja se izučava  
    na predmetu.
4. Kako se kreira realan sistem, potrebno je podatke trajno perzistirati u relacionoj bazi  
    podataka (npr. _MySQL_ ili _PostgreSQL_ ).
5. Kompletan dizajn aplikacije mora poštovati DDD princip koji se obrađuje na  
    predmetu.  
    Ukoliko bilo koja od prethodnih stavki nije zadovoljena smatra se da student nije položio  
    projekat, gde se podrazumeva da će student za obe ocene implementirati sve zahteve date  
    u nastavku specifikacije projekta. **Za ocenu 7 je potrebno prilikom implementacije  
    rešenja poštovati** **_TDD_** **princip (potrebno je prvo pisati testove, pa zatim kod, kako  
    sugeriše TDD princip).**

## Specifikacija

Ova aplikacija je dostupna vodičima tura i turistima koji imaju pristup funkcionalnostima  
datim u nastavku. Takođe, ova aplikacija je dostupna i administratorima radi održavanja  
sistema.

### Registracija

Turisti se moraju registrovati kako bi imali pristup funkcionalnostima u okviru aplikacije.  
Prilikom registracije se zadaju korisničko ime, lozinka, ime, prezime, e-mail adresa i  
interesovanja. Interesovanja se biraju među ponuđenim opcijama (priroda, umetnost,  
sport, šoping i hrana). Turista treba da označi da li je zainteresovan da dobija preporuke na  
osnovu svojih interesovanja ili ne.  
Administratori i vodiči tura se ne registruju preko aplikacije, već se unose u bazu podataka.

### Prijava na sistem

Omogućiti korisnicima sistema prijavu na sistem i dalji rad unutar sistema. Autentifikovati  
korisnika prilikom prijave i definisati permisije unutar sistema.

### Kreiranje ture

Vodič može da kreira turu tako što navodi naziv ture, opis, težinu ture, kategoriju (bira se  
među ponuđenim opcijama za interesovanja koje su se nudile i turisti prilikom registracije),  
cenu i datum održavanja ture. Ovako kreirana tura je u stanju “draft” i nije vidljiva turistima.

### Kreiranje ključne tačke

Vodič navodi ključne tačke za turu tako što na mapi bira određenu lokaciju. Informacija o  
geografskoj širini i dužini se beleži, zajedno sa nazivom, opisom i slikom (npr. ključna tačka  
može biti neki muzej, park, spomenik...). Te informacije vodič unosi sam. Vodič može  
objaviti turu, čime se njeno stanje menja u “published” i takve ture su vidljive turistima.  
Published tura mora imati popunjene sve osnovne informacije i bar dve ključne tačke.  
Integrisati kreiranje ture i kreiranje ključnih tačaka za tu turu na korisničkom interfejsu.

### Pregled tura

Vodič ture može da pregleda sve svoje ture, pri čemu su vidljive sve informacije koje je  
uneo, kao i mapa na kojoj je tura iscrtana (iscrtane su ključne tačke i putanje među njima).  
Omogućiti sortiranje tura po datumu održavanja (opadajuće i rastuće).

### Traženje zamene i otkazivanje ture

Vodič može potražiti zamenu ukoliko nije u stanju da održi turu. Vodič može označiti turu  
za koju traži zamenu, a tura se pojavljuje na stranici kojoj samo vodiči imaju pristup. Drugi  
vodiči mogu odabrati turu za koju originalni vodič traži zamenu, ali samo ako već nemaju  
svoju turu na isti taj datum. Kada se za turu prijavi neki vodič koji je slobodan i mogao bi da  
menja originalnog vodiča, tura se uklanja sa te stranice. Ukoliko se 24h pred održavanje  
ture ne nađe zamena, tura se otkazuje. U tom slučaju se svim turistima koji su prethodno  
kupili tu turu daju bonus poeni (u visini cene ture) koji se mogu iskoristiti pri bilo kojoj  
narednoj kupovini.

### Kupovina ture

Turista može pregledati sve ture i odabrati jednu ili više tura za kupovinu. Prilikom pregleda  
tura, omogućiti turisti da pregleda sve informacije vezane za ture. Pored toga, omogućiti  
turisti prikaz tura na mapi, gde će se videti ključne tačke i putanje među njima. Odabrane  
ture se smeštaju u korpu, gde se sabiraju cene svih odabranih tura i prikazuje ukupna cena.  
Omogućiti uklanjanje tura iz korpe.  
Ukoliko turista ima bonus poene (dobijene u funkcionalnosti “Otkazivanje ture”), može ih  
iskoristiti da umanji cenu korpe. Cena se može umanjiti do 0, ali ne i u minus. Ako turisti  
ostane još neiskorišćenih bonus poena, može ih iskoristiti u nekoj budućoj kupovini. Turista  
potvrđuje kupovinu tura (klikom na dugme) i sistem mu šalje mejl uspešne kupovine. U  
mejlu navesti osnovne informacije o svakoj kupljenoj turi.

### Podsetnik za kupljene ture

Sistem šalje turisti podsetnik za kupljenu turu dva dana (48h) pred datum održavanja ture.  
Podsetnik se šalje putem mejla, gde se navode sve informacije o turi za koju se šalje  
podsetnik.

### Ocenjivanje kupljenih tura

Turista može oceniti kupljenu turu, nakon što prođe datum održavanja ture. Ocena se bira  
od 1 do 5, a pored toga se može ostaviti komentar. Ako korisnik da ocenu 1 ili 2, ostavljanje  
komentara je neophodno, dok za ocene 3, 4 i 5 nije obavezno. Korisnik ima mogućnost da  
ostavi komentar u roku od 7 dana nakon održavanja ture, nakon čega mu sistem više ne  
sme dozvoliti ocenjivanje.

### Preporuka tura

Sistem šalje turistima mejl svaki put kada neki vodič kreira novu turu čija se kategorija  
podudara sa interesovanjima koje je turista naveo prilikom registracije. Turista može  
promeniti odabrana interesovanja bilo kada na svom profilu, a može i isključiti opciju za  
preporuke.

### Prijava problema na turi

Turista može prijaviti problem na turama koje je prethodno kupio. Problem treba da sadrži  
naziv i opis. Nakon prijave problema za određenu turu, problem ima status “na čekanju”.  
Vodič ture na kojoj je problem prijavljen dobija obaveštenje o novom problemu. Prijavljeni  
problem prelazi u stanje “rešen” nakon što vodič reši problem i označi ga kao rešenog.  
Ukoliko vodič smatra da je prijavljeni problem nevalidan, on šalje taj problem  
administratoru koji treba da revidira problem. U tom slučaju problem prelazi u stanje “na  
reviziji”. Administrator procenjuje problem i vraća ga vodiču ukoliko smatra da je problem  
validan i da vodič treba da ga reši (prelazi se opet u stanje “na čekanju”) ili odbacuje  
problem ukoliko je nevalidan pri čemu problem dobija status “odbačen”. Turista i vodič  
pored prijavljenog problema mogu videti status u bilo kom trenutku.  
Kada se otvori novi problem, pratiti sve dalje promene stanja tog problema (svaka promena  
stanja proizvodi događaj tj. event, čime se pokriva deo gradiva DDD event sourcing).

### Administracija sistema

Sistem identifikuje turiste i vodiče kao zlonamerne ukoliko 5 puta pogreše lozinku ili  
korisničko ime prilikom prijave na sistem. Ovakve korisnike sistem privremeno blokira, te  
im je onemogućeno da se prijave na sistem.  
Administrator sistema može videti listu privremeno blokiranih korisnika, gde za svakog  
korisnika piše koji put je blokiran.

Administrator ima mogućnost da odblokira privremeno blokirane korisnike, nakon čega im  
je omogućeno ponovno prijavljivanje. Ukoliko je neki korisnik blokiran 3. put, administrator  
nema mogućnost da odblokira korisnika.