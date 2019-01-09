using System;
using System.ComponentModel.DataAnnotations;

namespace JsonSample.Models
{
    public class Address
    {
        [Required]
        [Display(Name = "Line1")]
        public string line1 { get; set; }

        [Display(Name ="Line2")]
        [RegularExpression("^[^0-9]{1,}$", ErrorMessage = "Line 2 can not contain any numbers")]
        public string line2 { get; set; }

        [Required]
        [Display(Name ="Country")]
        public string country { get; set; }

        [Display(Name = "Post Code")]
        [RegularExpression("^[a-zA-Z0-9 ]{1,}$",ErrorMessage ="Post Code can not contain special symbols or characters")]
        public string postCode { get; set; }

        public string Index { get; set; }
    }
}
