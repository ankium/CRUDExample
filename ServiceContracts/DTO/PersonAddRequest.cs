using System;
using System.Collections.Generic;
using System.Text;
using Entities;
using ServiceContracts.Enums;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// Act as a DTO for adding a new person
    /// </summary>
    public class PersonAddRequest
    {
        [Required(ErrorMessage = "Person Name can't be blank")]
        public string? PersonName { get; set; }

        [Required(ErrorMessage = "Please enter email")]
        [EmailAddress(ErrorMessage = "Email value should be a valid email")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Please select date of birth")]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Please select gender of the person")]
        public GenderOptions? Gender { get; set; }

        [Required(ErrorMessage = "Please select a country")]
        public Guid? CountryID { get; set; }

        [Required(ErrorMessage = "Please enter address")]
        public string? Address { get; set; }

        public bool ReceiveNewsLetters { get; set; }
        /// <summary>
        /// Converts the current PersonAddRequest object to a Person object
        /// </summary>
        /// <returns></returns>
        public Person ToPerson()
        {
            Person person = new Person()
            {
                PersonName = PersonName,
                Email = Email,
                DateOfBirth = DateOfBirth,
                Gender = Gender?.ToString(),
                CountryID = CountryID,
                Address = Address,
                ReceiveNewsLetters = ReceiveNewsLetters,
            };
            return person;
        }
    }
}
