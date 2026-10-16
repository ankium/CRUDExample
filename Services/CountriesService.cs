using ServiceContracts;
using ServiceContracts.DTO;
using Entities;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        // private field to store the list of countries
        private readonly PersonsDbContext _db;

        // constructor
        public CountriesService(PersonsDbContext personsDbContext)
        {
            _db = personsDbContext;
        }

        public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
        {
            // Validation: Check if countryAddRequest is null
            if (countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }

            // Validation: Check if CountryName is null or empty
            if (string.IsNullOrWhiteSpace(countryAddRequest.CountryName))
            {
                throw new ArgumentException("Country name cannot be null or empty.", nameof(countryAddRequest.CountryName));
            }

            // Validation: Check if a country with the same name already exists in the _countries list
            if (_db.Countries.Any(temp => string.Equals(temp.CountryName, countryAddRequest.CountryName, StringComparison.OrdinalIgnoreCase) == true))
            {
                throw new ArgumentException($"Country with name '{countryAddRequest.CountryName}' already exists.", nameof(countryAddRequest.CountryName));
            }

            // Convert object from CountryAddRequest to Country type
            Country country = countryAddRequest.ToCountry();

            // Generate new GUID for CountryID
            country.CountryID = Guid.NewGuid();

            // Add country object into _contries list
            _db.Countries.Add(country);//Entity State:Added
            _db.SaveChanges();

            // Convert object from Country type to CountryResponse type and return
            return country.ToCountryResponse();
        }

        public List<CountryResponse> GetAllCountries()
        {
            return _db.Countries.Select(country => country.ToCountryResponse()).ToList();
        }

        public CountryResponse? GetCountryByCountryID(Guid? countryID)
        {
            if (countryID == null || countryID == Guid.Empty)
            {
                return null;
            }

            Country? country_response_from_list = _db.Countries.FirstOrDefault(temp => temp.CountryID == countryID);
            if (country_response_from_list == null)
            {
                return null;
            }
            else
            {
                return country_response_from_list.ToCountryResponse();
            }
        }
    }
}
