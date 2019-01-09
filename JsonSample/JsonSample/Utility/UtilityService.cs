using System;
using System.Collections.Generic;
using JsonSample.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;

namespace JsonSample.Utility
{
    public class UtilityService
    {
        public static List<SelectListItem> CreateEuropeanCoutryListItem(List<string> europeanCountries)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            foreach (string country in europeanCountries)
            {
                listItems.Add(new SelectListItem
                {
                    Text = country,
                    Value = country
                });
            }

            return listItems;
        }

    }
}
