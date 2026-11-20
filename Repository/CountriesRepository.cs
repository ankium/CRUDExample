using RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using Entities;

namespace Repositories
{
    public class CountriesRepository:ICountriesRepository
    {
        private readonly ApplicationDbContext _db;
        public CountriesRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Country> AddCountry(Country country)
        {
            _db.Countries.Add(country);
            await _db.SaveChangesAsync();
            return country;
        }

        public Task<List<Country>> GetAllCountries()
        {
            return _db.Countries.ToListAsync();
        }

        public async Task<Country?> GetCountryByCountryId(Guid countryId)
        {
            return await _db.Countries.FirstOrDefaultAsync(temp => temp.CountryID == countryId);
        }

        public async Task<Country?> GetCountryByCountryName(string countryName)
        {
            return await _db.Countries.FirstOrDefaultAsync(temp => temp.CountryName == countryName);
        }
    }
}
