// Project: PragueParking.Data
// Fil: Vehicle.cs
using System;
using System.Text.Json.Serialization;

namespace PragueParking.Data
{
    // Enumerera enkla storlekar
    public enum VehicleSize
    {
        Small,  // t.ex. MC, cyklar
        Medium, // t.ex. Bil
        Large   // t.ex. Buss
    }

    // Bas-klass för fordon
    public class Vehicle
    {
        public string LicensePlate { get; set; } = "";
        public DateTime? EntryTime { get; set; } // tidpunkt då fordonet parkerade
        public VehicleSize Size { get; set; }
        public string TypeName { get; set; } = "Vehicle";

        // Tom konstruktör för JSON-serialisering
        public Vehicle() { }

        public Vehicle(string license, VehicleSize size, string typeName)
        {
            LicensePlate = license;
            Size = size;
            TypeName = typeName;
            EntryTime = null;
        }

        // Räkna timmar/ minuter parkerade
        [JsonIgnore]
        public TimeSpan ParkedDuration
        {
            get
            {
                if (!EntryTime.HasValue) return TimeSpan.Zero;
                return DateTime.Now - EntryTime.Value;
            }
        }
    }
}
