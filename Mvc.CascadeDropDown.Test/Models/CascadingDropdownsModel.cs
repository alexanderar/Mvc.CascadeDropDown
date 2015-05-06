using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvc.CascadeDropDown.Test.Models
{
    using System.Web.Mvc;

    public class CascadingDropdownsModel
    {
        public IList<SelectListItem> Countries { get; set; }

        public string SelectedCountry { get; set; }

        public string SelectedCity { get; set; }

        public string SelectedStreet { get; set; }

    }
}