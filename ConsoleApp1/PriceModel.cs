// Project: PragueParking.Data
// Fil: PriceModel.cs
using System;
using System.Collections.Generic;

namespace PragueParking.Data
{
    // Prismodell - släng in enkel struktur
    public class PriceModel
    {
        // Pris per påbörjad timme i CZK för olika fordonstyper
        public Dictionary<string, decimal> HourlyPrices { get; set; } = new Dictionary<string, decimal>();

        // Gratis minuter i början
        public int FreeMinutes { get; set; } = 10;
    }
}
