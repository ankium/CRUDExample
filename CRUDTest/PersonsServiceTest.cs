using Entities;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using AutoFixture;
using FluentAssertions;

namespace CRUDTest
{
    public class PersonsServiceTest
    {
        // private fields
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;

        // constructor
        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture();

            // Create a mock DbContext using EntityFrameworkCoreMock
            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options);

            // Create a mock DbSet for Countries
            var countriesInitialData = new List<Country>() { };
            var personsInitialData = new List<Person>() { };
            dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
            dbContextMock.CreateDbSetMock(temp => temp.Persons, personsInitialData);

            // Get the mock DbContext object from the DbContextMock
            ApplicationDbContext dbContext = dbContextMock.Object;

            // Use the mock DbContext to create instances of CountriesService and PersonsService
            _countriesService = new CountriesService(null);
            _personsService = new PersonsService(null);

            _testOutputHelper = testOutputHelper;
        }

        #region AddPerson
        //when we supply null value for PersonAddRequest parameter, then it should throw ArgumentNullException
        [Fact]
        public async Task AddPerson_NullPersonAddRequest_ThrowsArgumentNullException()
        {
            //Arrange
            PersonAddRequest? personAddRequest = null;
            //Act
            //await Assert.ThrowsAsync<ArgumentNullException>(async () => await _personsService.AddPerson(personAddRequest));

            Func<Task> action = async () => await _personsService.AddPerson(personAddRequest);
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        //when we supply null value for PersonName property of PersonAddRequest parameter, then it should throw ArgumentException
        [Fact]
        public async Task AddPerson_NullPersonName_ThrowsArgumentException()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, null as string)
                .Create();

            //Act
            //await Assert.ThrowsAsync<ArgumentException>(async () => await _personsService.AddPerson(personAddRequest));
            Func<Task> action = async () => await _personsService.AddPerson(personAddRequest);
            await action.Should().ThrowAsync<ArgumentException>();
        }
        // when we supply proper PersonAddRequest object, it should insert the person into persons list ，then it should return PersonResponse object with newly generated PersonID
        [Fact]
        public async Task AddPerson_ProperPersonAddRequest_ReturnsPersonResponseWithNewlyGeneratedPersonID()
        {
            // Arrange
            // PersonAddRequest? personAddRequest = _fixture.Create<PersonAddRequest>();
            // If you want to set specific values for certain properties, you can use the With method to customize the generated object. For example, if you want to set the Email property to a specific value, you can do it like this:
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "someone@example.com")
                .Create();

            // Act
            PersonResponse person_response_from_add = await _personsService.AddPerson(personAddRequest);
            List<PersonResponse> person_list = await _personsService.GetAllPersons();

            // Assert
            //Assert.True(person_response_from_add.PersonID != Guid.Empty);
            person_response_from_add.PersonID.Should().NotBe(Guid.Empty);
            //Assert.Contains(person_response_from_add, person_list);
            person_list.Should().Contain(person_response_from_add);

        }
        #endregion

        #region GetAllPersons

        // If there are no persons in the list, it should return an empty list
        [Fact]
        public async Task GetAllPersons_EmptyList_ReturnsEmptyList()
        {
            //Arrange
            //Act
            List<PersonResponse> person_list = await _personsService.GetAllPersons();
            //Assert
            //Assert.Empty(person_list);
            person_list.Should().BeEmpty();
        }

        // First,we will add few persons; and then when we call GetAllPersons method, it should return list of PersonResponse objects
        [Fact]
        public async Task GetAllPersons_AfterAddingFewPersons_ReturnsListOfPersonResponseObjects()
        {
            //Arrange
            CountryAddRequest country_add_request_1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest country_add_request_2 = _fixture.Create<CountryAddRequest>();

            CountryResponse country_response_1 = await _countriesService.AddCountry(country_add_request_1);
            CountryResponse country_response_2 = await _countriesService.AddCountry(country_add_request_2);

            PersonAddRequest personAddRequest1 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "john@msn.cn")
                .Create();
            PersonAddRequest personAddRequest2 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "jane@msn.cn")
                .Create();

            PersonAddRequest personAddRequest3 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "alice@msn.cn")
                .Create();

            List <PersonAddRequest> person_add_requests = new List<PersonAddRequest>() { personAddRequest1, personAddRequest2, personAddRequest3 };
            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_add_request in person_add_requests)
            {
                PersonResponse person_response = await _personsService.AddPerson(person_add_request);
                person_response_list_from_add.Add(person_response);
            }

            // Print the list of PersonResponse objects added
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response.ToString());
            }

            //Act
            List<PersonResponse> person_response_list_from_get = await _personsService.GetAllPersons();

            // Print the list of PersonResponse objects retrieved
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response in person_response_list_from_get)
            {
                _testOutputHelper.WriteLine(person_response.ToString());
            }

            //Assert
            /*
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                Assert.Contains(person_response_from_add, person_response_list_from_get);
            }
            */
            person_response_list_from_get.Should().BeEquivalentTo(person_response_list_from_add);
        }
        #endregion

        #region GetPersonByPersonID
        // If we supply null as PersonID,it should return null as PersonResponse
        [Fact]
        public async Task GetPersonByPersonID_NullPersonID_ReturnsNull()
        {
            //Arrange
            Guid? personID = null;
            //Act
            PersonResponse? person_response_from_get = await _personsService.GetPersonByPersonID(personID);
            //Assert
            //Assert.Null(person_response_from_get);
            person_response_from_get.Should().BeNull();
        }

        // If we supply a valid person id,it should return the valid person details as PersonResponse object
        [Fact]
        public async Task GetPersonByPersonID_ValidPersonID_ReturnsValidPersonResponse()
        {
            //Arrange
            CountryAddRequest countryAddRequest = _fixture.Create<CountryAddRequest>();
            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);
            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "email@sample.com")
                .Create();
            PersonResponse person_response_from_add = await _personsService.AddPerson(personAddRequest);
            PersonResponse? person_response_from_get = await _personsService.GetPersonByPersonID(person_response_from_add.PersonID);

            //Assert
            //Assert.Equal(person_response_from_add, person_response_from_get);
            person_response_from_get.Should().Be(person_response_from_add);
        }
        #endregion

        #region GetFilteredPersons
        [Fact]
        public async Task GetFilteredPersons_EmptySearchString_ReturnsAllPersons()
        {
            //Arrange
            CountryAddRequest country_add_request_1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest country_add_request_2 = _fixture.Create<CountryAddRequest>();

            CountryResponse country_response_1 = await _countriesService.AddCountry(country_add_request_1);
            CountryResponse country_response_2 = await _countriesService.AddCountry(country_add_request_2);

            PersonAddRequest personAddRequest1 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "john@msn.cn")
                .Create();
            PersonAddRequest personAddRequest2 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "jane@msn.cn")
                .Create();

            PersonAddRequest personAddRequest3 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "alice@msn.cn")
                .Create();


            List<PersonAddRequest> person_add_requests = new List<PersonAddRequest>() { personAddRequest1, personAddRequest2, personAddRequest3 };
            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_add_request in person_add_requests)
            {
                PersonResponse person_response = await _personsService.AddPerson(person_add_request);
                person_response_list_from_add.Add(person_response);
            }

            // Print the list of PersonResponse objects added
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response.ToString());
            }

            //Act
            List<PersonResponse> person_response_list_from_search = await _personsService.GetFilteredPersons(nameof(Person.PersonName), "");

            // Print the list of PersonResponse objects retrieved
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response in person_response_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response.ToString());
            }

            //Assert
            /*
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                Assert.Contains(person_response_from_add, person_response_list_from_search);
            }
            */
            person_response_list_from_search.Should().BeEquivalentTo(person_response_list_from_add);
        }

        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName_ReturnsMatchingPersons()
        {
            //Arrange
            CountryAddRequest country_add_request_1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest country_add_request_2 = _fixture.Create<CountryAddRequest>();

            CountryResponse country_response_1 = await _countriesService.AddCountry(country_add_request_1);
            CountryResponse country_response_2 = await _countriesService.AddCountry(country_add_request_2);

            PersonAddRequest personAddRequest1 = _fixture.Build<PersonAddRequest>()
                .With(temp=>temp.PersonName, "John Doe")
                .With(temp => temp.Email, "john@msn.cn")
                .With(temp => temp.CountryID, country_response_1.CountryID)
                .Create();
            PersonAddRequest personAddRequest2 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Jane Smith")
                .With(temp => temp.Email, "jane@msn.cn")
                .With(temp => temp.CountryID, country_response_2.CountryID)
                .Create();

            PersonAddRequest personAddRequest3 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Alice Johnson")
                .With(temp => temp.Email, "alice@msn.cn")
                .With(temp => temp.CountryID, country_response_1.CountryID) 
                .Create();

            List<PersonAddRequest> person_add_requests = new List<PersonAddRequest>() { personAddRequest1, personAddRequest2, personAddRequest3 };
            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_add_request in person_add_requests)
            {
                PersonResponse person_response = await _personsService.AddPerson(person_add_request);
                person_response_list_from_add.Add(person_response);
            }

            // Print the list of PersonResponse objects added
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response.ToString());
            }

            //Act
            List<PersonResponse> person_response_list_from_search = await _personsService.GetFilteredPersons(nameof(Person.PersonName), "oh");

            // Print the list of PersonResponse objects retrieved
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response in person_response_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response.ToString());
            }

            //Assert
            /*
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                if (person_response_from_add.PersonName != null)
                {
                    if (person_response_from_add.PersonName.Contains("oh", StringComparison.OrdinalIgnoreCase))
                    {
                        Assert.Contains(person_response_from_add, person_response_list_from_search);
                    }
                }
            }
            */

            person_response_list_from_search.Should().OnlyContain(temp => temp.PersonName != null && temp.PersonName.Contains("oh", StringComparison.OrdinalIgnoreCase));
        }

        #endregion

        #region GetSortedPersons
        [Fact]
        public async Task GetSortedPersons()
        {

            //Arrange
            CountryAddRequest country_add_request_1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest country_add_request_2 = _fixture.Create<CountryAddRequest>();

            CountryResponse country_response_1 = await _countriesService.AddCountry(country_add_request_1);
            CountryResponse country_response_2 = await _countriesService.AddCountry(country_add_request_2);

            PersonAddRequest personAddRequest1 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "John Doe")
                .With(temp => temp.Email, "john@msn.cn")
                .With(temp => temp.CountryID, country_response_1.CountryID)
                .Create();
            PersonAddRequest personAddRequest2 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Jane Smith")
                .With(temp => temp.Email, "jane@msn.cn")
                .With(temp => temp.CountryID, country_response_2.CountryID)
                .Create();

            PersonAddRequest personAddRequest3 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Alice Johnson")
                .With(temp => temp.Email, "alice@msn.cn")
                .With(temp => temp.CountryID, country_response_1.CountryID)
                .Create();

            List<PersonAddRequest> person_add_requests = new List<PersonAddRequest>() { personAddRequest1, personAddRequest2, personAddRequest3 };
            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_add_request in person_add_requests)
            {
                PersonResponse person_response = await _personsService.AddPerson(person_add_request);
                person_response_list_from_add.Add(person_response);
            }

            // Print the list of PersonResponse objects added
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response.ToString());
            }

            //Act
            List<PersonResponse> person_response_list_from_sort =await _personsService.GetSortedPersons(await _personsService.GetAllPersons(), nameof(Person.PersonName), SortOrderOptions.DESC);

            // Print the list of PersonResponse objects retrieved
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response in person_response_list_from_sort)
            {
                _testOutputHelper.WriteLine(person_response.ToString());
            }

            //person_response_list_from_add = person_response_list_from_add.OrderByDescending(temp => temp.PersonName).ToList();
            //Assert
            /*
            for (int i = 0; i < person_response_list_from_add.Count; i++)
            {
                Assert.Equal(person_response_list_from_add[i], person_response_list_from_sort[i]);
            }
            */
            person_response_list_from_sort.Should().BeInDescendingOrder(temp => temp.PersonName);
        }
        #endregion

        #region UpdatePerson

        // When we supply null value for PersonUpdateRequest parameter, it should throw ArgumentNullException
        [Fact]
        public async Task UpdatePerson_WhenNullUpdateRequestProvided_ShouldThrowArgumentNullException()
        {
            // Arrange
            PersonUpdateRequest? person_update_request = null;

            // Act & Assert
            //await Assert.ThrowsAsync<ArgumentNullException>(async () => await _personsService.UpdatePerson(person_update_request));
            Func<Task> action = async () => await _personsService.UpdatePerson(person_update_request);
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        // When we supply invalid PersonID in PersonUpdateRequest parameter, it should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_WhenInvalidPersonIDProvided_ShouldThrowArgumentException()
        {
            // Arrange
            PersonUpdateRequest? person_update_request = _fixture.Create<PersonUpdateRequest>();

            // Act & Assert
            //await Assert.ThrowsAsync<ArgumentException>(async () => await _personsService.UpdatePerson(person_update_request));
            Func<Task> action = async () => await _personsService.UpdatePerson(person_update_request);
            await action.Should().ThrowAsync<ArgumentException>();
        }

        // When PersonName is null in PersonUpdateRequest parameter, it should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_WhenNullPersonNameProvided_ShouldThrowArgumentException()
        {
            // Arrange
            CountryAddRequest country_add_request = _fixture.Create<CountryAddRequest>();
            CountryResponse country_response = await _countriesService.AddCountry(country_add_request);
            PersonAddRequest person_add_request = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "John Doe")
                .With(temp => temp.Email, "john.doe@example.com")
                .With(temp => temp.CountryID, country_response.CountryID)
                .Create();
            PersonResponse person_response_from_add = await _personsService.AddPerson(person_add_request);

            PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
            person_update_request.PersonName = null; // Invalid PersonName

            // Act & Assert
            //await Assert.ThrowsAsync<ArgumentException>(async () => await _personsService.UpdatePerson(person_update_request));
            Func<Task> action = async () => await _personsService.UpdatePerson(person_update_request);
            await action.Should().ThrowAsync<ArgumentException>();

        }

        // When we supply valid PersonUpdateRequest parameter, it should update the person details and return updated PersonResponse object
        [Fact]
        public async Task UpdatePerson_WhenValidUpdateRequestProvided_ShouldUpdatePersonAndReturnUpdatedResponse()
        {
            // Arrange
            CountryAddRequest country_add_request = _fixture.Create<CountryAddRequest>();
            CountryResponse country_response = await _countriesService.AddCountry(country_add_request);
            PersonAddRequest person_add_request = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "John Doe")
                .With(temp => temp.Email, "john.doe@example.com")
                .With(temp => temp.CountryID, country_response.CountryID)
                .Create();
            PersonResponse person_response_from_add = await _personsService.AddPerson(person_add_request);

            PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
            person_update_request.PersonName = "John Smith"; // Update PersonName
            person_update_request.Email = "john.smith@example.com"; // Update Email

            // Act
            PersonResponse person_response_from_update = await _personsService.UpdatePerson(person_update_request);

            PersonResponse? person_response_from_get = await _personsService.GetPersonByPersonID(person_response_from_add.PersonID);

            // Assert
            //Assert.Equal(person_response_from_get, person_response_from_update);
            person_response_from_update.Should().Be(person_response_from_get);

        }

        #endregion

        #region DeletePerson
        // If you supply an valid person id, it should delete the person and return true
        [Fact]
        public async Task DeletePerson_ValidPersonID_ReturnsTrue()
        {
            // Arrange
            CountryAddRequest country_add_request = _fixture.Create<CountryAddRequest>();
            CountryResponse country_response = await _countriesService.AddCountry(country_add_request);
            PersonAddRequest person_add_request = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "John Doe")
                .With(temp => temp.Email, "john.doe@example.com")
                .With(temp => temp.CountryID, country_response.CountryID)
                .Create();
            PersonResponse person_response_from_add = await _personsService.AddPerson(person_add_request);

            // Act
            bool delete_result = await _personsService.DeletePerson(person_response_from_add.PersonID);

            // Assert
            //Assert.True(delete_result);
            delete_result.Should().BeTrue();
        }


        // If you supply an invalid person id, it should return false
        [Fact]
        public async Task DeletePerson_InvalidPersonID_ReturnsFalse()
        {
            //Arrange
            Guid invalid_person_id = Guid.NewGuid();
            // Act
            bool delete_result = await _personsService.DeletePerson(invalid_person_id);
            // Assert
            //Assert.False(delete_result);
            delete_result.Should().BeFalse();
        }
        #endregion
    }
}
