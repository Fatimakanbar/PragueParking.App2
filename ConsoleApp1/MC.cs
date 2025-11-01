// Project: PragueParking.Data
// Fil: MC.cs
using System;
using System.Reflection.Metadata;

namespace PragueParking.Data
{
    // MC (motorcykel)
    public class MC : Vehicle
    {
        public MC() : base() { TypeName = "MC"; }
        public MC(string license) : base(license, VehicleSize.Small, "MC") { }
    }
}
