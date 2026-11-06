using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

namespace Entities
{
    public class PersonsDbContext : DbContext
    {
        public PersonsDbContext(DbContextOptions options)
            : base(options) { }

        public DbSet<Person> Persons { get; set; }
        public DbSet<Country> Countries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons");

            // Seed data for Countries
            string countriesJson = System.IO.File.ReadAllText("countries.json");
            List<Country> countries = System.Text.Json.JsonSerializer.Deserialize<List<Country>>(countriesJson);

            foreach (Country country in countries)
            {
                modelBuilder.Entity<Country>().HasData(country);
            }

            // Seed data for Persons
            string personsJson = System.IO.File.ReadAllText("persons.json");
            List<Person> persons = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(personsJson);

            foreach (Person person in persons)
            {
                modelBuilder.Entity<Person>().HasData(person);
            }

            modelBuilder.Entity<Person>().Property(temp => temp.TIN)
                .HasColumnName("TaxIdentificationNumber")
                .HasColumnType("varchar(8)")
                .HasDefaultValue("ABC12345");

            modelBuilder.Entity<Person>().HasCheckConstraint("CHK_TIN", "LEN(TaxIdentificationNumber) = 8");
        }

        public List<Person> sp_GetAllPersons()
        {
            // Execute the stored procedure to get all persons
            List<Person> persons = this.Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList();
            return persons;
        }

        public int sp_InsertPerson(Person person)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@PersonID", person.PersonID),
                new SqlParameter("@PersonName", person.PersonName),
                new SqlParameter("@Email", person.Email),
                new SqlParameter("@DateOfBirth", person.DateOfBirth),
                new SqlParameter("@Gender", person.Gender),
                new SqlParameter("@CountryID", person.CountryID),
                new SqlParameter("@Address", person.Address),
                new SqlParameter("@ReceiveNewsLetters", person.ReceiveNewsLetters)
            };
            // Execute the stored procedure to insert a person
            int result = this.Database.ExecuteSqlRaw(
                "EXECUTE [dbo].[InsertPerson] @PersonID, @PersonName, @Email, @DateOfBirth, @Gender, @CountryID, @Address, @ReceiveNewsLetters", parameters);
            return result;
        }
    }
}
