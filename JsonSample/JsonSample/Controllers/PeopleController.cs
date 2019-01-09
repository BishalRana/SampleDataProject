using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JsonSample.Models;
using JsonSample.NotPageController;
using JsonSample.Service;
using JsonSample.Utility;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JsonSample.Controllers
{
    public class PeopleController : Controller
    {
        public PeopleController(IPeopleData _IPeopleData)
        {
            iPeopleData = _IPeopleData;
        }
        // GET: /<controller>/
        public IActionResult Index(string firstName,string lastName)
        {
            List<People> peopleData = iPeopleData.GetPeopleLists(firstName, lastName);
            PeopleList peopleViewModel = new PeopleList();
            peopleViewModel.peopleData = peopleData;
            return View(peopleViewModel);
        }

        public IActionResult Create()
        {
            People people = new People();
            people.addresses = new List<Address>();
            people.addresses.Add(new Address());

            ViewBag.europeanCountries = UtilityService.CreateEuropeanCoutryListItem(iPeopleData.GetEuropeanCountries());

            return View(people);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("firstName,lastName,Date_Of_Birth,nickname,country,addresses")] People people)
        {
            string res = null;
            if (ModelState.IsValid)
            {
                res = iPeopleData.CreatePeople(people);

                if(res.Equals("Success",StringComparison.CurrentCultureIgnoreCase))
                {
                    ViewBag.Message = "Successfully Added";
                    return RedirectToAction(nameof(Index));
                }
            }

            ViewBag.europeanCountries = UtilityService.CreateEuropeanCoutryListItem(iPeopleData.GetEuropeanCountries());
            ViewBag.Message = res;
            return View(people);
        }

        [HttpGet("People/Edit")]
        public IActionResult Edit(string fName,string lName)
        {
            People people = iPeopleData.GetPeople(fName,lName);
          
            if (people == null)
            {
                return new NotFoundViewResult("NotFound");
            }

            ViewBag.europeanCountries = UtilityService.CreateEuropeanCoutryListItem(iPeopleData.GetEuropeanCountries());

            return View(people);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string fName,string lName,[Bind("firstName,lastName,Date_Of_Birth,nickname,country,addresses")]
                                  People newPeople)
        {         
            String res = null;

            if (ModelState.IsValid)
            {
                res = iPeopleData.UpdatePeople(newPeople,fName,lName);

                if (res.Equals("Success", StringComparison.CurrentCultureIgnoreCase))
                {
                    //return View("Index");
                    return RedirectToAction("Index");
                }
            }

            ViewBag.Message = res;
            ViewBag.europeanCountries = UtilityService.CreateEuropeanCoutryListItem(iPeopleData.GetEuropeanCountries());

            return View(newPeople);
        }

        [HttpGet("People/Delete")]
        public IActionResult Delete(string fName,string lName)
        {
            People people = iPeopleData.GetPeople(fName, lName);

            if(people == null)
            {
                return new NotFoundViewResult("NotFound");
            }
            ViewBag.europeanCountries = UtilityService.CreateEuropeanCoutryListItem(iPeopleData.GetEuropeanCountries());

            return View(people);
        }

        [HttpPost,ActionName("Delete")]
        public IActionResult DeleteConfirmed(string fName,string lName)
        {
            People people = iPeopleData.GetPeople(fName, lName);

            string res = iPeopleData.DeletePeople(people);
            if (res.Equals("Success", StringComparison.CurrentCultureIgnoreCase))
            {
                ViewBag.Message = "Successfully Deleted";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Message = res;
            return View(people);
        }

        private IPeopleData iPeopleData;

    }
}
