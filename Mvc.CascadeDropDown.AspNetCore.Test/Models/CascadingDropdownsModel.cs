// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CascadingDropdownsModel.cs" company="">
//   
// </copyright>
// <summary>
//   The cascading dropdowns model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Mvc.CascadeDropDown.AspNetCore.Test.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Mvc.Rendering;

    /// <summary>
    /// The cascading dropdowns model.
    /// </summary>
    public class CascadingDropdownsModel
    {
        /// <summary>
        /// Gets or sets the countries.
        /// </summary>
        public IList<SelectListItem> Countries { get; set; }

        /// <summary>
        /// Gets or sets the selected city.
        /// </summary>
        [Required]
        public string SelectedCity { get; set; }

        /// <summary>
        /// Gets or sets the selected country.
        /// </summary>
        [Required]
        public string SelectedCountry { get; set; }

        /// <summary>
        /// Gets or sets the selected street.
        /// </summary>
        [Required]
        public int? SelectedStreet { get; set; }
    }
}