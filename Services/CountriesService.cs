using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;

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

        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
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
            await _db.SaveChangesAsync();

            // Convert object from Country type to CountryResponse type and return
            return country.ToCountryResponse();
        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {
            return await _db.Countries.Select(country => country.ToCountryResponse()).ToListAsync();
        }

        public async Task<CountryResponse?> GetCountryByCountryID(Guid? countryID)
        {
            if (countryID == null || countryID == Guid.Empty)
            {
                return null;
            }

            Country? country_response_from_list = await _db.Countries.FirstOrDefaultAsync(temp => temp.CountryID == countryID);
            if (country_response_from_list == null)
            {
                return null;
            }
            else
            {
                return country_response_from_list.ToCountryResponse();
            }
        }

        public async Task<int> UploadCountriesFromExcelFile(IFormFile file)
        {
            MemoryStream memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            int countriesAddedCount = 0;

            // Set the Non-commercial license context for EPPlus
            ExcelPackage.License.SetNonCommercialPersonal("ankium");
            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;
                for (int row = 2; row <= rowCount; row++)
                {
                    string countryName = worksheet.Cells[row, 1].Value?.ToString()?.Trim() ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(countryName) &&
                        !_db.Countries.ToList().Any(temp => string.Equals(temp.CountryName, countryName, StringComparison.OrdinalIgnoreCase)))
                    {
                        Country country = new Country
                        {
                            //CountryID = Guid.NewGuid(),
                            CountryName = countryName
                        };
                        _db.Countries.Add(country);
                        await _db.SaveChangesAsync();
                        countriesAddedCount++;
                    }
                }
            }
            return countriesAddedCount;
        }
    }
}
