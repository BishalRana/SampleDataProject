using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JsonSample.Models
{
    public class People
    {
       
        [Required]
        [Display(Name = "First Name")]
        [RegularExpression("^[^0-9]{1,}$",ErrorMessage ="First Name can not conatin any numbers")]
        public string firstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [RegularExpression("^[^0-9]{1,}$",ErrorMessage = "Last Name can not contain any numbers")]
        public string lastName { get; set; }

        [Required]
        [Display(Name = "Date Of Birth"), DataType(DataType.Date)]
        public DateTime Date_Of_Birth { get; set; }

        [Display(Name = "Nick Name")]
        [RegularExpression("^[^0-9]{1,}$", ErrorMessage = "Nick Name can not contain any numbers")]
        public string nickname { get; set; }

        public List<Address> addresses { get; set; }
    }
}
