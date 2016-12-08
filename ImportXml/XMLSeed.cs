using CarDealers.Data;
using CarDealers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ImportXml
{
    public class XMLSeed
    {
       
        private const string SuppliersPath = "../../../datasets/suppliers.xml";
        private const string PartsPath = "..//..//..//datasets//parts.xml";
        private const string CarsPath = "..//..//..//datasets//cars.xml";
        private const string CustomersPath = "..//..//..//datasets//customers.xml";

        static void Main()
        {


            CarDealersContext context = new CarDealersContext();
            context.Database.Initialize(true);
            context.SaveChanges();

         //   ImportSuppliers(context);
           // ImportParts(context);
           // ImportCars(context);
            ImportCustomers(context);
            ImportSalesRecords(context);

        }

        private static void ImportCustomers(CarDealersContext context)
        {
            var xml = XDocument.Load(CustomersPath);
            var customers = xml.XPathSelectElements("customers/customer");
            foreach (var cust in customers)
            {
                ImportCustomer(cust, context);
            }
            context.SaveChanges();
        }

        private static void ImportCustomer(XElement customerNode, CarDealersContext context)
        {
            var name = customerNode.Attribute("name");
            var birthDate =(DateTime) customerNode.Element("birth-date");
            var isYoungDriver = (bool)customerNode.Element("is-young-driver");
            Customer customer = new Customer
            {
                Name = name.Value,
                DateOfBirth= birthDate,
                isYoungDriver = isYoungDriver
            };
            context.Customers.Add(customer);
        }

        private static void ImportSalesRecords(CarDealersContext context)
        {
            Random rnd = new Random();
            List<decimal> discounts = new List<decimal> { 0, 0.05m, 0.1m, 0.15m, 0.2m, 0.3m, 0.4m, 0.5m };
            foreach (var car in context.Cars)
            {
                int randomCustomerId = rnd.Next(1, context.Customers.Count() + 1);
                var sale = new Sale
                {
                    Customer = context.Customers.FirstOrDefault(cust => cust.Id == randomCustomerId),
                    Discount = discounts[rnd.Next(0, discounts.Count)],
                    Car = car
                };
                context.Sales.Add(sale);
            }
            context.SaveChanges();
        }

        private static void ImportParts(CarDealersContext context)
        {
            var xml = XDocument.Load(PartsPath);
            var parts = xml.XPathSelectElements("parts/part");
            var rand = new Random();
            foreach (var part in parts)
            {
                ImportParts(part, context, rand);
            }
            context.SaveChanges();
        }

        private static void ImportParts(XElement partNode, CarDealersContext context, Random rand)
        {
            var name = partNode.Attribute("name");
            var price = (decimal)partNode.Attribute("price");
            var quantity = (int)partNode.Attribute("quantity");

            int supplierId = rand.Next(1, context.Suppliers.Count() + 1);

            Part part = new Part
            {
                Name = name.Value,
                Price = price,
                Quantity = quantity,
                Supplier = context.Suppliers.FirstOrDefault(sup => sup.Id == supplierId)
            };
            context.Parts.Add(part);
        }

        private static void ImportSuppliers(CarDealersContext context)
        {
           XDocument xml =  XDocument.Load(SuppliersPath);
            var suppliers = xml.XPathSelectElements("suppliers/supplier");
            foreach (var sup in suppliers)
            {
                ImportSupplier(sup, context);
            }
            context.SaveChanges();
        }

        private static void ImportSupplier(XElement supplierNode, CarDealersContext context)
        {
            var name = supplierNode.Attribute("name");
            var is_importer = supplierNode.Attribute("is-importer");
            var supplier = new Supplier
            {
                Name = name.Value,
                IsImporter = (bool)is_importer
            };
            context.Suppliers.Add(supplier);
        }

        public static void ImportCars(CarDealersContext context)
        {
            var xmlDocument = XDocument.Load(CarsPath);
            var cars = xmlDocument.XPathSelectElements("cars/car");
           var rand=new Random();
            foreach (var car in cars)
            {
                ImportCar(car, context,rand);
            }
            context.SaveChanges();
        }
        public static void ImportCar(XElement carNode, CarDealersContext context, Random rand)
        {
            var make = carNode.Element("make");
            var model = carNode.Element("model");
            var travelled_distance = (long)carNode.Element("travelled-distance");
            var parts = new List<Part>();
            int numberOfPartsToAdd = rand.Next(10, 21);
            for (int i = 0; i < numberOfPartsToAdd; i++)
            {
                int ranPartId = rand.Next(1, context.Parts.Count() + 1);
                parts.Add(context.Parts.FirstOrDefault(p => p.Id == ranPartId));
            }
            var car = new Car
            {
                Make = make.Value,
                Model = model.Value,
                TravelledDistance = travelled_distance,
                    Parts = parts
            };
            context.Cars.Add(car);

        }
    }
}
