// Project: PragueParking.Data
// Fil: ParkingGarage.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace PragueParking.Data
{
    // Representerar P-huset
    public class ParkingGarage
    {
        // Lista med rutor
        public List<ParkingSpot> Spots { get; set; } = new List<ParkingSpot>();

        public ParkingGarage() { }

        // Skapa garage med bestämd fördelning (enkelt)
        public ParkingGarage(int totalSpots, int smallCount = 10, int mediumCount = 80, int largeCount = 10)
        {
            // Säkerställ summa
            int sum = smallCount + mediumCount + largeCount;
            if (sum != totalSpots)
            {
                // Justera medium så att total blir rätt
                mediumCount = Math.Max(0, totalSpots - smallCount - largeCount);
            }

            int id = 1;
            for (int i = 0; i < smallCount; i++) Spots.Add(new ParkingSpot(id++, VehicleSize.Small));
            for (int i = 0; i < mediumCount; i++) Spots.Add(new ParkingSpot(id++, VehicleSize.Medium));
            for (int i = 0; i < largeCount; i++) Spots.Add(new ParkingSpot(id++, VehicleSize.Large));
        }

        // Hitta första lediga ruta som passar fordonet
        public ParkingSpot? FindSpotFor(Vehicle v)
        {
            return Spots.FirstOrDefault(s => !s.IsOccupied && s.CanFit(v));
        }

        // Parkera fordon i en specifik ruta (returnera true om OK)
        public bool ParkVehicleInSpot(int spotId, Vehicle v)
        {
            var spot = Spots.FirstOrDefault(s => s.Id == spotId);
            if (spot == null) return false;
            if (spot.IsOccupied) return false;
            if (!spot.CanFit(v)) return false;
            v.EntryTime = DateTime.Now;
            spot.OccupiedBy = v;
            return true;
        }

        // Parkera fordon i första lediga som passar
        public bool ParkVehicle(Vehicle v)
        {
            var spot = FindSpotFor(v);
            if (spot == null) return false;
            return ParkVehicleInSpot(spot.Id, v);
        }

        // Släpp fordon via registreringsnummer -> returnerar fordonet så kan vi räkna avgift
        public Vehicle? ReleaseVehicle(string license)
        {
            var spot = Spots.FirstOrDefault(s => s.IsOccupied && s.OccupiedBy != null && s.OccupiedBy.LicensePlate == license);
            if (spot == null) return null;
            var v = spot.OccupiedBy!;
            spot.OccupiedBy = null;
            return v;
        }

        // Enkel översiktsstatistik
        public int TotalSpots => Spots.Count;
        public int OccupiedCount => Spots.Count(s => s.IsOccupied);
        public int FreeCount => TotalSpots - OccupiedCount;
    }
}
