using System;
using System.Collections.Generic;
using Entities;
using ServiceContracts.DTO;
using ServiceContracts;
using Services;
using Microsoft.EntityFrameworkCore;

namespace CRUDTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;

        public CountriesServiceTest()
        {
            _countriesService = new CountriesService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options));
        }

        #region AddCountry

        [Fact]
        public async Task AddCountry_NullCountry()
        {
            CountryAddRequest? request = null;

            await Assert.ThrowsAsync<ArgumentNullException>(async() =>
            {
                await _countriesService.AddCountry(request);
            });
            
        }

        [Fact]
        public async Task AddCountry_CountryNameIsNull()
        {
            CountryAddRequest? request = new CountryAddRequest()
            {
                CountryName = null
            };

            await Assert.ThrowsAsync<ArgumentException>(async() =>
            {
                await _countriesService.AddCountry(request);
            });

        }

        [Fact]
        public async Task AddCountry_DuplicateCountryName()
        {
            CountryAddRequest? request1 = new CountryAddRequest
            {
                CountryName = "USA"
            };
            CountryAddRequest? request2 = new CountryAddRequest
            {
                CountryName = "USA"
            };

            await Assert.ThrowsAsync<ArgumentException>(async() =>
            {
                await _countriesService.AddCountry(request1);
                await _countriesService.AddCountry(request2);
            });

        }

        [Fact]
        public async Task AddCountry_ProperCountryDetails()
        {
            // Arrange
            CountryAddRequest? request = new CountryAddRequest()
            {
                CountryName = "Japan"
            };

            // Act
            CountryResponse response = await _countriesService.AddCountry(request);
            List<CountryResponse> countries_from_GetAllCountries = await _countriesService.GetAllCountries();

            // Assert
            Assert.True(response.CountryID != Guid.Empty);
            Assert.Contains(response, countries_from_GetAllCountries);
        }

        #endregion


        #region GetAllCountries

        [Fact]
        public async Task GetAllCountries_EmptyList()
        {
            // Arrange
            List<CountryResponse> actual_country_response_list = await _countriesService.GetAllCountries();

            // Act
            Assert.Empty(actual_country_response_list);
        }

        [Fact]
        public async Task GetAllCountries_AddFewCountries()
        {
            // Arrange
            List<CountryAddRequest> country_request_list = new List<CountryAddRequest>()
            {
                new CountryAddRequest(){CountryName = "USA" },
                new CountryAddRequest(){CountryName = "UK" }
            };

            // Act
            List<CountryResponse> countries_list_from_add_country = new List<CountryResponse>();
            foreach(CountryAddRequest country_request in country_request_list)
            {
                countries_list_from_add_country.Add(await _countriesService.AddCountry(country_request));
            }

            // Assert
            List<CountryResponse> actualCountryResponseList = await _countriesService.GetAllCountries();
            foreach(CountryResponse expected_country in countries_list_from_add_country)
            {
                Assert.Contains(expected_country, actualCountryResponseList);
            }
        }
        #endregion


        #region GetCountryByCountryID

        [Fact]
        // if we supply null as CountryID, it should return null
        // as CountryResponse
        public async Task GetCountryByCountryID_NullCountryID()
        {
            // Arrange
            Guid? countryID = null;

            // Act
            CountryResponse? country_response_from_get_method = await _countriesService.GetCountryByCountryId(countryID);

            //Assert
            Assert.Null(country_response_from_get_method);
        }

        [Fact]
        // if we supply a valid country id, it should return the
        // matching country details as CountryResponse object
        public async Task GetCountryById_ValidCountryID()
        {
            // Arrange 
            CountryAddRequest? country_add_request = new CountryAddRequest() { CountryName = "China"};
            CountryResponse? country_response_from_add = await _countriesService.AddCountry(country_add_request);

            // Act
            CountryResponse? country_response_from_get = await _countriesService.GetCountryByCountryId(country_response_from_add.CountryID);

            // Assert
            Assert.Equal(country_response_from_add, country_response_from_get);
        }
        #endregion
    }
}
