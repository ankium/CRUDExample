using System;
using System.Collections.Generic;
using System.Text;
using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using System.Threading.Tasks;
using EntityFrameworkCoreMock;
using Moq;
using FluentAssertions;
using RepositoryContracts;
using AutoFixture;
using AutoFixture.Kernel;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.ObjectModel;

namespace CRUDTest
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;
        private readonly Mock<ICountriesRepository> _countriesRepositoryMock;
        private readonly ICountriesRepository _countriesRepository;
        private readonly IFixture _fixture;

        // Constructor to initialize the CountriesService instance
        public CountriesServiceTest()
        {
            _fixture = new Fixture();
            _countriesRepositoryMock = new Mock<ICountriesRepository>();
            _countriesRepository = _countriesRepositoryMock.Object;

            // Use the mock DbContext to create an instance of CountriesService
            _countriesService = new CountriesService(_countriesRepository);
        }

        #region AddCountry Tests
        // When CountryAddRequest is null, AddCountry should throw ArgumentNullException
        [Fact]
        public async Task AddCountry_NullRequest_ThrowsArgumentNullException()
        {
            // Arrange
            CountryAddRequest? countryAddRequest = null;
            Country country = _fixture.Build<Country>().With(temp => temp.Persons, null as List<Person>).Create();

            _countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>())).ReturnsAsync(country);

            // Act & Assert
            Func<Task> action = async () => await _countriesService.AddCountry(countryAddRequest);
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        // When the CountryName is null or empty, AddCountry should throw ArgumentException
        [Fact]
        public async Task AddCountry_NullOrEmptyCountryName_ThrowsArgumentException()

        {
            // Arrange
            CountryAddRequest countryAddRequest = _fixture.Build<CountryAddRequest>().With(temp => temp.CountryName, null as string).Create();

            Country country = _fixture.Build<Country>().With(temp => temp.Persons, null as List<Person>).Create();

            _countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>())).ReturnsAsync(country);

            // Act & Assert
            Func<Task> action = async () => await _countriesService.AddCountry(countryAddRequest);
            await action.Should().ThrowAsync<ArgumentException>();
        }

        // When the CountryName is duplicate, AddCountry should throw ArgumentException
        [Fact]
        public async Task AddCountry_DuplicateCountryName_ThrowsArgumentException()
        {
            // Arrange
            CountryAddRequest first_country_request = _fixture.Build<CountryAddRequest>().With(temp => temp.CountryName, "USK").Create();

            CountryAddRequest second_country_request = _fixture.Build<CountryAddRequest>().With(temp => temp.CountryName, "USK").Create();

            Country first_country = first_country_request.ToCountry();
            Country second_country = second_country_request.ToCountry();

            _countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>())).ReturnsAsync(first_country);

            _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>())).ReturnsAsync(null as Country);

            CountryResponse first_country_from_add_country = await _countriesService.AddCountry(first_country_request);

            // Act
            Func<Task> action = async () =>
            {
                _countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>())).ReturnsAsync(first_country);

                _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>())).ReturnsAsync(first_country);

                await _countriesService.AddCountry(second_country_request);
            };

            // Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        // When the CountryName is valid and unique, AddCountry should insert (add) the country to the existing list of countries and return the newly added country object (with newly generated CountryID)
        [Fact]
        public async Task AddCountry_ValidCountryName_AddsCountrySuccessfully()
        {
            // Arrange
            CountryAddRequest country_request = _fixture.Create<CountryAddRequest>();
            Country country = country_request.ToCountry();
            CountryResponse country_response = country.ToCountryResponse();

            _countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>())).ReturnsAsync(country);

            _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>())).ReturnsAsync(null as Country);

            // Act
            CountryResponse country_from_add_country = await _countriesService.AddCountry(country_request);

            country.CountryID = country_from_add_country.CountryID;
            country_response.CountryID = country_from_add_country.CountryID;

            // Assert
            country_from_add_country.CountryID.Should().NotBe(Guid.Empty);
            country_from_add_country.Should().BeEquivalentTo(country_response);
        }
        #endregion

        #region GetAllCountries Tests

        // When there are no countries in the list, GetAllCountries should return an empty list
        [Fact]
        public async Task GetCountryList_NoCountries_ReturnsEmptyList()
        {
            //Arrange
            List<Country> country_empty_list = new List<Country>();

            _countriesRepositoryMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(country_empty_list);

            // Act
            List<CountryResponse> actual_country_response_list = await _countriesService.GetAllCountries();
            // Assert
            actual_country_response_list.Should().BeEmpty();
        }

        [Fact]
        public async Task GetCountryList_WithCountries_ReturnsCountryList()
        {
            // Arrange
            List<Country> country_list = new List<Country>()
            {
                _fixture.Build<Country>().With(temp=>temp.Persons,null as List<Person>).Create(),
                _fixture.Build<Country>().With(temp=>temp.Persons,null as List<Person>).Create(),
            };

            List<CountryResponse> country_response_list = country_list.Select(temp => temp.ToCountryResponse()).ToList();

            _countriesRepositoryMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(country_list);

            // Act
            List<CountryResponse> actual_country_response_list = await _countriesService.GetAllCountries();

            // Assert
            actual_country_response_list.Should().BeEquivalentTo(country_response_list);
        }
        #endregion

        #region GetCountryByCountryID Tests

        // If we supply null as the CountryID to GetCountryByID, it should return null as CountryResponse
        [Fact]
        public async Task GetCountryByCountryID_NullCountryID_ReturnsNull()
        {
            // Arrange
            Guid? countryID = null;

            _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryId(It.IsAny<Guid>())).ReturnsAsync(null as Country);

            // Act
            CountryResponse? country_response_from_get_method = await _countriesService.GetCountryByCountryID(countryID);
            // Assert
            country_response_from_get_method.Should().BeNull();
        }

        // If we supply a valid CountryID to GetCountryByID, it should return the corresponding CountryResponse object
        [Fact]
        public async Task GetCountryByCountryID_ValidCountryID_ReturnsCountryResponse()
        {
            // Arrange
            Country country = _fixture.Build<Country>().With(temp => temp.Persons, null as List<Person>).Create();

            CountryResponse country_response = country.ToCountryResponse();

            _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryId(It.IsAny<Guid>())).ReturnsAsync(country);

            // Act
            CountryResponse? country_response_from_get = await _countriesService.GetCountryByCountryID(country.CountryID);

            // Assert
            country_response_from_get.Should().BeEquivalentTo(country_response);
        }

        #endregion
    }
}
