// Project: PragueParking.Data
// Fil: MinaFiler.cs
using System;
using System.IO;
using System.Text.Json;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace PragueParking.Data
{
    // Enkel dataåtkomstklass - Läs/Skriv filer (konfig, prislista, state)
    public static class MinaFiler
    {
        // Läs konfigurationsfil (JSON)
        public static ConfigModel ReadConfig(string path)
        {
            if (!File.Exists(path))
            {
                // Skapa default config om fil saknas
                var defaultConfig = new ConfigModel();
                SaveConfig(path, defaultConfig);
                return defaultConfig;
            }
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<ConfigModel>(json) ?? new ConfigModel();
        }

        public static void SaveConfig(string path, ConfigModel cfg)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(cfg, options);
            File.WriteAllText(path, json);
        }

        // Läs prislista från textfil; rader "Car=20" eller kommentarer med #
        public static PriceModel ReadPriceFile(string path)
        {
            var pm = new PriceModel();
            if (!File.Exists(path))
            {
                // skapa default prisfil
                var defaultLines = new List<string>
                {
                    "# Prislista: format Typ=PrisPerTimme (CZK). Kommentarer börjar med #",
                    "Car=20",
                    "MC=10",
                    "FreeMinutes=10"
                };
                File.WriteAllLines(path, defaultLines);
            }

            var lines = File.ReadAllLines(path);
            foreach (var raw in lines)
            {
                var line = raw.Trim();
                if (string.IsNullOrEmpty(line)) continue;
                if (line.StartsWith("#")) continue;
                var parts = line.Split('=', 2);
                if (parts.Length != 2) continue;
                var key = parts[0].Trim();
                var val = parts[1].Trim();
                if (key.Equals("FreeMinutes", StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(val, out int fm)) pm.FreeMinutes = fm;
                }
                else
                {
                    if (decimal.TryParse(val, out decimal price))
                    {
                        pm.HourlyPrices[key] = price;
                    }
                }
            }
            return pm;
        }

        // Spara state (vilka fordon står i garaget) i JSON
        public static void SaveGarageState(string path, ParkingGarage garage)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            // Because ParkingSpot contains Vehicle with DateTime, default serializer works
            var json = JsonSerializer.Serialize(garage, options);
            File.WriteAllText(path, json);
        }

        // Läs state (om fil saknas returnera null)
        public static ParkingGarage? ReadGarageState(string path)
        {
            if (!File.Exists(path)) return null;
            var json = File.ReadAllText(path);
            var garage = JsonSerializer.Deserialize<ParkingGarage>(json);
            return garage;
        }
    }
}
