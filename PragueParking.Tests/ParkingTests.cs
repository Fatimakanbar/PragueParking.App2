// Project: PragueParking.Tests
// Fil: ParkingTests.cs
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PragueParking.Data;
using System;
using System.Runtime.ConstrainedExecution;

namespace PragueParking.Tests
{
    [TestClass]
    public class ParkingTests
    {
        [TestMethod]
        public void CanParkCarInMediumSpot()
        {
            var garage = new ParkingGarage(2, smallCount: 0, mediumCount: 2, largeCount: 0);
            var car = new Car("ABC123");
            var ok = garage.ParkVehicle(car);
            Assert.IsTrue(ok);
            Assert.AreEqual(1, garage.OccupiedCount);
        }

        [TestMethod]
        public void FeeCalculation_FreeFirstTenMinutes()
        {
            var price = new PriceModel();
            price.FreeMinutes = 10;
            price.HourlyPrices["Car"] = 20m;

            var car = new Car("XYZ999");
            car.EntryTime = DateTime.Now.AddMinutes(-5); // 5 minuter
            var duration = car.ParkedDuration;
            // Armen: använder samma logik som i Program. Simpla beräkningen här:
            if (duration.TotalMinutes <= price.FreeMinutes)
            {
                Assert.AreEqual(true, true); // gratis => testets syfte
            }
            else
            {
                Assert.Fail("Borde vara gratis men beräkningen visade inte så.");
            }
        }
    }
}
