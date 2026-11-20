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
using RepositoryContracts;
using Moq;
using System.Linq.Expressions;

namespace CRUDTest
{
    public class PersonsServiceTest
    {
        // private fields
        private readonly IPersonsService _personsService;
        private readonly Mock<IPersonsRepository> _personsRepositoryMock;
        private readonly IPersonsRepository _personsRepository;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;

        // constructor
        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture();
            _personsRepositoryMock = new Mock<IPersonsRepository>();
            _personsRepository = _personsRepositoryMock.Object;
            _personsService = new PersonsService(_personsRepository);
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

            Person person = personAddRequest.ToPerson();

            // When PersonsRepository.AddPerson is called, it has to return the same "person" object
            _personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);

            //Act
            Func<Task> action = async () => await _personsService.AddPerson(personAddRequest);
            await action.Should().ThrowAsync<ArgumentException>();
        }
        // when we supply proper PersonAddRequest object, it should insert the person into persons list ，then it should return PersonResponse object with newly generated PersonID
        [Fact]
        public async Task AddPerson_ProperPersonAddRequest_ReturnsPersonResponseWithNewlyGeneratedPersonID()
        {
            // Arrange
            // If you want to set specific values for certain properties, you can use the With method to customize the generated object. For example, if you want to set the Email property to a specific value, you can do it like this:
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "someone@example.com")
                .Create();

            Person person = personAddRequest.ToPerson();
            PersonResponse person_response_expected = person.ToPersonResponse();

            // If we supply any argument value to the AddPerson method, it should return the same return value
            _personsRepositoryMock.Setup(temp=>temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);

            // Act
            PersonResponse person_response_from_add = await _personsService.AddPerson(personAddRequest);

            // Set the expected PersonID to match the actual PersonID returned from the service
            person_response_expected.PersonID = person_response_from_add.PersonID; 

            // Assert
            person_response_from_add.PersonID.Should().NotBe(Guid.Empty);
            person_response_from_add.Should().Be(person_response_expected);

        }
        #endregion

        #region GetAllPersons

        // If there are no persons in the list, it should return an empty list
        [Fact]
        public async Task GetAllPersons_EmptyList_ReturnsEmptyList()
        {
            //Arrange
            List<Person> persons = new List<Person>();
            _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);
            //Act
            List<PersonResponse> person_list = await _personsService.GetAllPersons();
            //Assert
            person_list.Should().BeEmpty();
        }

        // First,we will add few persons; and then when we call GetAllPersons method, it should return list of PersonResponse objects
        [Fact]
        public async Task GetAllPersons_AfterAddingFewPersons_ReturnsListOfPersonResponseObjects()
        {
            //Arrange
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(temp => temp.Email, "john@msn.cn")
                .With(temp=>temp.Country,null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "jane@msn.cn")
                .With(temp=>temp.Country,null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "alice@msn.cn")
                .With(temp=>temp.Country,null as Country)
                .Create()
            };

            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();


            // Print the list of PersonResponse objects added
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(person_response.ToString());
            }

            _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

            //Act
            List<PersonResponse> person_response_list_from_get = await _personsService.GetAllPersons();

            // Print the list of PersonResponse objects retrieved
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response in person_response_list_from_get)
            {
                _testOutputHelper.WriteLine(person_response.ToString());
            }

            //Assert
            person_response_list_from_get.Should().BeEquivalentTo(person_response_list_expected);
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
            person_response_from_get.Should().BeNull();
        }

        // If we supply a valid person id,it should return the valid person details as PersonResponse object
        [Fact]
        public async Task GetPersonByPersonID_ValidPersonID_ReturnsValidPersonResponse()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
                .With(temp => temp.Email, "email@sample.com")
                .With(temp=>temp.Country,null as Country)
                .Create();

            PersonResponse person_response_expected = person.ToPersonResponse();

            _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>())).ReturnsAsync(person);

            // Act
            PersonResponse? person_response_from_get = await _personsService.GetPersonByPersonID(person.PersonID);

            //Assert
            person_response_from_get.Should().Be(person_response_expected);
        }
        #endregion

        #region GetFilteredPersons
        [Fact]
        public async Task GetFilteredPersons_EmptySearchString_ReturnsAllPersons()
        {
            //Arrange
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(temp => temp.Email, "john@msn.cn")
                .With(temp=>temp.Country,null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "jane@msn.cn")
                .With(temp=>temp.Country,null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "alice@msn.cn")
                .With(temp=>temp.Country,null as Country)
                .Create()
            };

            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            // Print the list of PersonResponse objects added
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(person_response.ToString());
            }

            _personsRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);

            //Act
            List<PersonResponse> person_response_list_from_search = await _personsService.GetFilteredPersons(nameof(Person.PersonName), "");

            // Print the list of PersonResponse objects retrieved
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response in person_response_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response.ToString());
            }

            //Assert
            person_response_list_from_search.Should().BeEquivalentTo(person_response_list_expected);
        }

        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName_ReturnsMatchingPersons()
        {
            //Arrange
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(temp => temp.Email, "john@msn.cn")
                .With(temp=>temp.Country,null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "jane@msn.cn")
                .With(temp=>temp.Country,null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "alice@msn.cn")
                .With(temp=>temp.Country,null as Country)
                .Create()
            };

            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            // Print the list of PersonResponse objects added
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(person_response.ToString());
            }

            _personsRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);

            //Act
            List<PersonResponse> person_response_list_from_search = await _personsService.GetFilteredPersons(nameof(Person.PersonName), "oh");

            // Print the list of PersonResponse objects retrieved
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response in person_response_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response.ToString());
            }

            //Assert
            person_response_list_from_search.Should().BeEquivalentTo(person_response_list_expected);
        }

        #endregion

        #region GetSortedPersons
        [Fact]
        public async Task GetSortedPersons()
        {
            //Arrange
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(temp => temp.Email, "john@msn.cn")
                .With(temp=>temp.Country,null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "jane@msn.cn")
                .With(temp=>temp.Country,null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "alice@msn.cn")
                .With(temp=>temp.Country,null as Country)
                .Create()
            };

            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

            // Print the list of PersonResponse objects added
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response in person_response_list_expected)
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
                        
            //Assert
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
            Func<Task> action = async () => await _personsService.UpdatePerson(person_update_request);
            await action.Should().ThrowAsync<ArgumentException>();
        }

        // When PersonName is null in PersonUpdateRequest parameter, it should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_WhenNullPersonNameProvided_ShouldThrowArgumentException()
        {
            // Arrange
            Person person = _fixture.Build<Person>()
                .With(temp => temp.PersonName, null as string)
                .With(temp => temp.Email, "john.doe@example.com")
                .With(temp => temp.Country, null as Country)
                .With(temp=>temp.Gender,"Male")
                .Create();

            PersonResponse person_response_from_add = person.ToPersonResponse();

            PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();

            // Act & Assert
            Func<Task> action = async () => await _personsService.UpdatePerson(person_update_request);
            await action.Should().ThrowAsync<ArgumentException>();

        }

        // When we supply valid PersonUpdateRequest parameter, it should update the person details and return updated PersonResponse object
        [Fact]
        public async Task UpdatePerson_WhenValidUpdateRequestProvided_ShouldUpdatePersonAndReturnUpdatedResponse()
        {
            // Arrange
            Person person = _fixture.Build<Person>()
                .With(temp => temp.PersonName, "John Doe")
                .With(temp => temp.Email, "john.doe@example.com")
                .With(temp => temp.Country, null as Country)
                .With(temp=>temp.Gender,"Male")
                .Create();
            PersonResponse person_response_expected = person.ToPersonResponse();

            PersonUpdateRequest person_update_request = person_response_expected.ToPersonUpdateRequest();

            _personsRepositoryMock.Setup(temp => temp.UpdatePerson(It.IsAny<Person>())).ReturnsAsync(person);
            _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>())).ReturnsAsync(person);

            // Act
            PersonResponse person_response_from_update = await _personsService.UpdatePerson(person_update_request);

            // Assert
            //Assert.Equal(person_response_from_get, person_response_from_update);
            person_response_from_update.Should().Be(person_response_expected);

        }

        #endregion

        #region DeletePerson
        // If you supply an valid person id, it should delete the person and return true
        [Fact]
        public async Task DeletePerson_ValidPersonID_ReturnsTrue()
        {
            // Arrange
            Person person = _fixture.Build<Person>()
                .With(temp => temp.PersonName, "John Doe")
                .With(temp => temp.Email, "john.doe@example.com")
                .With(temp => temp.Country, null as Country)
                .With(temp=>temp.Gender,"Female")
                .Create();

            // Act
            _personsRepositoryMock.Setup(temp => temp.DeletePersonByPersonId(It.IsAny<Guid>())).ReturnsAsync(true);
            _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>())).ReturnsAsync(person);

            bool delete_result = await _personsService.DeletePerson(person.PersonID);

            // Assert
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
