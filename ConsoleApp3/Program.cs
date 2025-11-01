

using PragueParking.Data;
using Spectre.Console;

// Filvägar (enkla - samma katalog som exe)
string configFile = "config.json";
string priceFile = "price.txt";
string stateFile = "garage_state.json";

ConfigModel config = new ConfigModel();
PriceModel prices = new PriceModel();
ParkingGarage garage = new ParkingGarage();

Console.Title = "Prague Parking 2.0";

// Läs in filer
LoadConfigAndData();

// Huvudloop
while (true)
{
    AnsiConsole.Clear();
    AnsiConsole.MarkupLine("[bold yellow]Prague Parking 2.0[/]");
    AnsiConsole.MarkupLine($"Platser: {garage.TotalSpots}  Upptagna: {garage.OccupiedCount}  Lediga: {garage.FreeCount}");
    AnsiConsole.WriteLine();

    var choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("Välj åtgärd:")
            .AddChoices(new[] {
                        "Visa karta",
                        "Parkera fordon",
                        "Släpp fordon (akta avgift)",
                        "Läs om prislista från fil",
                        "Spara state",
                        "Avsluta (spara)"
            }));

    if (choice == "Visa karta") ShowMap();
    else if (choice == "Parkera fordon") ParkVehicleFlow();
    else if (choice == "Släpp fordon (akta avgift)") ReleaseVehicleFlow();
    else if (choice == "Läs om prislista från fil") { prices = MinaFiler.ReadPriceFile(priceFile); AnsiConsole.MarkupLine("[green]Prislista inläst.[/]"); AnsiConsole.WriteLine("Tryck valfri tangent..."); Console.ReadKey(); }
    else if (choice == "Spara state") { MinaFiler.SaveGarageState(stateFile, garage); AnsiConsole.MarkupLine("[green]State sparad.[/]"); Console.ReadKey(); }
    else if (choice == "Avsluta (spara)") { MinaFiler.SaveGarageState(stateFile, garage); AnsiConsole.MarkupLine("[green]Sparat och avslutar.[/]"); break; }
}

void LoadConfigAndData()
{
    // Läs konfig
    config = MinaFiler.ReadConfig(configFile);

    // Läs prislista
    prices = MinaFiler.ReadPriceFile(priceFile);

    // Läs state om finns annars skapa nytt garage enligt config
    var maybeGarage = MinaFiler.ReadGarageState(stateFile);
    if (maybeGarage != null)
    {
        garage = maybeGarage;
    }
    else
    {
        garage = new ParkingGarage(config.TotalSpots, config.SmallSpots, config.MediumSpots, config.LargeSpots);
    }
}

void ShowMap()
{
    // Visa enkel karta i rader om 10 per rad
    int perRow = 10;
    var table = new Table();
    for (int i = 0; i < perRow; i++) table.AddColumn("");
    int col = 0;
    var rowCells = new string[perRow];
    for (int i = 0; i < garage.Spots.Count; i++)
    {
        var s = garage.Spots[i];
        string disp;
        if (s.IsOccupied)
        {
            disp = $"[red]{s.Id:000}X[/]"; // X = upptagen
        }
        else
        {
            disp = $"[green]{s.Id:000}.[/]"; // . = ledig
        }
        rowCells[col] = disp;
        col++;
        if (col == perRow)
        {
            table.AddRow(rowCells);
            rowCells = new string[perRow];
            col = 0;
        }
    }
    // Om sista raden inte full
    if (col != 0)
    {
        for (int j = col; j < perRow; j++) rowCells[j] = "";
        table.AddRow(rowCells);
    }
    AnsiConsole.Write(table);

    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine("[green]. = ledig  [red]X[/] = upptagen[/]");
    AnsiConsole.WriteLine("Tryck valfri tangent för att gå tillbaka...");
    Console.ReadKey();
}

void ParkVehicleFlow()
{
    // Välj typ
    var typ = AnsiConsole.Prompt(new SelectionPrompt<string>()
        .Title("Välj fordonstyp")
        .AddChoices(config.VehicleTypes));

    var license = AnsiConsole.Ask<string>("Ange registreringsnummer:");

    Vehicle v;
    if (typ == "Car") v = new Car(license);
    else if (typ == "MC") v = new MC(license);
    else v = new Vehicle(license, VehicleSize.Medium, typ); // fallback

    var spot = garage.FindSpotFor(v);
    if (spot == null)
    {
        AnsiConsole.MarkupLine("[red]Ingen lämplig plats ledig för detta fordon.[/]");
        Console.ReadKey();
        return;
    }

    // Parkera
    garage.ParkVehicleInSpot(spot.Id, v);
    AnsiConsole.MarkupLine($"[green]Parkerat i ruta {spot.Id} kl {v.EntryTime}[/]");
    Console.ReadKey();
}

void ReleaseVehicleFlow()
{
    var license = AnsiConsole.Ask<string>("Ange registreringsnummer som lämnar:");
    var v = garage.ReleaseVehicle(license);
    if (v == null)
    {
        AnsiConsole.MarkupLine("[red]Hittade inget fordon med det registreringsnumret.[/]");
        Console.ReadKey();
        return;
    }

    // Räkna avgift
    var duration = v.ParkedDuration;
    decimal amount = CalculateFee(v, duration);
    AnsiConsole.MarkupLine($"Fordon {v.LicensePlate} parkerade i {duration.TotalMinutes:F0} minuter. Avgift: {amount} CZK");
    Console.ReadKey();
}

decimal CalculateFee(Vehicle v, TimeSpan duration)
{
    // gratisperiod
    if (duration.TotalMinutes <= prices.FreeMinutes) return 0m;
    // räkna antal påbörjade timmar efter gratisperiod
    var minutesCharged = duration.TotalMinutes - prices.FreeMinutes;
    var hours = (int)Math.Ceiling(minutesCharged / 60.0);
    // hämta pris per timme
    if (!prices.HourlyPrices.TryGetValue(v.TypeName, out decimal pricePerHour))
    {
        // fallback: Car-price
        if (!prices.HourlyPrices.TryGetValue("Car", out pricePerHour)) pricePerHour = 20m;
    }
    return pricePerHour * hours;
}
