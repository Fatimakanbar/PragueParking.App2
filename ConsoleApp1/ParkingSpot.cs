// Project: PragueParking.Data
// Fil: ParkingSpot.cs
using System;
using System.Text.Json.Serialization;

namespace PragueParking.Data
{
    // Representerar en parkeringsruta
    public class ParkingSpot
    {
        public int Id { get; set; } // index/id för rutan
        public VehicleSize Size { get; set; } // vilken storlek rutan är
        public Vehicle? OccupiedBy { get; set; } // null om ledig

        public ParkingSpot()
        {
            Id = 0;
            Size = VehicleSize.Medium;
            OccupiedBy = null;
        }

        public ParkingSpot(int id, VehicleSize size)
        {
            Id = id;
            Size = size;
            OccupiedBy = null;
        }

        // Hjälpmetod för att avgöra om ett fordon får stå här
        public bool CanFit(Vehicle v)
        {
            // Small fits in all, Medium fits Medium+Large, Large only in Large
            if (v.Size == VehicleSize.Small) return true;
            if (v.Size == VehicleSize.Medium) return Size == VehicleSize.Medium || Size == VehicleSize.Large;
            return Size == VehicleSize.Large;
        }

        [JsonIgnore]
        public bool IsOccupied => OccupiedBy != null;
    }
}
