using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars
{
    class Program
    {
        static void Main(string[] args)
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<CarDb>());
            InsertData();
            QueryData();

            Console.Write("Press any keep to continue");
            Console.ReadKey(); 
        }

        private static void InsertData()
        {
            var cars = ProcessCars("fuel.csv");
            var db = new CarDb();
            db.Database.Log = Console.WriteLine;

            if (!db.Cars.Any())
            {
                foreach (var car in cars)
                {
                    db.Cars.Add(car);
                }
                db.SaveChanges();
            }
        }

        private static void QueryData()
        {
            var db = new CarDb();

            var query =
                db.Cars.OrderByDescending(c => c.Combined).ThenBy(c => c.Name).Take(10);

            foreach (var car in query)
            {
                Console.WriteLine($"{car.Name}: {car.Combined}");
            }
        }



        private static List<Car> ProcessCars(string path)
        {
            var query =
             File.ReadAllLines(path)
                .Skip(1)
                .Where(line => line.Length > 1)
                .ToCar();

            return query.ToList();
        }

        private static List<Manufacturer> ProcessManufacturers(string path)
        {
            var query =

                File.ReadAllLines(path)
                    .Skip(1)
                    .Where(c => c.Length > 1)
                    .Select(r =>
                    {
                        var columns = r.Split(',');

                        return new Manufacturer
                        {
                            Name = columns[0],
                            Headquarters = columns[1],
                            Year = int.Parse(columns[2])
                        };
                    });

            return query.ToList();
        }
    }

    public static class CarExtensions
    {
        public static IEnumerable<Car> ToCar(this IEnumerable<string> source)
        {
            foreach (var line in source)
            {
                var column = line.Split(',');

                yield return new Car
                {
                    Year = int.Parse(column[0]),
                    Manufacturer = column[1],
                    Name = column[2],
                    Displacement = double.Parse(column[3]),
                    Cylinders = int.Parse(column[4]),
                    City = int.Parse(column[5]),
                    Highway = int.Parse(column[6]),
                    Combined = int.Parse(column[7])
                };
            };

        }
    }
}
