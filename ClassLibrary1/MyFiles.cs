using System.Text.Json;
using System.IO;
using PragueParking.Data;

public static class MinaFiler
{
    // 🔹 Läs konfigurationsfilen
    public static ConfigModel ReadConfig(string path)
    {
        if (!File.Exists(path)) return new ConfigModel();
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<ConfigModel>(json)!;
    }

    // 🔹 Läs prislista från textfil
    public static PriceModel ReadPriceFile(string path)
    {
        var model = new PriceModel();
        if (!File.Exists(path)) return model;

        foreach (var line in File.ReadAllLines(path))
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;
            var parts = line.Split('=');
            if (parts.Length == 2 && decimal.TryParse(parts[1], out decimal price))
            {
                model.HourlyPrices[parts[0].Trim()] = price;
            }
        }

        return model;
    }

    // 🔹 Spara garage-state till JSON
    public static void SaveGarageState(string path, ParkingGarage garage)
    {
        var vehicles = garage.Spots
            .Where(s => s.IsOccupied && s.OccupiedBy != null)
            .Select(s => s.OccupiedBy)
            .ToList();

        var json = JsonSerializer.Serialize(vehicles, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }

    // 🔹 Läs garage-state från JSON
    public static ParkingGarage? ReadGarageState(string path)
    {
        if (!File.Exists(path)) return null;
        var json = File.ReadAllText(path);
        var vehicles = JsonSerializer.Deserialize<List<Vehicle>>(json);
        if (vehicles == null) return null;

        var garage = new ParkingGarage(); // Du kan justera detta om du vill läsa config först
        foreach (var v in vehicles)
        {
            garage.ParkVehicle(v);
        }
        return garage;
    }
}
