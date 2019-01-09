using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using JsonSample.Models;
using JsonSample.Utility;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;

namespace JsonSample.Service
{
    public class PeopleDataService : IPeopleData
    {
        /*
         * saves json data of people 
         * checks if two different address fields are same,if same then data is not saved and returns a failure message
         * checks dupicate name,if exists then data is not saved and returns a failure message
         * checks duplicate addresses with all same field value, if number of duplicate address
         * is twice then data is not saved and returns a failure message
        */
        public string CreatePeople(People people)
        {
            try
            {
                string res = CheckMultipleAddressHasDuplicateFields(people);
                if(!res.Equals(""))
                {
                    return res;
                }

                PeopleList peopleList = JsonConvert.DeserializeObject<PeopleList>(GetPeopleJsonData());

                int duplicateNameCount = DuplicateNameCount(peopleList, people);
                if(duplicateNameCount >0)
                {
                    return "Duplicate Name Exists";
                }

                if (people.addresses != null)
                {
                    foreach (var enteredAddress in people.addresses)
                    {
                        int addressCount = DuplicateMultipleAddressFieldValueCount(peopleList, enteredAddress);

                        if (addressCount == 2)
                        {
                            return "Duplicate Address Field Exists";
                        }

                    }
                }
                else
                {
                    people.addresses = new List<Address>();
                    people.addresses.Add(new Address { });
                }
               
                FilterPeopleAddress(people);
                peopleList.peopleData.Add(people);

                return Save(peopleList);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception "+ex.Message);
                return "Failed to Save Data";
            }
        }

        public List<string> GetEuropeanCountries()
        {
            try
            {
                StreamReader reader;
                string jsondata;

                var assembly = Assembly.GetEntryAssembly();

                Stream resourceStream = assembly.GetManifestResourceStream("JsonSample.EuropeonCountries.json");

                using (reader = new StreamReader(resourceStream))
                {
                    jsondata = reader.ReadToEnd();
                    reader.Close();

                }

                CountryList countryList = JsonConvert.DeserializeObject<CountryList>(jsondata);

                return countryList.countries;
            }
            catch (Exception ex)
            {

                Console.WriteLine("Exception " + ex.Message);
                return null;
            }

        }

        public List<People> GetPeopleLists(string firstName, string lastName)
        {
            try
            {
                List<People> peopleData = JsonConvert.DeserializeObject<PeopleList>(GetPeopleJsonData()).peopleData;
                peopleData = peopleData.OrderBy(x=>x.firstName).ToList();
                if (!String.IsNullOrEmpty(firstName))
                {
                    peopleData = peopleData.Where(m => m.firstName.Trim().Equals(firstName.Trim(), StringComparison.CurrentCultureIgnoreCase)).OrderBy(x=>x.firstName).ToList();
                }

                if (!String.IsNullOrEmpty(lastName))
                {
                    peopleData = peopleData.Where(m => m.lastName.Trim().Equals(lastName.Trim(), StringComparison.CurrentCultureIgnoreCase)).OrderBy(x=>x.firstName).ToList();
                }


                return peopleData;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception " + ex.Message);
                return null;
            }

        }


        public string GetPeopleJsonData()
        {
            try
            {
                StreamReader reader;
                string jsondata;

                var assembly = Assembly.GetExecutingAssembly();

                string resourceName = assembly.GetManifestResourceNames()                                               .Single(str => str.EndsWith("JsonSample.PeopleData.json",StringComparison.CurrentCultureIgnoreCase));

                using( reader = new StreamReader(resourceName))
                {
                    jsondata = reader.ReadToEnd();
                   
                    reader.Close();

                }


                return jsondata; 
            }
            catch(Exception ex)
            {
                Console.WriteLine("Message "+ex.Message);
                return "";
            }
                      
        }

        public People GetPeople(string firstName, string lastName)
        {
            try
            {
                PeopleList peopleList = JsonConvert.DeserializeObject<PeopleList>(GetPeopleJsonData());

                List<People> peoples = peopleList.peopleData.Where(m => m.firstName.Trim().Equals(firstName.Trim(), StringComparison.CurrentCultureIgnoreCase)
                                                                   && m.lastName.Trim().Equals(lastName.Trim(), StringComparison.CurrentCultureIgnoreCase)).ToList();


                return peoples[0];

            }
            catch(Exception ex)
            {
                Console.WriteLine("Message "+ex.Message);
                return null;
            }
        }


        /*
         * saves json data of people 
         * checks if two different address fields are same,if same then data is not saved and returns a failure message
         * checks dupicate name,if exists then data is not saved and returns a failure message
         * checks duplicate addresses with all same field value, if number of duplicate address
         * is thrice then data is not saved and returns a failure message
        */
        public string UpdatePeople(People people,string firstName,string lastName)
        {
            people.Date_Of_Birth = people.Date_Of_Birth.ToUniversalTime();

            DateTime utcTime1 = new DateTime(people.Date_Of_Birth.Year, people.Date_Of_Birth.Month, people.Date_Of_Birth.Day, 0, 0, 0);
            people.Date_Of_Birth = DateTime.SpecifyKind(utcTime1, DateTimeKind.Utc);
            try
            {
                string res = CheckMultipleAddressHasDuplicateFields(people);

                if (!res.Equals(""))
                {
                    return res;
                }

                PeopleList peopleList = JsonConvert.DeserializeObject<PeopleList>(GetPeopleJsonData());

                //removing the old people data from the list
                peopleList.peopleData.RemoveAll(x => x.firstName.Trim().Equals(firstName.Trim(), StringComparison.CurrentCultureIgnoreCase)
                                               && x.lastName.Trim().Equals(lastName.Trim(), StringComparison.CurrentCultureIgnoreCase));
                
                peopleList.peopleData.Add(people);//adding the new entered people data in the list

                int duplicateNameCount = DuplicateNameCount(peopleList, people);

                if (duplicateNameCount > 1)
                {
                    return "Duplicate Name Exists";
                } 

                if(people.addresses!=null)
                {
                    foreach (var enteredAddress in people.addresses)
                    {
                        int addressCount = DuplicateMultipleAddressFieldValueCount(peopleList, enteredAddress);

                        if (addressCount == 3)
                        {
                            return "Duplicate Address Field Exists";
                        }

                    }
                }
                else
                {
                    people.addresses = new List<Address>();
                    people.addresses.Add(new Address { });  
                }


                FilterPeopleAddress(people);

                return Save(peopleList);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Message "+ex.Message);
                return "Failed To Update Data";
            }
        }

        public void FilterPeopleAddress(People people)
        {
            if(people.addresses!=null)
            {
                foreach (var address in people.addresses)
                {
                    if (address.line1 == null)
                    {
                        address.line1 = "";
                    }
                    if (address.line2 == null)
                    {
                        address.line2 = "";
                    }

                    if (address.country == null)
                    {
                        address.country = "";
                    }

                    if (address.postCode == null)
                    {
                        address.postCode = "";
                    }
                }
            }


        }

        public string DeletePeople(People people)
        {
            try
            {
                PeopleList peopleList = JsonConvert.DeserializeObject<PeopleList>(GetPeopleJsonData());

                //removing the people data from the list
                peopleList.peopleData.RemoveAll(x => x.firstName.Trim().Equals(people.firstName.Trim(), StringComparison.CurrentCultureIgnoreCase)
                                                && x.lastName.Trim().Equals(people.lastName.Trim(), StringComparison.CurrentCultureIgnoreCase));


                return Save(peopleList);

            }
            catch(Exception ex)
            {
                Console.WriteLine("Message "+ex.Message);
                return "Unable To Delete";
            }

        }

        public string CheckMultipleAddressHasDuplicateFields(People people){
            HashSet<Address> addressHashSet = new HashSet<Address>(new AddressComparer());
            if(people.addresses != null)
            {
                foreach (var address in people.addresses)
                {
                    if (!addressHashSet.Add(address))
                    {
                        return "Multiple Address Has Duplicate Fields Values";
                    }
                }
            }


            return "";
        }

       

        public int DuplicateNameCount(PeopleList peopleList,People people)
        {
            int count  = peopleList.peopleData.Where(m => m.firstName.Trim().Equals(people.firstName.Trim(), StringComparison.CurrentCultureIgnoreCase)
                                                     && m.lastName.Trim().Equals(people.lastName.Trim(), StringComparison.CurrentCultureIgnoreCase)).Count();

            return count;
        }

        public int DuplicateMultipleAddressFieldValueCount(PeopleList peopleList,Address address)
        {
            
            if(address.line2 == null)
            {
                address.line2 = "";
            }

            if(address.postCode == null)
            {
                address.postCode = "";
            }
            int count = peopleList.peopleData.Where(
                                a => a.addresses.Any(
                                    (
                        x =>x.line1!=null && x.line2!=null && x.postCode!=null && x.country!=null && x.country.Equals(address.country, StringComparison.CurrentCultureIgnoreCase)
                        && x.line1.Trim().Equals(address.line1.Trim(), StringComparison.CurrentCultureIgnoreCase)
                        &&  String.Equals(address.line2.Trim(), x.line2.Trim(), StringComparison.CurrentCultureIgnoreCase)
                        && x.country.Trim().Equals(address.country.Trim(), StringComparison.CurrentCultureIgnoreCase)
                        && String.Equals(address.postCode.Trim(), x.postCode.Trim(), StringComparison.CurrentCultureIgnoreCase)
                                    )
                                )).Count();

            return count;
                        
        }

        //saving the data in json and xml file
        public string Save(PeopleList peopleList)
        {
          
            StreamWriter writer;

            peopleList.peopleData.OrderBy(x => x.firstName).ToList();
            string jsonFormatData = JsonConvert.SerializeObject(peopleList);


            using (writer = new StreamWriter("JsonSample.PeopleData.json", append: false))
            {
                writer.WriteLine(jsonFormatData);
            }


            jsonFormatData = "{'?xml':{ '@version':'1.0','@standalone':'no'},'root':" +jsonFormatData.Replace("\"", "'")+"}";

            XmlDocument doc = (XmlDocument)JsonConvert.DeserializeXmlNode(jsonFormatData);


            using (writer = new StreamWriter("People_Data.xml", append: false)) 
            {
                writer.WriteLine(doc.OuterXml);

            }
            return "SUCCESS";
           
        }


    }
}
