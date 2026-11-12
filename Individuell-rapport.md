# Individuell rapport – Tjugoett

## Projektet och mitt arbetssätt

Jag har arbetat individuellt med ett konsolspel i C# där en till fem spelare spelar tjugoett mot en givare. Jag samlade ihop koden och använde Claude som stöd för den ursprungliga Blackjack-versionen. Därefter har projektet granskats och bearbetats med stöd av Codex för att bättre motsvara uppgiftsbeskrivningen. Jag har alltså inte arbetat i en grupp.

Spelet delar ut två kort till varje spelare och två till givaren. Ett av givarens kort är dolt medan spelarna gör sina val. Varje spelare kan köpa kort eller stanna. Om en spelare får över 21 förlorar spelaren. När alla spelare är klara visas givarens dolda kort. Givaren köper kort på 16 eller lägre och stannar på 17 eller högre, även på mjuk 17. Därefter jämförs varje spelares hand med givarens.

Programmet sparar antalet vinster, förluster och oavgjorda resultat för varje spelarnamn. Statistiken läses in nästa gång programmet startar. Inmatning och meddelanden till spelaren är på svenska, medan klasser och metoder har engelska namn.

## Tillvalet: kortgrafik

Tillvalet är kortgrafik i konsolen. Korten visas som rutor med valören i två hörn och färgen i mitten. Givarens dolda kort visas med en baksida. Korten i en hand står bredvid varandra.

Klassen `CardRenderer` har ett tydligt ansvar: den bygger bilden av ett kort som sju textrader. Metoden `Render` returnerar en array med dessa rader. När kortet ska vara dolt returneras i stället raderna för en kortbaksida. Renderaren skriver inte själv något till konsolen.

`ConsoleUi` tar hand om själva utskriften. För att placera korten bredvid varandra hämtas först raderna för varje kort. Programmet skriver sedan den första raden för alla kort, därefter den andra raden för alla kort och så vidare. `ConsoleUi` ansvarar också för färgerna när terminalen stöder färgutskrift: hjärter och ruter visas i rött och klöver och spader i svart.

## Designval och alternativ

Ett viktigt designval är att skilja kortens form från utskriften. Om `CardRenderer` returnerar strängar kan `ConsoleUi` bestämma hur korten ska placeras och när färgerna ska användas. Renderaren blir också möjlig att kontrollera utan att den behöver skriva direkt till konsolen.

Ett alternativ vore att låta varje kort skriva ut sig självt direkt. Det skulle vara enkelt för ett enstaka kort, men om hela ett kort skrivs ut innan nästa börjar hamnar korten under varandra. Lösningen med rader kräver att `ConsoleUi` samordnar flera kort och lagrar rader tillfälligt, men gör placeringen sida vid sida möjlig.

En annan uppdelning gäller spelets regler och kommunikationen med användaren. `Game` hanterar utdelning och jämförelse av händer, medan `ConsoleUi` läser spelarens val och visar resultaten. `Hand` håller reda på korten i en hand och beräknar dess värde. Om inmatning och utskrift blandas in i regelklasserna blir de svårare att kontrollera utan att samtidigt använda konsolen. Den tydligare uppdelningen innebär fler klasser, men varje klass får ett mer avgränsat ansvar.

Ess visar varför handens värde behöver beräknas utifrån hela handen. Ett ess kan vara värt 11 eller 1. I programmet räknas ess först som 11 och värdet räknas sedan ned när summan annars blir högre än 21. Därför kan ett ess ändra värde när ett nytt kort läggs till. Till exempel kan två ess och en nia räknas som 21.

## Problem och förbättringar

Jag hade problem kring flera spelare och att få projektet att fungera i Visual Studio Code. I den tidigare versionen kunde bara en spelare spela, spelregler och konsolinmatning var blandade, och resultaten sparades inte mellan körningar. Vid bearbetningen med Codex lades stöd för en till fem spelare till. Dialogen flyttades till `ConsoleUi`, och `GameStorage` lades till för att läsa och skriva spelarstatistik i en JSON-fil. Om filen inte kan läsas avbryts starten med ett meddelande i stället för att den gamla filen skrivs över. Om sparandet misslyckas får användaren också ett meddelande.

För Visual Studio Code fick den tidigare versionen en projektkonfiguration för byggning och körning. Byggkommandot och programmet kontrollerades, men en faktisk körning med F5 i editorn verifierades inte. Det är därför ett moment som fortfarande behöver kontrolleras i den miljö där projektet ska användas.

Efter bearbetningen byggdes programmet och kontrollerades med bland annat fem spelare, felaktig inmatning, essräkning, givarens regel och återinläsning av resultat. Kontrollerna visar att de prövade fallen fungerade, men inte att alla möjliga spelsituationer är felfria. Färgvisningen har inte verifierats visuellt i en interaktiv terminal.

En känd begränsning är att leken kan ta slut under en ovanligt lång runda. Programmet skapar då en ny blandad lek och meddelar det, vilket kan göra att ett kort som redan ligger i en hand förekommer igen. Med mer tid skulle jag undersöka hur en sådan situation ska hanteras enligt lärarens förväntningar och lägga till ett test för den. Det är en avvikelse från den vanliga hanteringen som beskrivs i README-filen.

## Samarbete och reflektion

Jag har arbetat ensam och använt Claude och Codex som stöd. Jag har därför inte granskat någon gruppmedlems pull request och kan inte beskriva en sådan granskning som utförd. Den ursprungliga uppgiften beskriver grupparbete och granskade pull requests. Hur de kraven gäller för en individuell omtenta är inte verifierat.

Det här arbetet visar hur spelet kan delas upp i olika ansvar: kort och händer, spelregler, presentation och sparning. Den uppdelningen hjälper mig att följa vad som händer när en spelare väljer att köpa ett kort och hur resultatet sedan sparas. Jag behöver kunna förklara lösningen med egna ord, särskilt kortgrafiken, essräkningen och hanteringen av spelarnas resultat.
