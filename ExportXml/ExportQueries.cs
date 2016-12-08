using CarDealers.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ExportXml
{
    class ExportQueries
    {

        static void Main(string[] args)
        {
            var context = new CarDealersContext();

            ExportOrderedCustomers(context);
            ExportToyotaCars(context);
            ExportLocalSuppliers(context);
            ExportCarsAndTheirParts(context);
            ExportTotalSalesByCustomer(context);
            ExportSalesWithAppliedDiscount(context);
        }

            //query 1
        private static void ExportOrderedCustomers(CarDealersContext context)
        {
            var customers = context.Customers
            .OrderBy(cust => cust.DateOfBirth)
            .ThenBy(cust => cust.isYoungDriver)
            .Select(cust => new
            {
                id = cust.Id,
                name = cust.Name,
                birth_date = cust.DateOfBirth,
                is_young_driver = cust.isYoungDriver
            });
            var xmlDocument =new XElement("customers");
            foreach (var cust in customers)
            {
                var cusomersNode = new XElement("customer");
                cusomersNode.Add(new XElement("id", cust.id));
                cusomersNode.Add(new XElement("name", cust.name));
                cusomersNode.Add(new XElement("birth-date", cust.birth_date));
                cusomersNode.Add(new XElement("is-youn-driver", cust.is_young_driver));
                xmlDocument.Add(cusomersNode);
            }
            xmlDocument.Save("..//..//..//ExportedFiles//ordered-customers.xml");
        }

        //query 2
        private static void ExportToyotaCars(CarDealersContext context)
        {
            var carsFromToyota = context.Cars
            .Where(car => car.Make == "Toyota")
            .OrderBy(car => car.Model)
            .ThenByDescending(car => car.TravelledDistance)
            .Select(car => new
            {
                id = car.Id,
                make = car.Make,
                model = car.Model,
                travelled_distance = car.TravelledDistance
            });
            var xmlDocument =new  XElement("cars");
            foreach (var car in carsFromToyota)
            {
                var carNode = new XElement("car");
                carNode.Add(new XAttribute("id", car.id));
                carNode.Add(new XAttribute("make", car.make));
                carNode.Add(new XAttribute("model", car.model));
                carNode.Add(new XAttribute("travelled-distance", car.travelled_distance));
                xmlDocument.Add(carNode);

            }

            xmlDocument.Save("..//..//..//ExportedFiles//toyota-cars.xml");
        }
        //query 3

        private static void ExportLocalSuppliers(CarDealersContext context)
        {
            var suppliers = context.Suppliers
            .Where(sup => sup.IsImporter == false)
            .Select(sup => new
            {
                id = sup.Id,
                name = sup.Name,
                parts_count = sup.Parts.Count
            });
            var xmlDocument =new XElement("suppliers");
            foreach (var sup in suppliers)
            {
                var supplierNode = new XElement("supplier");
                supplierNode.Add(new XAttribute("id", sup.id));
                supplierNode.Add(new XAttribute("name", sup.name));
                supplierNode.Add(new XAttribute("parts-count", sup.parts_count));
                xmlDocument.Add(supplierNode);

            }
            xmlDocument.Save("..//..//..//ExportedFiles//local-suppliers.xml");
        }
        //query 4
        private static void ExportCarsAndTheirParts(CarDealersContext context)
        {
            var carsWithParts = context.Cars
            .Select(car => new
            {
                make = car.Make,
                model = car.Model,
                travelled_distance = car.TravelledDistance,
                parts = car.Parts.Select(part => new
                {
                    name = part.Name,
                    price = part.Price
                })
            });
            var xmlDocument = new XElement("cars");
            foreach (var car in carsWithParts)
            {
                var carNode = new XElement("car");
                carNode.Add(new XAttribute("make", car.make));
                carNode.Add(new XAttribute("model", car.model));
                carNode.Add(new XAttribute("travelled-distance", car.travelled_distance));
                var partsNode = new XElement("parts");
                foreach (var part in car.parts)
                {
                    var partNode = new XElement("part");
                    partNode.Add(new XAttribute("name", part.name));
                    partNode.Add(new XAttribute("price", part.price));
                    partsNode.Add(partNode);

                }
                xmlDocument.Add(partsNode);
                xmlDocument.Add(carNode);
            }
            xmlDocument.Save("..//..//..//ExportedFiles//cars-and-parts.xml");

        }
        //query 5
        private static void ExportTotalSalesByCustomer(CarDealersContext context)
        {
            var customersWithCars = context.Customers
            .Where(cust => cust.Sales.Count > 0)
            .OrderByDescending(cust => cust.Sales.Sum(sale => sale.Car.Parts.Sum(part => part.Price)))
            .ThenByDescending(cust => cust.Sales.Count)
            .Select(cust => new
            {
                full_name = cust.Name,
                bought_cars = cust.Sales.Count,
                spent_money = cust.Sales.Sum(sale => sale.Car.Parts.Sum(part => part.Price))
            });
            var xmlDocument = new XElement("customers");
            foreach (var cust in customersWithCars)
            {
                var customerNode = new XElement("customer");
                customerNode.Add(new XAttribute("full-name", cust.full_name));
                customerNode.Add(new XAttribute("bought-cars", cust.bought_cars));
                customerNode.Add(new XAttribute("spent-money", cust.spent_money));
                xmlDocument.Add(customerNode);
            }
            xmlDocument.Save("..//..//..//ExportedFiles//customers-total-sales.xml");
        }

        //query 6
        private static void ExportSalesWithAppliedDiscount(CarDealersContext context)
        {
            var sales = context.Sales.Select(s => new
            {
                
                    make = s.Car.Make,
                    model = s.Car.Model,
                    travelledDistance = s.Car.TravelledDistance,
               
                customerName = s.Customer.Name,
                discount = s.Discount,
                price = s.Car.Parts.Sum(p => p.Price),
                priceWithDiscount = (s.Car.Parts.Sum(p => p.Price)) - (s.Car.Parts.Sum(p => p.Price) * s.Discount)
            });
            var xmlDocument = new XElement("sales");
            foreach (var sale in sales)
            {
                var saleNode = new XElement("sale");
                var carNode = new XElement("car");
                carNode.Add(new XAttribute("make", sale.make));
                carNode.Add(new XAttribute("model", sale.model));
                carNode.Add(new XAttribute("travelled-distance", sale.travelledDistance));
                saleNode.Add(carNode);
                saleNode.Add(new XElement("customer-name", sale.customerName));
                saleNode.Add(new XElement("discount", sale.discount));
                saleNode.Add(new XElement("price", sale.price));
                saleNode.Add(new XElement("price-with-discount", sale.priceWithDiscount));
                xmlDocument.Add(saleNode);
            }
            xmlDocument.Save("..//..//..//ExportedFiles//sales-discounts.xml");
        }
    }
}


