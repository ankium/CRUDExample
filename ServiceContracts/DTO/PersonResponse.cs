using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// Represents DTO class that is used as retrun type of most methods of Persons Service
    /// </summary>
    public class PersonResponse
    {
        public Guid PersonID { get; set; }
        public string? PersonName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public Guid? CountryID { get; set; }
        public string? Country { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsLetters { get; set; }
        public double? Age { get; set; }
        /// <summary>
        /// Compares the current object data with the parameter object
        /// </summary>
        /// <param name="obj">The PersonResponse Ojbect to compare</param>
        /// <returns>True or false,indicating whether all person details are matched with the specified parameter object</returns>
        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(PersonResponse)) return false;
            PersonResponse personResponse = (PersonResponse)obj;
            return personResponse.PersonID == PersonID && personResponse.PersonName == PersonName &&
                personResponse.Email == Email && personResponse.DateOfBirth == DateOfBirth && personResponse.Gender == Gender && personResponse.CountryID == CountryID && personResponse.Address == Address && personResponse.ReceiveNewsLetters == ReceiveNewsLetters;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        override public string ToString()
        {
            return $"PersonID={PersonID}, PersonName={PersonName}, Email={Email}, DateOfBirth={DateOfBirth}, Gender={Gender}, CountryID={CountryID}, Address={Address}, ReceiveNewsLetters={ReceiveNewsLetters}";
        }

        public PersonUpdateRequest ToPersonUpdateRequest()
        {
            PersonUpdateRequest person_update_request = new PersonUpdateRequest()
            {
                PersonID = PersonID,
                PersonName = PersonName,
                Email = Email,
                DateOfBirth = DateOfBirth,
                Gender = (GenderOptions)Enum.Parse(typeof(GenderOptions),Gender,true),
                CountryID = CountryID,
                Address = Address,
                ReceiveNewsLetters = ReceiveNewsLetters
            };
            return person_update_request;
        }
    }

    public static class PersonExtensions
    {
        /// <summary>
        /// An extension method to convert an object of Person class to an object of PersonResponse class
        /// </summary>
        /// <param name="person">The Person object to convert</param>
        /// <returns>Return the converted PersonResponse object</returns>
        public static PersonResponse ToPersonResponse(this Entities.Person person)
        {
            //if (person == null) return null;
            PersonResponse personResponse = new PersonResponse()
            {
                PersonID = person.PersonID,
                PersonName = person.PersonName,
                Email = person.Email,
                DateOfBirth = person.DateOfBirth,
                ReceiveNewsLetters = person.ReceiveNewsLetters,
                Address = person.Address,
                CountryID = person.CountryID,
                Gender = person.Gender,
                Age= person.DateOfBirth.HasValue ? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25, 2) : null
            };
            return personResponse;
        }

    }
}
