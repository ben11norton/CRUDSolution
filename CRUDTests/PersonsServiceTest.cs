﻿using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Entities;
using Services;
using Xunit.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personService;
        private readonly ICountriesService _countriesService;
        private readonly ITestOutputHelper _testOutputHelper;
        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {

            _countriesService = new CountriesService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options));
            _personService = new PersonsService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options), _countriesService);

            _testOutputHelper = testOutputHelper;
        }

        #region AddPerson

        [Fact]
        public async Task AddPerson_NullPerson()
        {
            // Arrange
            PersonAddRequest? personAddRequest = null;
             
            // Act
            await Assert.ThrowsAsync<ArgumentNullException>(async() =>
            {
                await _personService.AddPerson(personAddRequest);
            });
        }

        [Fact]
        public async Task AddPerson_PersonNameIsNull()
        {
            // Arrange
            PersonAddRequest? personAddRequest = new PersonAddRequest()
            {
                PersonName = null
            };
            // Act
            await Assert.ThrowsAsync<ArgumentException>(async() =>
            {
                await _personService.AddPerson(personAddRequest);
            });
        }

        [Fact]
        public async Task AddPerson_ProperPersonDetails()
        {
            // Arrange
            PersonAddRequest? personAddRequest = new PersonAddRequest()
            {
                PersonName = "Person Name", 
                Email = "name@example.com",
                Address = "sample address",
                CountryID = Guid.NewGuid(),
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                ReceiveNewsLetters = true

            };

            // Act
            PersonResponse person_response_from_add = await _personService.AddPerson(personAddRequest);
            List<PersonResponse> persons_list = await _personService.GetAllPersons();

            // Assert
            Assert.True(person_response_from_add.PersonID != Guid.Empty);
            Assert.Contains(person_response_from_add, persons_list);
        }

        #endregion


        #region GetPersonByPersonID

        [Fact]
        public async Task GetPersonByPersonID_NullPersonID()
        {
            // Arrange
            Guid? personID = null;

            // Act
            PersonResponse? person_response_from_get = await _personService.GetPersonByPersonID(personID);

            // Assert
            Assert.Null(personID);
        }

        [Fact]
        public async Task GetPErsonByPersonID_WithPersonID()
        {
            // Arrange
            CountryAddRequest country_request = new CountryAddRequest(){ CountryName = "Canada" };
            CountryResponse country_response = await _countriesService.AddCountry(country_request);

            // Act
            PersonAddRequest person_request = new PersonAddRequest()
            {
                PersonName = "person Name",
                Email = "email@sample.com",
                Address = "address",
                CountryID = country_response.CountryID,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                Gender = GenderOptions.Female,
                ReceiveNewsLetters = false
            };

            PersonResponse person_response_from_add = await _personService.AddPerson(person_request);

            PersonResponse? person_response_from_get = await _personService.GetPersonByPersonID(person_response_from_add.PersonID);

            // Assert
            Assert.Equal(person_response_from_add, person_response_from_get);
        }

        #endregion


        #region GetAllPersons

        [Fact]
        public async Task GetAllPerson_EmptyList()
        {
            // Arrange
            List<PersonResponse> persons_from_get = await _personService.GetAllPersons();

            // Assert
            Assert.Empty(persons_from_get);
        }

        [Fact]
        public async Task GetAllPerson_AddFewPersons()
        {
            // Arrange
            CountryAddRequest country_request_1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest country_request_2 = new CountryAddRequest() { CountryName = "India" };
            CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = new PersonAddRequest()
            {
                PersonName = "Smith",
                Email = "Smith@example.com",
                Gender = GenderOptions.Male,
                Address = "address of Smith",
                CountryID = country_response_1.CountryID,
                DateOfBirth = DateTime.Parse("2002-05-06"),
                ReceiveNewsLetters = true
            };
            PersonAddRequest person_request_2 = new PersonAddRequest()
            {
                PersonName = "Norton",
                Email = "Norton@example.com",
                Gender = GenderOptions.Male,
                Address = "address of Norton",
                CountryID = country_response_2.CountryID,
                DateOfBirth = DateTime.Parse("2001-02-16"),
                ReceiveNewsLetters = false
            };

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>()
            {
                person_request_1,
                person_request_2
            };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach(PersonAddRequest personRequest in person_requests)
            {
                PersonResponse person_response = await _personService.AddPerson(personRequest);
                person_response_list_from_add.Add(person_response);
            }

            _testOutputHelper.WriteLine("Expected: ");
            foreach(PersonResponse person_response_from_add in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            // Act
            List<PersonResponse> person_list_from_get = await _personService.GetAllPersons();

            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse person_response_from_get in person_list_from_get)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            // Assert
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                Assert.Contains(person_response_from_add, person_list_from_get);
            }
        }

        #endregion


        #region GetFilteredPersons

        [Fact]
        public async Task GetFilteredPersons_EmptySearchText()
        {
            // Arrange
            CountryAddRequest country_request_1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest country_request_2 = new CountryAddRequest() { CountryName = "India" };
            CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = new PersonAddRequest()
            {
                PersonName = "Smith",
                Email = "Smith@example.com",
                Gender = GenderOptions.Male,
                Address = "address of Smith",
                CountryID = country_response_1.CountryID,
                DateOfBirth = DateTime.Parse("2002-05-06"),
                ReceiveNewsLetters = true
            };
            PersonAddRequest person_request_2 = new PersonAddRequest()
            {
                PersonName = "Norton",
                Email = "Norton@example.com",
                Gender = GenderOptions.Male,
                Address = "address of Norton",
                CountryID = country_response_2.CountryID,
                DateOfBirth = DateTime.Parse("2001-02-16"),
                ReceiveNewsLetters = false
            };

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>()
            {
                person_request_1,
                person_request_2
            };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest personRequest in person_requests)
            {
                PersonResponse person_response = await _personService.AddPerson(personRequest);
                person_response_list_from_add.Add(person_response);
            }

            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            // Act
            List<PersonResponse> person_list_from_search = await _personService.GetFilteredPersons(nameof(Person.PersonName), "");

            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse person_response_from_get in person_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            // Assert
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                Assert.Contains(person_response_from_add, person_list_from_search);
            }
        }


        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName()
        {
            // Arrange
            CountryAddRequest country_request_1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest country_request_2 = new CountryAddRequest() { CountryName = "India" };
            CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = new PersonAddRequest()
            {
                PersonName = "Smith",
                Email = "Smith@example.com",
                Gender = GenderOptions.Male,
                Address = "address of Smith",
                CountryID = country_response_1.CountryID,
                DateOfBirth = DateTime.Parse("2002-05-06"),
                ReceiveNewsLetters = true
            };
            PersonAddRequest person_request_2 = new PersonAddRequest()
            {
                PersonName = "Norton",
                Email = "Norton@example.com",
                Gender = GenderOptions.Male,
                Address = "address of Norton",
                CountryID = country_response_2.CountryID,
                DateOfBirth = DateTime.Parse("2001-02-16"),
                ReceiveNewsLetters = false
            };

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>()
            {
                person_request_1,
                person_request_2
            };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest personRequest in person_requests)
            {
                PersonResponse person_response = await _personService.AddPerson(personRequest);
                person_response_list_from_add.Add(person_response);
            }

            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            // Act
            List<PersonResponse> person_list_from_search = await _personService.GetFilteredPersons(nameof(Person.PersonName), "no");

            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse person_response_from_get in person_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            // Assert
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                if (person_response_from_add != null)
                {
                    if(person_response_from_add.PersonName.Contains("no", StringComparison.OrdinalIgnoreCase))
                    {
                        Assert.Contains(person_response_from_add, person_list_from_search);
                    }
                }
            }
        }

        #endregion


        #region GetSortedPersons

        [Fact]
        public async Task GetSortedPersons()
        {
            // Arrange
            CountryAddRequest country_request_1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest country_request_2 = new CountryAddRequest() { CountryName = "India" };
            CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = new PersonAddRequest()
            {
                PersonName = "Smith",
                Email = "Smith@example.com",
                Gender = GenderOptions.Male,
                Address = "address of Smith",
                CountryID = country_response_1.CountryID,
                DateOfBirth = DateTime.Parse("2002-05-06"),
                ReceiveNewsLetters = true
            };
            PersonAddRequest person_request_2 = new PersonAddRequest()
            {
                PersonName = "Norton",
                Email = "Norton@example.com",
                Gender = GenderOptions.Male,
                Address = "address of Norton",
                CountryID = country_response_2.CountryID,
                DateOfBirth = DateTime.Parse("2001-02-16"),
                ReceiveNewsLetters = false
            };

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>()
            {
                person_request_1,
                person_request_2
            };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest personRequest in person_requests)
            {
                PersonResponse person_response = await _personService.AddPerson(personRequest);
                person_response_list_from_add.Add(person_response);
            }

            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            // Act
            List<PersonResponse> allPersons = await _personService.GetAllPersons();
            List<PersonResponse> person_list_from_sort = await _personService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse person_response_from_get in person_list_from_sort)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }
            person_response_list_from_add = person_response_list_from_add.OrderByDescending(temp => temp.PersonName).ToList();

            // Assert
            for(int i = 0; i < person_response_list_from_add.Count; i++)
            {
                Assert.Equal(person_response_list_from_add[i], person_list_from_sort[i]);
            }
        }

        #endregion


        #region UpdatePerson

        [Fact]
        public async Task UpdatePerson_NullPerson()
        {
            // Arrange
            PersonUpdateRequest? person_update_request = null;

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async() =>
            {
                // Act 
                await _personService.UpdatePerson(person_update_request);
            });
        }


        [Fact]
        public async Task UpdatePerson_InvalidID()
        {
            // Arrange
            PersonUpdateRequest? person_update_request = new PersonUpdateRequest()
            {
                PersonID = Guid.NewGuid()
            };

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(async() =>
            {
                // Act 
                await _personService.UpdatePerson(person_update_request);
            });
        }


        [Fact]
        public async Task UpdatePerson_PersonNameIsNull()
        {
            // Arrange
            CountryAddRequest country_add_request = new CountryAddRequest()
            {
                CountryName = "UK"
            };
            CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);

            PersonAddRequest person_add_request = new PersonAddRequest()
            {
                PersonName = "John",
                CountryID = country_response_from_add.CountryID,
                Email = "john@example.com",
                Address = "address",
                Gender = GenderOptions.Male
            };
            PersonResponse person_response_from_add = await _personService.AddPerson(person_add_request);

            PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
            person_update_request.PersonName = null;

            // Assert and Act 
            await Assert.ThrowsAsync<ArgumentException>(async() =>
            {
                await _personService.UpdatePerson(person_update_request);
            });
        }


        [Fact]
        public async Task UpdatePerson_PersonFullDetails()
        {
            // Arrange
            CountryAddRequest country_add_request = new CountryAddRequest()
            {
                CountryName = "UK"
            };
            CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);

            PersonAddRequest person_add_request = new PersonAddRequest()
            {
                PersonName = "John",
                CountryID = country_response_from_add.CountryID,
                Address = " ABc road",
                DateOfBirth = DateTime.Parse("2002-01-01"),
                Email = "abc@example.com",
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true
            };
            PersonResponse person_response_from_add = await _personService.AddPerson(person_add_request);

            PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
            person_update_request.PersonName = "William";
            person_update_request.Email = "william@example.com";

            // Act
            PersonResponse person_response_from_update = await _personService.UpdatePerson(person_update_request);
            PersonResponse? person_response_from_get = await _personService.GetPersonByPersonID(person_response_from_update.PersonID);

            // Assert 
            Assert.Equal(person_response_from_get, person_response_from_update);
        }

        #endregion


        #region DeletePerson

        [Fact]
        public async Task DeletePerson_ValidPersonID()
        {
            // Arrange
            CountryAddRequest country_add_request = new CountryAddRequest()
            {
                CountryName = "USA"
            };
            CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);

            PersonAddRequest person_add_request = new PersonAddRequest()
            {
                PersonName = "Jones",
                Address = "address",
                CountryID = country_response_from_add.CountryID,
                DateOfBirth = DateTime.Parse("2010-01-01"),
                Email = "jones@example.com",
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true
            };

            PersonResponse person_response_from_add = await _personService.AddPerson(person_add_request);

            // Act
            bool isDeleted = await _personService.DeletePerson(person_response_from_add.PersonID);

            // Assert
            Assert.True(isDeleted);
        }



        [Fact]
        public async Task DeletePerson_InvalidPersonID()
        {
            // Act
            bool isDeleted = await _personService.DeletePerson(Guid.NewGuid());

            // Assert
            Assert.False(isDeleted);
        }

        #endregion
    }
}
