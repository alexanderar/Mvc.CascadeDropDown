using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvc.CascadeDropDown.Test.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class CascadingDropdownsModel
    {
        public IList<SelectListItem> Countries { get; set; }

        [Required]
        public string SelectedCountry { get; set; }

        [Required]
        public string SelectedCity { get; set; }

        [Required]
        public int? SelectedStreet { get; set; }

    }
}