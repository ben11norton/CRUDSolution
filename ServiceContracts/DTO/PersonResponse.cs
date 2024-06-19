using System;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using Entities;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO
{
    public class PersonResponse
    {
        public Guid? PersonID { get; set; }
        public string? PersonName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public double? Age { get; set; }
        public string? Gender { get; set; }
        public Guid? CountryID { get; set; }
        public string? Country { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsLetters { get; set; }

        public override bool Equals(object? obj)
        {
            if(obj == null)
            {
                return false;
            }
            if(obj.GetType() != typeof(PersonResponse))
            {
                return false;
            }

            PersonResponse person = (PersonResponse)obj;

            return PersonID == person.PersonID &&
                PersonName == person.PersonName &&
                Email == person.Email &&
                DateOfBirth == person.DateOfBirth &&
                Age == person.Age &&
                Gender == person.Gender &&
                CountryID == person.CountryID &&
                Country == person.Country &&
                Address == person.Address &&
                ReceiveNewsLetters == person.ReceiveNewsLetters;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"Person ID: {PersonID}" +
                $"Person Name: {PersonName}" +
                $"Email: {Email}" +
                $"Date of Birth: {DateOfBirth}" +
                $"Age: {Age}" +
                $"Gender: {Gender}" +
                $"Country ID: {CountryID}," +
                $"Country: {Country}" +
                $"Address: {Address}" +
                $"Receive News Letter {ReceiveNewsLetters}";
        }

        public PersonUpdateRequest ToPersonUpdateRequest()
        {
            return new PersonUpdateRequest()
            {
                PersonID = PersonID,
                PersonName = PersonName,
                Email = Email,
                DateOfBirth = DateOfBirth, 
                Gender = (GenderOptions)Enum.Parse(typeof(GenderOptions), Gender, true),
                Address = Address,
                CountryID = CountryID,
                ReceiveNewsLetters = ReceiveNewsLetters
            };
        }
    }

    public static class PersonExtensions
    {
        
        public static PersonResponse ToPersonResponse(this Person person)
        {
            return new PersonResponse()
            {
                PersonID = person.PersonID,
                PersonName = person.PersonName,
                Email = person.Email,
                DateOfBirth = person.DateOfBirth,
                Gender = person.Gender,
                CountryID = person.CountryID,
                Address = person.Address,
                ReceiveNewsLetters = person.ReceiveNewsLetters,
                Age = (person.DateOfBirth != null) ? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays) : null,
                Country = person.Country?.CountryName
            };
        }
    }
}
