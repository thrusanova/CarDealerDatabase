using CarDealers.Data;
using CarDealers.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDEalers.Client
{
   public class Program
    {
        private const string carsPath = "../../../datasets/cars.json";
        private const string suppliersPath = "../../../datasets/suppliers.json";
        private const string partsPath = "../../../datasets/parts.json";
        private const string customersPath = "../../../datasets/customers.json";

        static void Main(string[] args)
        {
            ImportSuppliers();
            ImportsParts();
            ImportCars();
            ImportCustomers();
            ImportSales();
        }

        private static void ImportSales()
        {
            double[] discounts = new double[] { 0, 0.05, 0.10, 0.20, 0.30, 0.40, 0.50 };
            var context = new CarDealersContext();
            Random rnd = new Random();
            var cars = context.Cars.ToList();
            var customers = context.Customers.ToList();
            for (int i = 0; i < 50; i++)
            {
                var car = cars[rnd.Next(cars.Count)];
                var customer = customers[rnd.Next(customers.Count)];
                var discount = discounts[rnd.Next(discounts.Length)];
                if (customer.isYoungDriver)
                {
                    discount += 0.05;
                }

                Sale sale = new Sale()
                {
                    Car = car,
                    Customer = customer,
                    Discount = discount
                };

                context.Sales.Add(sale);
            }

            context.SaveChanges();
        }

        private static void ImportCustomers()
        {
            var context = new CarDealersContext();
            var json = File.ReadAllText(customersPath);
            var CustomerEntities = JsonConvert.DeserializeObject<ICollection<Customer>>(json);

            foreach (var customer in CustomerEntities)
            {
                context.Customers.Add(customer);
            }

            context.SaveChanges();
        }

        private static void ImportCars()
        {
            var context = new CarDealersContext();
            var json = File.ReadAllText(carsPath);
            var carEntity = JsonConvert.DeserializeObject<IEnumerable<Car>>(json);
            var rand = new Random();
            var parts = context.Parts.ToList();
            foreach (var car in carEntity)
            {
                var counts = rand.Next(10, 20);
                for (int i = 0; i < counts; i++)
                {
                    car.Parts.Add(parts[rand.Next(parts.Count)]);
                }
                context.Cars.Add(car);
            }
            context.SaveChanges();
        }

        private static void ImportsParts()
        {
            var context = new CarDealersContext();
            var json = File.ReadAllText(partsPath);
            var partEntity = JsonConvert.DeserializeObject<IEnumerable<Part>>(json);
            var rand = new Random();
            var suppliers = context.Suppliers.ToList();
            foreach (var part in partEntity)
            {
                part.Supplier = suppliers[rand.Next(suppliers.Count)];
                context.Parts.Add(part);
            }
            context.SaveChanges();
        }

        private static void ImportSuppliers()
        {
            var context = new CarDealersContext();
            var json = File.ReadAllText(suppliersPath);
            var SupplierEntity = JsonConvert.DeserializeObject<IEnumerable<Supplier>>(json);
            context.Suppliers.AddRange(SupplierEntity);
            context.SaveChanges();
        }
    }
}
