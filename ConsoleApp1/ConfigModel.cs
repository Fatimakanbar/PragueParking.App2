// Project: PragueParking.Data
// Fil: ConfigModel.cs
using System;
using System.Collections.Generic;

namespace PragueParking.Data
{
    // Modell för konfigurationsfil (JSON)
    public class ConfigModel
    {
        public int TotalSpots { get; set; } = 100;
        public int SmallSpots { get; set; } = 10;
        public int MediumSpots { get; set; } = 80;
        public int LargeSpots { get; set; } = 10;

        // Möjliga fordonstyper (kan utökas)
        public List<string> VehicleTypes { get; set; } = new List<string> { "Car", "MC" };

        // För framtida användning: maximum antal fordon per P-plats per typ
        public Dictionary<string, int> MaxVehiclesPerSpot { get; set; } = new Dictionary<string, int>
        {
            {"Car",1},
            {"MC",1}
        };
    }
}
