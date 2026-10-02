using System;
using System.Collections.Generic;
using System.Text;
using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;

namespace CRUDTest
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;

        // Constructor to initialize the CountriesService instance
        public CountriesServiceTest()
        {
            _countriesService = new CountriesService(false);
        }

        #region AddCountry Tests
        // When CountryAddRequest is null, AddCountry should throw ArgumentNullException
        [Fact]
        public void AddCountry_NullRequest_ThrowsArgumentNullException()
        {
            // Arrange
            CountryAddRequest? countryAddRequest = null;
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _countriesService.AddCountry(countryAddRequest));
        }

        // When the CountryName is null or empty, AddCountry should throw ArgumentException
        [Fact]
        public void AddCountry_NullOrEmptyCountryName_ThrowsArgumentException()
        {
            // Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest { CountryName = null };
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _countriesService.AddCountry(countryAddRequest));
        }

        // When the CountryName is duplicate, AddCountry should throw ArgumentException
        [Fact]
        public void AddCountry_DuplicateCountryName_ThrowsArgumentException()
        {
            // Arrange
            CountryAddRequest countryAddRequest1 = new CountryAddRequest { CountryName = "USA" };
            CountryAddRequest countryAddRequest2 = new CountryAddRequest { CountryName = "USA" };
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
            {
                _countriesService.AddCountry(countryAddRequest1);
                _countriesService.AddCountry(countryAddRequest2);
            });
        }

        // When the CountryName is valid and unique, AddCountry should insert (add) the country to the existing list of countries and return the newly added country object (with newly generated CountryID)
        [Fact]
        public void AddCountry_ValidCountryName_AddsCountrySuccessfully()
        {
            // Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest { CountryName = "Japan" };
            // Act
            CountryResponse countryResponse = _countriesService.AddCountry(countryAddRequest);
            List<CountryResponse> countries_from_GetAllCountries = _countriesService.GetAllCountries();
            // Assert
            Assert.True(countryResponse.CountryID != Guid.Empty);
            Assert.Contains(countryResponse, countries_from_GetAllCountries);
        }
        #endregion

        #region GetAllCountries Tests

        // When there are no countries in the list, GetAllCountries should return an empty list
        [Fact]
        public void GetCountryList_NoCountries_ReturnsEmptyList()
        {
            // Act
            List<CountryResponse> countryList = _countriesService.GetAllCountries();
            // Assert
            Assert.Empty(countryList);
        }

        [Fact]
        public void GetCountryList_WithCountries_ReturnsCountryList()
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
                countries_list_from_add_country.Add(_countriesService.AddCountry(country_add_request));
            }

            List<CountryResponse> actualCountryResponseList = _countriesService.GetAllCountries();
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
        public void GetCountryByCountryID_NullCountryID_ReturnsNull()
        {
            // Arrange
            Guid? countryID = null;
            // Act
            CountryResponse? country_response_from_get_method = _countriesService.GetCountryByCountryID(countryID);
            // Assert
            Assert.Null(country_response_from_get_method);
        }

        // If we supply a valid CountryID to GetCountryByID, it should return the corresponding CountryResponse object
        [Fact]
        public void GetCountryByCountryID_ValidCountryID_ReturnsCountryResponse()
        {
            // Arrange
            CountryAddRequest? country_add_request = new CountryAddRequest { CountryName = "China" };
            CountryResponse country_response_from_add = _countriesService.AddCountry(country_add_request);
            // Act
            CountryResponse? country_response_from_get = _countriesService.GetCountryByCountryID(country_response_from_add.CountryID);
            // Assert
            Assert.Equal(country_response_from_add, country_response_from_get);
        }

        #endregion
    }
}
