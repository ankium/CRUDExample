using RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using Entities;
using System.Linq.Expressions;

namespace Repository
{
    public class PersonsRepository:IPersonsRepository
    {
        private readonly ApplicationDbContext _db;
        public PersonsRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Person> AddPerson(Person person)
        {
            _db.Persons.Add(person);
            await _db.SaveChangesAsync();
            return person;
        }

        public async Task<bool> DeletePersonByPersonId(Guid personId)
        {
            _db.Persons.RemoveRange(_db.Persons.Where(temp => temp.PersonID == personId));
            int rowsDeleted = await _db.SaveChangesAsync();
            return rowsDeleted > 0;
        }

        public async Task<List<Person>> GetAllPersons()
        {
            return await _db.Persons.Include("Country").ToListAsync();
        }

        public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
        {
            return await _db.Persons.Include("Country")
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<Person?> GetPersonByPersonId(Guid personId)
        {
            return await _db.Persons.Include("Country").FirstOrDefaultAsync(temp => temp.PersonID == personId);
        }

        public async Task<Person> UpdatePerson(Person person)
        {
            Person? matchPerson = await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonID == person.PersonID);
            if (matchPerson==null)
            {
                return person;
            }
            matchPerson.PersonName = person.PersonName;
            matchPerson.Email = person.Email;
            matchPerson.DateOfBirth = person.DateOfBirth;
            matchPerson.Gender = person.Gender;
            matchPerson.CountryID = person.CountryID;
            matchPerson.Address = person.Address;
            matchPerson.ReceiveNewsLetters = person.ReceiveNewsLetters;
            int countUpdated = await _db.SaveChangesAsync();
            return matchPerson;
        }
    }
}
