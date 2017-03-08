using System;
using System.Linq;
using LinqToDB;
using LinqToDB.Mapping;
using LinqToDB.DataProvider.MySql;
using MySql.Data.MySqlClient;
using MySql.Data.Types;

namespace Linq2dbDemo
{
    internal class Program
    {
        public static string ConnectionString = "Server=localhost;DataBase=sakila;User=root";

        public static void Main(string[] args)
        {
            // Old way
            // Create the SQL connection.
            var connection = new MySqlConnection(ConnectionString);
            connection.Open();

            // Create the command.
            var command = new MySqlCommand("SELECT * FROM sakila.actor;", connection);

            // Execute the command
            var reader = command.ExecuteReader();

            // Print the results
            while (reader.Read())
            {
                Console.WriteLine($"{reader.GetInt32(0)}\t{reader.GetString(1)} {reader.GetString(2)}");
            }

            // Close the connection
            reader.Close();
            connection.Close();

            Console.WriteLine("--------------");
            Console.ReadKey();

            // New way
            using (var db = new DbSakila())
            {
                var query = from actor in db.Actor
                    orderby actor.ActorId
                    select actor;

                foreach (var actor in query)
                {
                    Console.WriteLine(actor.ToString());
                }

                Console.WriteLine("--------------");
                Console.ReadKey();

                // Some more examples
                // 1. What are the names of all the languages in the database (sorted alphabetically)?
                foreach (string language in db.Language.Select(ia => ia.Name).OrderBy(ia => ia))
                {
                    Console.WriteLine(language);
                }

                Console.WriteLine("--------------");
                Console.ReadKey();

                // 2. Return the full names (first and last) of actors with “SON” in their last name, ordered by their first
                // name.
                query = from actor in db.Actor
                    where actor.LastName.Contains("SON")
                    orderby actor.FirstName
                    select actor;

                foreach (var actor in query)
                {
                    Console.WriteLine(actor.ToString());
                }

                Console.WriteLine("--------------");
                Console.ReadKey();

                // 3. Find all the addresses where the second address is not empty (i.e., contains some text), and return
                //     these second addresses sorted.
                foreach (var address in db.Address.Where(ia => !string.IsNullOrEmpty(ia.Address2)).OrderBy(ia => ia.Address2))
                {
                    Console.WriteLine(address.Address2);
                }

                Console.WriteLine("--------------");
                Console.ReadKey();

                // 4. How many films involve a whale?
                var whale = (from film in db.Film
                    where film.Title.Contains("WHALE")
                    select film).ToList();

                Console.WriteLine($"There are {whale.Count} films involving a whale");

                Console.WriteLine("--------------");
                Console.ReadKey();

                // 5. Return the first and last names of actors who played in a film involving a whale,
                // along with the release year of the movie, sorted by the actors’ last names.
                var actorYearQuery =
                    from actor in db.Actor
                    from film in db.Film
                    from filmActor in db.FilmActor
                    where actor.ActorId == filmActor.ActorId &&
                          film.FilmId == filmActor.FilmId &&
                          film.Title.Contains("WHALE")
                    orderby actor.LastName
                    select new {actor.FirstName, actor.LastName, film.Title, film.ReleaseYear};

                foreach (var actorYear in actorYearQuery)
                {
                    Console.WriteLine($"Actor: {actorYear.FirstName} {actorYear.LastName}, Title: {actorYear.Title} Year: {actorYear.ReleaseYear}");
                }

                Console.WriteLine("--------------");

                // 6. Find all the film categories in which there are between 55 and 65 films. Return the names of these
                //     categories and the number of films per category, sorted by the number of films.
                // 7. In how many film categories is the average difference between the film replacement cost and the
                // rental rate larger than 17?
                //     8. Find the address district(s) name(s) such that the minimal postal code in the district(s) is maximal
                //     over all the districts. Make sure your query ignores empty postal codes and district names.
                // 9. Find the names (first and last) of all the actors and costumers whose first name is the same as the
                // first name of the actor with ID 8. Do not return the actor with ID 8 himself. Note that you cannot
                // use the name of the actor with ID 8 as a constant (only the ID). There is more than one way to solve
                // this question, but you need to provide only one solution.
                // 10. Give an interesting query of your own that is not already in the assignment. The query should
                //     involve an aggregation operation, and a nested SELECT. Give, along with the query, the English
                //     explanation and the answer.
            }
        }
    }

    public class DbSakila : LinqToDB.Data.DataConnection
    {
        public DbSakila() : base(new MySqlDataProvider(), Program.ConnectionString) { }

        public ITable<Actor> Actor => GetTable<Actor>();
        public ITable<Address> Address => GetTable<Address>();
        public ITable<Category> Category=> GetTable<Category>();
        public ITable<Film> Film => GetTable<Film>();
        public ITable<FilmActor> FilmActor => GetTable<FilmActor>();
        public ITable<FilmCategory> FilmCategory => GetTable<FilmCategory>();
        public ITable<Language> Language => GetTable<Language>();
    }

    [Table(Name = "actor")]
    public class Actor
    {
        [Column(Name = "actor_id"), PrimaryKey, Identity]
        public int ActorId { get; set; }

        [Column(Name = "first_name"), NotNull]
        public string FirstName { get; set; }

        [Column(Name = "last_name"), NotNull]
        public string LastName { get; set; }

        [Column(Name = "last_update"), NotNull]
        public MySqlDateTime LastUpdate { get; set; }

        public override string ToString()
        {
            return $"{ActorId}\t{FirstName} {LastName}";
        }
    }

    [Table(Name = "address")]
    public class Address
    {
        [Column(Name = "address_id"), PrimaryKey, Identity]
        public int AddressId { get; set; }

        [Column(Name = "address"), NotNull]
        public string Address1 { get; set; }

        [Column(Name = "address2"), NotNull]
        public string Address2 { get; set; }

        [Column(Name = "district"), NotNull]
        public string District { get; set; }

        [Column(Name = "city_id"), Identity]
        public int CityId { get; set; }

        [Column(Name = "postal_code"), NotNull]
        public string PostalCode { get; set; }

        [Column(Name = "phone"), NotNull]
        public string Phone { get; set; }

        [Column(Name = "location"), NotNull]
        public MySqlGeometry Location { get; set; }

        [Column(Name = "last_update"), NotNull]
        public MySqlDateTime LastUpdate { get; set; }

        public override string ToString()
        {
            return $"{AddressId}\t{Address1}, {Address2}, {District}";
        }
    }

    [Table(Name = "category")]
    public class Category
    {
        [Column(Name = "category_id"), PrimaryKey, Identity]
        public int CategoryId { get; set; }

        [Column(Name = "name"), NotNull]
        public string Name { get; set; }

        [Column(Name = "last_update"), NotNull]
        public MySqlDateTime LastUpdate { get; set; }

        public override string ToString()
        {
            return $"{CategoryId}\t{Name}";
        }
    }

    [Table(Name = "language")]
    public class Language
    {
        [Column(Name = "language_id"), PrimaryKey, Identity]
        public int LanguageId { get; set; }

        [Column(Name = "name"), NotNull]
        public string Name { get; set; }

        [Column(Name = "last_update"), NotNull]
        public MySqlDateTime LastUpdate { get; set; }

        public override string ToString()
        {
            return $"{LanguageId}\t{Name}";
        }
    }

    [Table(Name = "film")]
    public class Film
    {
        [Column(Name = "film_id"), PrimaryKey, Identity]
        public int FilmId { get; set; }

        [Column(Name = "title"), NotNull]
        public string Title { get; set; }

        [Column(Name = "language_id"), Identity]
        public string LanguageId { get; set; }

        [Column(Name = "release_year"), NotNull]
        public string ReleaseYear { get; set; }

        [Column(Name = "last_update"), NotNull]
        public MySqlDateTime LastUpdate { get; set; }

        public override string ToString()
        {
            return $"{FilmId}\t{Title}";
        }
    }

    [Table(Name = "film_actor")]
    public class FilmActor
    {
        [Column(Name = "actor_id"), Identity]
        public int ActorId { get; set; }

        [Column(Name = "film_id"), Identity]
        public int FilmId { get; set; }

        [Column(Name = "last_update"), NotNull]
        public MySqlDateTime LastUpdate { get; set; }
    }

    [Table(Name = "film_category")]
    public class FilmCategory
    {
        [Column(Name = "category_id"), Identity]
        public int CategoryId { get; set; }

        [Column(Name = "film_id"), Identity]
        public int FilmId { get; set; }

        [Column(Name = "last_update"), NotNull]
        public MySqlDateTime LastUpdate { get; set; }
    }
}