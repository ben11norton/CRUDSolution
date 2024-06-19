using Microsoft.AspNetCore.Http;
using ServiceContracts.DTO;
using System.Threading.Tasks;

namespace ServiceContracts
{
    public interface ICountriesService
    {
        Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);

        Task<List<CountryResponse>> GetAllCountries();

        Task<CountryResponse?> GetCountryByCountryId(Guid? countryID);

        Task<int> UploadCountriesFromExcel(IFormFile formFile); 
        // may need to download Microsoft.AspNetCore.Http in nuget package manager
    }
}
