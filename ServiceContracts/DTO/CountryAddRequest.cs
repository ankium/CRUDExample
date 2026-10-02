using System;
using System.Collections.Generic;
using System.Text;
using Entities;
namespace ServiceContracts.DTO
{
    /// <summary>
    /// DTO class for adding a new country. It contains properties that represent the data required to create a new country entity.
    /// </summary>
    public class CountryAddRequest
    {
        public string? CountryName { get; set; }
        public Country ToCountry()
        {
            return new Country()
            {
                CountryName = this.CountryName
            };
        }
    }
}
