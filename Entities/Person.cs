using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Entities
{
    /// <summary>
    /// Person domain model class
    /// </summary>
    public class Person
    {
        // Primary key for the Person entity
        [Key] 
        public Guid PersonID { get; set; }

        // Max length of 40 characters for PersonName
        [StringLength(40)] 
        public string? PersonName { get; set; }

        // Max length of 40 characters for Email
        [StringLength(40)] 
        public string? Email { get; set; }

        public DateTime? DateOfBirth { get; set; }

        // Max length of 10 characters for Gender
        [StringLength(10)] 
        public string? Gender { get; set; }

        // Foreign key to Country
        public Guid? CountryID { get; set; }

        // Max length of 200 characters for Address
        [StringLength(200)] 
        public string? Address { get; set; }

        // Indicates whether the person wants to receive newsletters
        public bool ReceiveNewsLetters { get; set; }
    }
}
