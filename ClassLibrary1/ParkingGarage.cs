using System;
using System.Collections.Generic;
using System.Linq;
namespace PragueParking.Data
{
    public class ParkingGarage
    {
        public List<ParkingSpot> Spots { get; set; } = new List<ParkingSpot>();

        // 🔐 Filväg för automatisk sparning
        private readonly string dataFilePath = "data.json";

        public ParkingGarage() { }

        public ParkingGarage(int totalSpots, int smallCount = 10, int mediumCount = 80, int largeCount = 10)
        {
            int sum = smallCount + mediumCount + largeCount;
            if (sum != totalSpots)
            {
                mediumCount = Math.Max(0, totalSpots - smallCount - largeCount);
            }

            int id = 1;
            for (int i = 0; i < smallCount; i++) Spots.Add(new ParkingSpot(id++, VehicleSize.Small));
            for (int i = 0; i < mediumCount; i++) Spots.Add(new ParkingSpot(id++, VehicleSize.Medium));
            for (int i = 0; i < largeCount; i++) Spots.Add(new ParkingSpot(id++, VehicleSize.Large));
        }

        public ParkingSpot? FindSpotFor(Vehicle v)
        {
            return Spots.FirstOrDefault(s => !s.IsOccupied && s.CanFit(v));
        }

        public bool ParkVehicleInSpot(int spotId, Vehicle v)
        {
            var spot = Spots.FirstOrDefault(s => s.Id == spotId);
            if (spot == null || spot.IsOccupied || !spot.CanFit(v)) return false;

            v.EntryTime = DateTime.Now;
            spot.OccupiedBy = v;
            SaveToFile(); // ✅ Spara direkt
            return true;
        }

        public bool ParkVehicle(Vehicle v)
        {
            var spot = FindSpotFor(v);
            if (spot == null) return false;
            return ParkVehicleInSpot(spot.Id, v);
        }

        public Vehicle? ReleaseVehicle(string license)
        {
            var spot = Spots.FirstOrDefault(s => s.IsOccupied && s.OccupiedBy?.LicensePlate == license);
            if (spot == null) return null;

            var v = spot.OccupiedBy!;
            spot.OccupiedBy = null;
            SaveToFile(); // ✅ Spara direkt
            return v;
        }

        public void SaveToFile()
        {
            var parkedVehicles = Spots
                .Where(s => s.IsOccupied && s.OccupiedBy != null)
                .Select(s => s.OccupiedBy)
                .ToList();

            FileHandler.SaveJson(dataFilePath, parkedVehicles);
        }

        public int TotalSpots => Spots.Count;
        public int OccupiedCount => Spots.Count(s => s.IsOccupied);
        public int FreeCount => TotalSpots - OccupiedCount;
    }
}
