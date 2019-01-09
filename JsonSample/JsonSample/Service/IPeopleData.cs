using System;
using System.Collections.Generic;
using JsonSample.Models;
using Newtonsoft.Json.Linq;

namespace JsonSample.Service
{
    public interface IPeopleData
    {
        List<string> GetEuropeanCountries();
        List<People> GetPeopleLists(string firstName,string lastName);
        string CreatePeople(People people);
        People GetPeople(string firstName, string lastName);
        string UpdatePeople(People people,string firstName,string lastName);
        string DeletePeople(People people);
    }
}
