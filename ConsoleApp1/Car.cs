// Project: PragueParking.Data
// Fil: Car.cs
using System;
using System.Reflection.Metadata;

namespace PragueParking.Data
{
    // Bil är en Vehicle
    public class Car : Vehicle
    {
        public Car() : base() { TypeName = "Car"; }
        public Car(string license) : base(license, VehicleSize.Medium, "Car") { }
    }
}
