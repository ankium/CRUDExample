using Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryContracts
{
    public interface ICountriesRepository
    {
        /// <summary>
        /// Adds a new country object to the data store
        /// </summary>
        /// <param name="country">Country object to add</param>
        /// <returns>Returns the country object after adding it to the data store</returns>
        Task<Country> AddCountry(Country country);

        /// <summary>
        /// Returns all countries in the data store
        /// </summary>
        /// <returns>All countries from the table</returns>
        Task<List<Country>> GetAllCountries();

        /// <summary>
        /// Returns a country object based on the given country id; otherwise, it return null
        /// </summary>
        /// <param name="countryId">countryId to search</param>
        /// <returns></returns>
        Task<Country?> GetCountryByCountryId(Guid countryId);

        /// <summary>
        /// Returns a country object based on the given country name
        /// </summary>
        /// <param name="countryName">countryName to search</param>
        /// <returns></returns>
        Task<Country?> GetCountryByCountryName(string countryName);
    }
}
