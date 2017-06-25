// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultipleDependenciesModel.cs" company="">
//   
// </copyright>
// <summary>
//   The multiple dependencies model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Mvc.CascadeDropDown.Test.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Mvc.Rendering;

    /// <summary>
    /// The multiple dependencies model.
    /// </summary>
    public class MultipleDependenciesModel
    {
        /// <summary>
        /// Gets or sets the makes.
        /// </summary>
        public IList<SelectListItem> Makes { get; set; }

        /// <summary>
        /// Gets or sets the selected make.
        /// </summary>
        [Required]
        public string SelectedMake { get; set; }

        /// <summary>
        /// Gets or sets the selected model.
        /// </summary>
        [Required]
        public int? SelectedModel { get; set; }

        /// <summary>
        /// Gets or sets the selected year.
        /// </summary>
        [Required]
        public string SelectedYear { get; set; }

        /// <summary>
        /// Gets or sets the years.
        /// </summary>
        public IList<SelectListItem> Years { get; set; }
    }
}