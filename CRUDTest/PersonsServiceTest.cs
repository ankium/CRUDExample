using System;
using System.Collections.Generic;
using System.Text;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using Xunit;
using ServiceContracts.Enums;
using Entities;
using Xunit.Abstractions;

namespace CRUDTest
{
    public class PersonsServiceTest
    {
        // private fields
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;
        private readonly ITestOutputHelper _testOutputHelper;

        // constructor
        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _personsService = new PersonsService(false);
            _countriesService = new CountriesService(false);
        }

        #region AddPerson
        //when we supply null value for PersonAddRequest parameter, then it should throw ArgumentNullException
        [Fact]
        public void AddPerson_NullPersonAddRequest_ThrowsArgumentNullException()
        {
            //Arrange
            PersonAddRequest? personAddRequest = null;
            //Act
            Assert.Throws<ArgumentNullException>(() => _personsService.AddPerson(personAddRequest));
        }

        //when we supply null value for PersonName property of PersonAddRequest parameter, then it should throw ArgumentException
        [Fact]
        public void AddPerson_NullPersonName_ThrowsArgumentException()
        {
            //Arrange
            PersonAddRequest? personAddRequest = new PersonAddRequest()
            {
                PersonName = null
            };
            //Act
            Assert.Throws<ArgumentException>(() => _personsService.AddPerson(personAddRequest));
        }
        // when we supply proper PersonAddRequest object, it should insert the person into persons list ，then it should return PersonResponse object with newly generated PersonID
        [Fact]
        public void AddPerson_ProperPersonAddRequest_ReturnsPersonResponseWithNewlyGeneratedPersonID()
        {
            //Arrange
            PersonAddRequest? personAddRequest = new PersonAddRequest()
            {
                PersonName = "John Doe",
                Email = "john@msn.cn",
                DateOfBirth = DateTime.Parse("2002-12-15"),
                Address = "123 Main St",
                CountryID = Guid.NewGuid(),
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true
            };
            // Act
            PersonResponse person_response_from_add = _personsService.AddPerson(personAddRequest);
            List<PersonResponse> person_list = _personsService.GetAllPersons();

            // Assert
            Assert.True(person_response_from_add.PersonID != Guid.Empty);
            Assert.Contains(person_response_from_add, person_list);
        }
        #endregion

        #region GetAllPersons

        // First,we will add few persons; and then when we call GetAllPersons method, it should return list of PersonResponse objects
        [Fact]
        public void GetAllPersons_AfterAddingFewPersons_ReturnsListOfPersonResponseObjects()
        {
            //Arrange
            CountryAddRequest country_add_request_1 = new CountryAddRequest()
            {
                CountryName = "canada"
            };
            CountryAddRequest country_add_request_2 = new CountryAddRequest()
            {
                CountryName = "usa"
            };

            CountryResponse country_response_1 = _countriesService.AddCountry(country_add_request_1);
            CountryResponse country_response_2 = _countriesService.AddCountry(country_add_request_2);

            PersonAddRequest personAddRequest1 = new PersonAddRequest()
            {
                PersonName = "John Doe",
                Email = "john@msn.cn",
                DateOfBirth = DateTime.Parse("2002-12-15"),
                Address = "123 Main St",
                CountryID = country_response_1.CountryID,
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true
            };
            PersonAddRequest personAddRequest2 = new PersonAddRequest()
            {
                PersonName = "Jane Smith",
                Email = "jane@msn.cn",
                DateOfBirth = DateTime.Parse("2001-08-20"),
                Address = "456 Oak Ave",
                CountryID = country_response_2.CountryID,
                Gender = GenderOptions.Female,
                ReceiveNewsLetters = false
            };
            PersonAddRequest personAddRequest3 = new PersonAddRequest()
            {
                PersonName = "Alice Johnson",
                Email = "alice@msn.cn",
                DateOfBirth = DateTime.Parse("2000-05-10"),
                Address = "789 Pine Rd",
                CountryID = country_response_1.CountryID,
                Gender = GenderOptions.Female,
                ReceiveNewsLetters = true
            };

            List<PersonAddRequest> person_add_requests = new List<PersonAddRequest>() { personAddRequest1, personAddRequest2, personAddRequest3 };
            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_add_request in person_add_requests)
            {
                PersonResponse person_response = _personsService.AddPerson(person_add_request);
                person_response_list_from_add.Add(person_response);
            }

            // Print the list of PersonResponse objects added
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response.ToString());
            }

            //Act
            List<PersonResponse> person_response_list_from_get = _personsService.GetAllPersons();

            // Print the list of PersonResponse objects retrieved
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response in person_response_list_from_get)
            {
                _testOutputHelper.WriteLine(person_response.ToString());
            }

            //Assert
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                Assert.Contains(person_response_from_add, person_response_list_from_get);
            }
        }
        #endregion

        #region GetPersonByPersonID
        // If we supply null as PersonID,it should return null as PersonResponse
        [Fact]
        public void GetPersonByPersonID_NullPersonID_ReturnsNull()
        {
            //Arrange
            Guid? personID = null;
            //Act
            PersonResponse? person_response_from_get = _personsService.GetPersonByPersonID(personID);
            //Assert
            Assert.Null(person_response_from_get);
        }

        // If we supply a valid person id,it should return the valid person details as PersonResponse object
        [Fact]
        public void GetPersonByPersonID_ValidPersonID_ReturnsValidPersonResponse()
        {
            //Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                CountryName = "canada"
            };
            CountryResponse countryResponse = _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "John Doe",
                Email = "john@msn.cn",
                DateOfBirth = new DateTime(2000, 1, 1),
                Address = "123 Main St",
                CountryID = countryResponse.CountryID,
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true
            };
            PersonResponse person_response_from_add = _personsService.AddPerson(personAddRequest);
            PersonResponse? person_response_from_get = _personsService.GetPersonByPersonID(person_response_from_add.PersonID);

            //Assert
            Assert.Equal(person_response_from_add, person_response_from_get);
        }
        #endregion

        #region GetFilteredPersons
        [Fact]
        public void GetFilteredPersons_EmptySearchString_ReturnsAllPersons()
        {
            //Arrange
            CountryAddRequest country_add_request_1 = new CountryAddRequest()
            {
                CountryName = "canada"
            };
            CountryAddRequest country_add_request_2 = new CountryAddRequest()
            {
                CountryName = "usa"
            };

            CountryResponse country_response_1 = _countriesService.AddCountry(country_add_request_1);
            CountryResponse country_response_2 = _countriesService.AddCountry(country_add_request_2);

            PersonAddRequest personAddRequest1 = new PersonAddRequest()
            {
                PersonName = "John Doe",
                Email = "john@msn.cn",
                DateOfBirth = DateTime.Parse("2002-12-15"),
                Address = "123 Main St",
                CountryID = country_response_1.CountryID,
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true
            };
            PersonAddRequest personAddRequest2 = new PersonAddRequest()
            {
                PersonName = "Jane Smith",
                Email = "jane@msn.cn",
                DateOfBirth = DateTime.Parse("2001-08-20"),
                Address = "456 Oak Ave",
                CountryID = country_response_2.CountryID,
                Gender = GenderOptions.Female,
                ReceiveNewsLetters = false
            };
            PersonAddRequest personAddRequest3 = new PersonAddRequest()
            {
                PersonName = "Alice Johnson",
                Email = "alice@msn.cn",
                DateOfBirth = DateTime.Parse("2000-05-10"),
                Address = "789 Pine Rd",
                CountryID = country_response_1.CountryID,
                Gender = GenderOptions.Female,
                ReceiveNewsLetters = true
            };

            List<PersonAddRequest> person_add_requests = new List<PersonAddRequest>() { personAddRequest1, personAddRequest2, personAddRequest3 };
            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_add_request in person_add_requests)
            {
                PersonResponse person_response = _personsService.AddPerson(person_add_request);
                person_response_list_from_add.Add(person_response);
            }

            // Print the list of PersonResponse objects added
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response.ToString());
            }

            //Act
            List<PersonResponse> person_response_list_from_search = _personsService.GetFilteredPersons(nameof(Person.PersonName), "");

            // Print the list of PersonResponse objects retrieved
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response in person_response_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response.ToString());
            }

            //Assert
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                Assert.Contains(person_response_from_add, person_response_list_from_search);
            }
        }

        [Fact]
        public void GetFilteredPersons_SearchByPersonName_ReturnsMatchingPersons()
        {
            //Arrange
            CountryAddRequest country_add_request_1 = new CountryAddRequest()
            {
                CountryName = "canada"
            };
            CountryAddRequest country_add_request_2 = new CountryAddRequest()
            {
                CountryName = "usa"
            };

            CountryResponse country_response_1 = _countriesService.AddCountry(country_add_request_1);
            CountryResponse country_response_2 = _countriesService.AddCountry(country_add_request_2);

            PersonAddRequest personAddRequest1 = new PersonAddRequest()
            {
                PersonName = "John Doe",
                Email = "john@msn.cn",
                DateOfBirth = DateTime.Parse("2002-12-15"),
                Address = "123 Main St",
                CountryID = country_response_1.CountryID,
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true
            };
            PersonAddRequest personAddRequest2 = new PersonAddRequest()
            {
                PersonName = "Jane Smith",
                Email = "jane@msn.cn",
                DateOfBirth = DateTime.Parse("2001-08-20"),
                Address = "456 Oak Ave",
                CountryID = country_response_2.CountryID,
                Gender = GenderOptions.Female,
                ReceiveNewsLetters = false
            };
            PersonAddRequest personAddRequest3 = new PersonAddRequest()
            {
                PersonName = "Alice Johnson",
                Email = "alice@msn.cn",
                DateOfBirth = DateTime.Parse("2000-05-10"),
                Address = "789 Pine Rd",
                CountryID = country_response_1.CountryID,
                Gender = GenderOptions.Female,
                ReceiveNewsLetters = true
            };

            List<PersonAddRequest> person_add_requests = new List<PersonAddRequest>() { personAddRequest1, personAddRequest2, personAddRequest3 };
            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_add_request in person_add_requests)
            {
                PersonResponse person_response = _personsService.AddPerson(person_add_request);
                person_response_list_from_add.Add(person_response);
            }

            // Print the list of PersonResponse objects added
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response.ToString());
            }

            //Act
            List<PersonResponse> person_response_list_from_search = _personsService.GetFilteredPersons(nameof(Person.PersonName), "oh");

            // Print the list of PersonResponse objects retrieved
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response in person_response_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response.ToString());
            }

            //Assert
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
        }

        #endregion

        #region GetSortedPersons
        [Fact]
        public void GetSortedPersons()
        {

            //Arrange
            CountryAddRequest country_add_request_1 = new CountryAddRequest()
            {
                CountryName = "canada"
            };
            CountryAddRequest country_add_request_2 = new CountryAddRequest()
            {
                CountryName = "usa"
            };

            CountryResponse country_response_1 = _countriesService.AddCountry(country_add_request_1);
            CountryResponse country_response_2 = _countriesService.AddCountry(country_add_request_2);

            PersonAddRequest personAddRequest1 = new PersonAddRequest()
            {
                PersonName = "John Doe",
                Email = "john@msn.cn",
                DateOfBirth = DateTime.Parse("2002-12-15"),
                Address = "123 Main St",
                CountryID = country_response_1.CountryID,
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true
            };
            PersonAddRequest personAddRequest2 = new PersonAddRequest()
            {
                PersonName = "Jane Smith",
                Email = "jane@msn.cn",
                DateOfBirth = DateTime.Parse("2001-08-20"),
                Address = "456 Oak Ave",
                CountryID = country_response_2.CountryID,
                Gender = GenderOptions.Female,
                ReceiveNewsLetters = false
            };
            PersonAddRequest personAddRequest3 = new PersonAddRequest()
            {
                PersonName = "Alice Johnson",
                Email = "alice@msn.cn",
                DateOfBirth = DateTime.Parse("2000-05-10"),
                Address = "789 Pine Rd",
                CountryID = country_response_1.CountryID,
                Gender = GenderOptions.Female,
                ReceiveNewsLetters = true
            };

            List<PersonAddRequest> person_add_requests = new List<PersonAddRequest>() { personAddRequest1, personAddRequest2, personAddRequest3 };
            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_add_request in person_add_requests)
            {
                PersonResponse person_response = _personsService.AddPerson(person_add_request);
                person_response_list_from_add.Add(person_response);
            }

            // Print the list of PersonResponse objects added
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response.ToString());
            }

            //Act
            List<PersonResponse> person_response_list_from_sort = _personsService.GetSortedPersons(_personsService.GetAllPersons(), nameof(Person.PersonName), SortOrderOptions.DESC);

            // Print the list of PersonResponse objects retrieved
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response in person_response_list_from_sort)
            {
                _testOutputHelper.WriteLine(person_response.ToString());
            }

            person_response_list_from_add = person_response_list_from_add.OrderByDescending(temp => temp.PersonName).ToList();
            //Assert
            for (int i = 0; i < person_response_list_from_add.Count; i++)
            {
                Assert.Equal(person_response_list_from_add[i], person_response_list_from_sort[i]);
            }
        }
        #endregion

        #region UpdatePerson

        // When we supply null value for PersonUpdateRequest parameter, it should throw ArgumentNullException
        [Fact]
        public void UpdatePerson_WhenNullUpdateRequestProvided_ShouldThrowArgumentNullException()
        {
            // Arrange
            PersonUpdateRequest? person_update_request = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _personsService.UpdatePerson(person_update_request));
        }

        // When we supply invalid PersonID in PersonUpdateRequest parameter, it should throw ArgumentException
        [Fact]
        public void UpdatePerson_WhenInvalidPersonIDProvided_ShouldThrowArgumentException()
        {
            // Arrange
            PersonUpdateRequest? person_update_request = new PersonUpdateRequest()
            {
                PersonID = Guid.NewGuid(), // Invalid PersonID
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _personsService.UpdatePerson(person_update_request));
        }

        // When PersonName is null in PersonUpdateRequest parameter, it should throw ArgumentException
        [Fact]
        public void UpdatePerson_WhenNullPersonNameProvided_ShouldThrowArgumentException()
        {
            // Arrange
            CountryAddRequest country_add_request = new CountryAddRequest()
            {
                CountryName = "canada"
            };
            CountryResponse country_response = _countriesService.AddCountry(country_add_request);

            PersonAddRequest person_add_request = new PersonAddRequest()
            {
                PersonName = "John Doe",
                Email = "john.doe@example.com",
                DateOfBirth = new DateTime(1990, 1, 1),
                Address = "123 Main St",
                CountryID = country_response.CountryID,
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true
            };
            PersonResponse person_response_from_add = _personsService.AddPerson(person_add_request);

            PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
            person_update_request.PersonName = null; // Invalid PersonName

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _personsService.UpdatePerson(person_update_request));

        }

        // When we supply valid PersonUpdateRequest parameter, it should update the person details and return updated PersonResponse object
        [Fact]
        public void UpdatePerson_WhenValidUpdateRequestProvided_ShouldUpdatePersonAndReturnUpdatedResponse()
        {
            // Arrange
            CountryAddRequest country_add_request = new CountryAddRequest()
            {
                CountryName = "canada"
            };
            CountryResponse country_response_from_add = _countriesService.AddCountry(country_add_request);

            PersonAddRequest person_add_request = new PersonAddRequest()
            {
                PersonName = "John Doe",
                Email = "john.doe@example.com",
                CountryID = country_response_from_add.CountryID,
                DateOfBirth = new DateTime(1990, 1, 1),
                Address = "123 Main St",
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true
            };
            PersonResponse person_response_from_add = _personsService.AddPerson(person_add_request);

            PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
            person_update_request.PersonName = "John Smith"; // Update PersonName
            person_update_request.Email = "john.smith@example.com"; // Update Email

            // Act
            PersonResponse person_response_from_update = _personsService.UpdatePerson(person_update_request);

            PersonResponse? person_response_from_get = _personsService.GetPersonByPersonID(person_response_from_add.PersonID);

            // Assert
            Assert.Equal(person_response_from_get, person_response_from_update);
        
        }

        #endregion

        #region DeletePerson
        // If you supply an valid person id, it should delete the person and return true
        [Fact]
        public void DeletePerson_ValidPersonID_ReturnsTrue()
        {
            //Arrange
            CountryAddRequest country_add_request = new CountryAddRequest()
            {
                CountryName = "canada"
            };
            CountryResponse country_response_from_add = _countriesService.AddCountry(country_add_request);
            PersonAddRequest person_add_request = new PersonAddRequest()
            {
                PersonName = "John Doe",
                Email = "john.doe@example.com",
                CountryID = country_response_from_add.CountryID,
                DateOfBirth = new DateTime(1990, 1, 1),
                Address = "123 Main St",
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true
            };
            PersonResponse person_response_from_add = _personsService.AddPerson(person_add_request);

            // Act
            bool delete_result = _personsService.DeletePerson(person_response_from_add.PersonID);

            // Assert
            Assert.True(delete_result);
        }


        // If you supply an invalid person id, it should return false
        [Fact]
        public void DeletePerson_InvalidPersonID_ReturnsFalse()
        {
            //Arrange
            Guid invalid_person_id = Guid.NewGuid();
            // Act
            bool delete_result = _personsService.DeletePerson(invalid_person_id);
            // Assert
            Assert.False(delete_result);
        }
        #endregion
    }
}
