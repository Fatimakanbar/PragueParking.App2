using System.Collections.Generic;

namespace PragueParking.Data
{
    public class PriceModel
    {
        // Gratisperiod i minuter
        public int FreeMinutes { get; set; } = 10;

        // Pris per timme per fordonstyp
        public Dictionary<string, decimal> HourlyPrices { get; set; } = new Dictionary<string, decimal>
        {
            { "Car", 20m },
            { "MC", 10m },
            { "Bike", 5m },
            { "Bus", 80m }
        };
    }
}
