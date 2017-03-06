using System;
using System.Collections.Generic;
using System.Linq;
using LinqToDB;
using LinqToDB.Mapping;

namespace Linq2dbDemo
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            using (var db = new DataModel.TestDataDB())
            {
                var q =
                    from c in db.People
                    select c;

                foreach (var c in q)
                    Console.WriteLine(c.FirstName);
            }

            foreach (var product in All())
            {
                Console.WriteLine($"{product.ProductID}: {product.Name}");
            }

        }

        public static List<Product> All()
        {
            using (var db = new DbNorthwind())
            {
                var query = from p in db.Product
                    where p.ProductID > 25
                    orderby p.Name descending
                    select p;
                return query.ToList();
            }
        }

        public static List<Product> All1()
        {
            using (var db = new DbNorthwind())
            {
                return db.Product.Where(p => p.ProductID > 25).OrderByDescending(p => p.Name).Select(p => p).ToList();
            }
        }
    }


    [Table(Name = "Products")]
    public class Product
    {
        [PrimaryKey, Identity]
        public int ProductID { get; set; }

        [Column(Name = "ProductName"), NotNull]
        public string Name { get; set; }

        // ... other columns ...
    }

    [Table(Name = "Categories")]
    public class Category
    {
        [PrimaryKey, Identity]
        public int CategoryID { get; set; }

        [Column(Name = "CategoryName"), NotNull]
        public string Name { get; set; }

        // ... other columns ...
    }

    public class DbNorthwind : LinqToDB.Data.DataConnection
    {
        public DbNorthwind() : base("Northwind") { }

        public ITable<Product> Product => GetTable<Product>();
        public ITable<Category> Category => GetTable<Category>();

        // ... other tables ...
    }

}