using ServiceContracts.DTO;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Country entities. This interface defines the contract for services that handle operations related to countries, such as adding, retrieving, updating, and deleting country records.
    /// </summary>
    public interface ICountriesService
    {
        /// <summary>
        /// Adds a country object to the list of countries.This method takes a CountryAddRequest object as input, which contains the necessary information to create a new country record. It returns a CountryResponse object that represents the newly added country, including its unique identifier and name.
        /// </summary>
        /// <param name="countryAddRequest">Country object to add</param>
        /// <returns>Returns the country object after adding it (including newly generated country id)</returns>
        CountryResponse AddCountry(CountryAddRequest? countryAddRequest);

        /// <summary>
        /// Returns all countries from the list of countries. This method retrieves a list of all country records currently stored in the system. It returns a list of CountryResponse objects, each representing a country with its unique identifier and name.
        /// </summary>
        /// <returns></returns>
        List<CountryResponse> GetAllCountries();

        /// <summary>
        /// Returns a country object from the list of countries based on the provided country ID. 
        /// </summary>
        /// <param name="countryID">This method takes a nullable Guid parameter representing the unique identifier of the country to retrieve.</param>
        /// <returns>If a country with the specified ID exists, it returns a CountryResponse object representing that country; otherwise, it returns null.</returns>
        CountryResponse? GetCountryByCountryID(Guid? countryID);

    }
}
