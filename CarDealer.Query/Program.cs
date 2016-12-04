using CarDealers.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealer.Query
{
    class Program
    {
        static void Main(string[] args)
        {
            var context = new CarDealersContext();
            ExportCustomer(context);
            ExportAllToyotaCars(context);
            ExportLocalSupplier(context);
            ExportCarsAndParts(context);
            ExportTotalSAles(context);
            ExportSAleWithDiscount(context);
        }

        private static void ExportSAleWithDiscount(CarDealersContext context)
        {
            var sales = context.Sales.Select(s => new
            {
                car = new { s.Car.Make, s.Car.Model, s.Car.TravelledDistance },
                customerName = s.Customer.Name,
                Discount = s.Discount,
                price = s.Car.Parts.Sum(p => p.Price),
                priceWithDiscount = (s.Car.Parts.Sum(p => p.Price)) - (s.Car.Parts.Sum(p => p.Price) * s.Discount)
            });
            var salesJson = JsonConvert.SerializeObject(sales, Formatting.Indented);
            File.WriteAllText("..//..//..//ExportedFiles//sales-discounts.json", salesJson);
        }

        private static void ExportTotalSAles(CarDealersContext context)
        {
            var cusomers = context.Customers.Where(c => c.Sales.Count != 0).
                Select(c => new
                {
                    fullName = c.Name,
                    boughtCars = c.Sales.Count,
                    spentMoney = c.Sales.Sum(s => s.Car.Parts.Sum(p => p.Price))
                }).OrderByDescending(c => c.spentMoney).
                ThenByDescending(c => c.boughtCars).ToList();
            var totalSalesJson = JsonConvert.SerializeObject(cusomers, Formatting.Indented);
            File.WriteAllText("..//..//..//ExportedFiles//customers-total-sales.json", totalSalesJson);
        }
        
        private static void ExportCarsAndParts(CarDealersContext context)
        {
            var cars = context.Cars.Select(c => new
            {
                car=new
                {
                    c.Make,
                    c.Model,
                    c.TravelledDistance
                },
                parts=c.Parts.Select(p=>new
                {
                    p.Name,
                    p.Price
                })
            });
            var carsAndPartsToJson = JsonConvert.SerializeObject(cars, Formatting.Indented);
            File.WriteAllText("..//..//..//ExportedFiles//cars-and-parts.json", carsAndPartsToJson);
        }

        private static void ExportLocalSupplier(CarDealersContext context)
        {
            var supplier = context.Suppliers.Where(s => s.IsImporter == false).
                Select(s => new
                {
                    Id = s.Id,
                    Name = s.Name,
                    partsCount = s.Parts.Count()
                });
            var supplierToJson = JsonConvert.SerializeObject(supplier, Formatting.Indented);
            File.WriteAllText("..//..//..//ExportedFiles//local-suppliers.json", supplierToJson);
        }

        private static void ExportAllToyotaCars(CarDealersContext context)
        {
            var toyotaCar = context.Cars.Where(c => c.Make == "Toyota").OrderBy(m => m.Model).
                ThenByDescending(t => t.TravelledDistance).
                Select(car => new
                {
                    id=car.Id,
                    Make=car.Make,
                    Model=car.Model,
                    TravelledDistance=car.TravelledDistance
                });
            var carsToJson = JsonConvert.SerializeObject(toyotaCar, Formatting.Indented);
            File.WriteAllText("..//..//..//ExportedFiles//toyota-cars.json", carsToJson);
        }

        private static void ExportCustomer(CarDealersContext context)
        {
            var customers = context.Customers.OrderBy(b => b.DateOfBirth).
                ThenByDescending(c => c.isYoungDriver);
            var customersASJson = JsonConvert.SerializeObject(customers, Formatting.Indented);
            File.WriteAllText("..//..//..//ExportedFiles//ordered-customers.json", customersASJson);
        }
    }
}
