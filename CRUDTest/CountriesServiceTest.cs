using System;
using System.Collections.Generic;
using System.Text;
using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using System.Threading.Tasks;

namespace CRUDTest
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;

        // Constructor to initialize the CountriesService instance
        public CountriesServiceTest()
        {
            _countriesService = new CountriesService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options));
        }

        #region AddCountry Tests
        // When CountryAddRequest is null, AddCountry should throw ArgumentNullException
        [Fact]
        public async Task AddCountry_NullRequest_ThrowsArgumentNullException()
        {
            // Arrange
            CountryAddRequest? countryAddRequest = null;
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _countriesService.AddCountry(countryAddRequest));
        }

        // When the CountryName is null or empty, AddCountry should throw ArgumentException
        [Fact]
        public async Task AddCountry_NullOrEmptyCountryName_ThrowsArgumentException()

        {
            // Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest { CountryName = null };
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _countriesService.AddCountry(countryAddRequest));
        }

        // When the CountryName is duplicate, AddCountry should throw ArgumentException
        [Fact]
        public async Task AddCountry_DuplicateCountryName_ThrowsArgumentException()
        {
            // Arrange
            CountryAddRequest countryAddRequest1 = new CountryAddRequest { CountryName = "USA" };
            CountryAddRequest countryAddRequest2 = new CountryAddRequest { CountryName = "USA" };
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _countriesService.AddCountry(countryAddRequest1);
                await _countriesService.AddCountry(countryAddRequest2);
            });
        }

        // When the CountryName is valid and unique, AddCountry should insert (add) the country to the existing list of countries and return the newly added country object (with newly generated CountryID)
        [Fact]
        public async Task AddCountry_ValidCountryName_AddsCountrySuccessfully()
        {
            // Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest { CountryName = "Japan" };
            // Act
            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);
            List<CountryResponse> countries_from_GetAllCountries = await _countriesService.GetAllCountries();
            // Assert
            Assert.True(countryResponse.CountryID != Guid.Empty);
            Assert.Contains(countryResponse, countries_from_GetAllCountries);
        }
        #endregion

        #region GetAllCountries Tests

        // When there are no countries in the list, GetAllCountries should return an empty list
        [Fact]
        public async Task GetCountryList_NoCountries_ReturnsEmptyList()
        {
            // Act
            List<CountryResponse> countryList = await _countriesService.GetAllCountries();
            // Assert
            Assert.Empty(countryList);
        }

        [Fact]
        public async Task GetCountryList_WithCountries_ReturnsCountryList()
        {
            // Arrange
            List<CountryAddRequest> country_add_request_list = new List<CountryAddRequest>
            {
                new CountryAddRequest { CountryName = "USA" },
                new CountryAddRequest { CountryName = "UK" }
            };

            // Act
            List<CountryResponse> countries_list_from_add_country = new List<CountryResponse>();
            foreach (CountryAddRequest country_add_request in country_add_request_list)
            {
                countries_list_from_add_country.Add(await _countriesService.AddCountry(country_add_request));
            }

            List<CountryResponse> actualCountryResponseList = await _countriesService.GetAllCountries();
            // read each element from countries_list_from_add_country
            foreach (var expected_country in countries_list_from_add_country)
            {
                Assert.Contains(expected_country, actualCountryResponseList);
            }
        }
        #endregion

        #region GetCountryByCountryID Tests

        // If we supply null as the CountryID to GetCountryByID, it should return null as CountryResponse
        [Fact]
        public async Task GetCountryByCountryID_NullCountryID_ReturnsNull()
        {
            // Arrange
            Guid? countryID = null;
            // Act
            CountryResponse? country_response_from_get_method = await _countriesService.GetCountryByCountryID(countryID);
            // Assert
            Assert.Null(country_response_from_get_method);
        }

        // If we supply a valid CountryID to GetCountryByID, it should return the corresponding CountryResponse object
        [Fact]
        public async Task GetCountryByCountryID_ValidCountryID_ReturnsCountryResponse()
        {
            // Arrange
            CountryAddRequest? country_add_request = new CountryAddRequest { CountryName = "China" };
            CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);
            // Act
            CountryResponse? country_response_from_get = await _countriesService.GetCountryByCountryID(country_response_from_add.CountryID);
            // Assert
            Assert.Equal(country_response_from_add, country_response_from_get);
        }

        #endregion
    }
}
