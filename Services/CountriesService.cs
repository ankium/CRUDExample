using ServiceContracts;
using ServiceContracts.DTO;
using Entities;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        // private field to store the list of countries
        private readonly List<Country> _countries;

        // constructor
        public CountriesService(bool initialize = true)
        {
            _countries = new List<Country>();
            if (initialize)
            {
                List<Country> country_list = new List<Country>()
                {
                            new Country() {  CountryID = Guid.Parse("000C76EB-62E9-4465-96D1-2C41FDB64C3B"), CountryName = "USA" },

                            new Country() { CountryID = Guid.Parse("32DA506B-3EBA-48A4-BD86-5F93A2E19E3F"), CountryName = "Canada" },

                            new Country() { CountryID = Guid.Parse("DF7C89CE-3341-4246-84AE-E01AB7BA476E"), CountryName = "UK" },

                            new Country() { CountryID = Guid.Parse("15889048-AF93-412C-B8F3-22103E943A6D"), CountryName = "India" },

                            new Country() { CountryID = Guid.Parse("80DF255C-EFE7-49E5-A7F9-C35D7C701CAB"), CountryName = "Australia" }
                };
                _countries.AddRange(country_list);
            }
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
            if (_countries.Any(temp => string.Equals(temp.CountryName, countryAddRequest.CountryName, StringComparison.OrdinalIgnoreCase) == true))
            {
                throw new ArgumentException($"Country with name '{countryAddRequest.CountryName}' already exists.", nameof(countryAddRequest.CountryName));
            }

            // Convert object from CountryAddRequest to Country type
            Country country = countryAddRequest.ToCountry();

            // Generate new GUID for CountryID
            country.CountryID = Guid.NewGuid();

            // Add country object into _contries list
            _countries.Add(country);

            // Convert object from Country type to CountryResponse type and return
            return country.ToCountryResponse();
        }

        public List<CountryResponse> GetAllCountries()
        {
            return _countries.Select(country => country.ToCountryResponse()).ToList();
        }

        CountryResponse? ICountriesService.GetCountryByCountryID(Guid? countryID)
        {
            if (countryID == null || countryID == Guid.Empty)
            {
                return null;
            }

            Country? country_response_from_list = _countries.FirstOrDefault(temp => temp.CountryID == countryID);
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
