# Tjugoett

Ett konsolspel i C# för en till fem spelare mot en givare. Spelarna väljer att köpa kort eller stanna. Programmet visar korten som grafik i konsolen och sparar antal vinster, förluster och lika resultat per spelarnamn.

## Starta programmet

Installera .NET SDK 10 och kör följande i projektmappen:

```sh
dotnet run
```

Ange antalet spelare och deras namn. Under en spelares tur anger du `k` för att köpa eller `s` för att stanna. Svara `j` eller `n` på frågan om att spela en runda till. Programmet använder Unicode för kortens ramar och färger; resultatet ser bäst ut i en terminal som stöder Unicode och färg.

Resultaten sparas i `spelare.json` bredvid programmets körbara filer. Samma namn, oavsett stora eller små bokstäver, återanvänder tidigare resultat. Om filen inte går att läsa avbryts starten utan att den skrivs över. Om resultat inte kan sparas visas ett felmeddelande.

## Regler

- Målet är att komma närmare 21 än givaren utan att gå över 21. Ett ess räknas som 1 eller 11 beroende på handen.
- En naturlig tjugoett är ess och ett tiokort på de två första korten. Den slår en tjugoett med fler kort. Två naturliga är lika.
- Givaren har ett dolt kort under spelarnas turer, köper på 16 eller lägre och stannar på 17 eller högre, även mjuk 17. Rundans givare spelar även om alla spelare har spräckt.
- En lek med 52 kort blandas med Fisher–Yates. Före en runda skapas en ny blandad lek om färre än 15 kort återstår.
- Inga marker eller insatser används; resultatet räknas som vinst, förlust eller lika.

## Struktur

Se [klassdiagrammet](klassdiagram.svg). `Program` startar `ConsoleUi`, som sköter in- och utmatning. `Game` hanterar giv och jämförelse av händer och använder `Deck`, `Player` och `Hand`. `Hand` räknar värdet från sina `Card`-objekt. `GameStorage` läser och skriver spelarstatistik. `CardRenderer` bygger kortens bildrader, medan `ConsoleUi` skriver ut dem och sätter färgerna.

### Kortgrafik

Kortgrafiken motsvarar tillval 1 i uppgiftsbeskrivningen. `CardRenderer` returnerar en radlista för varje kort utan att själv använda konsolen. `ConsoleUi` skriver raderna sida vid sida och visar baksidan på givarens dolda kort.

## Arbete och kända begränsningar

Projektet har främst sparats lokalt på datorn. Filerna samlades och laddades upp till GitHub i ett sammanhållet tillfälle nära inlämningen, vilket förklarar att många commits ligger tätt i tiden. De visar uppladdningen och efterföljande förtydliganden, inte hela utvecklingshistoriken.

Arbetet har enligt uppgift genomförts individuellt med stöd av Claude och Codex. Uppgiftsbeskrivningen gäller ett grupparbete och innehåller krav på granskade pull requests; hur dessa krav tillämpas vid individuell omtenta behöver bekräftas med läraren.

Om kortleken skulle ta slut under en ovanligt lång runda skapas en ny blandad lek och programmet meddelar det. Kort från den pågående rundan kan då förekomma igen i den nya leken. Kortfärgernas visning beror på terminalens stöd för färg och Unicode.
