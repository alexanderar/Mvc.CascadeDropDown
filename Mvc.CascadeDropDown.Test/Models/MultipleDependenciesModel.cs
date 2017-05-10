using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvc.CascadeDropDown.Test.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class MultipleDependenciesModel
    {
        public IList<SelectListItem> Years { get; set; }

        public IList<SelectListItem> Makes { get; set; }

        [Required]
        public string SelectedYear { get; set; }

        [Required]
        public string SelectedMake { get; set; }

        [Required]
        public int? SelectedModel { get; set; }

    }
}