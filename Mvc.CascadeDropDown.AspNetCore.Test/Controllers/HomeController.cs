// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HomeController.cs" company="">
//   
// </copyright>
// <summary>
//   The home controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Mvc.CascadeDropDown.AspNetCore.Test.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    using Mvc.CascadeDropDown.AspNetCore.Test.Models;
    using Mvc.CascadeDropDown.Test.Models;

    using ActionResult = Microsoft.AspNetCore.Mvc.ActionResult;
    using Controller = Microsoft.AspNetCore.Mvc.Controller;
    using SelectListItem = Microsoft.AspNetCore.Mvc.Rendering.SelectListItem;

    /// <summary>
    /// The home controller.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// The cascading drop down test.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public ActionResult CascadingDropDownTest(CascadingDropdownsModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            return this.RedirectToAction("CascadingDropDownTest");
        }

        /// <summary>
        /// The get cities.
        /// </summary>
        /// <param name="country">
        /// The country.
        /// </param>
        /// <param name="currency">
        /// The currency.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult GetCities([FromBody]GetCitiesModel model)
        {
            if (model.Country == "US")
            {
                var cities = new List<SelectListItem>
                                 {
                                     new SelectListItem { Text = "New York", Value = "New York" },
                                     new SelectListItem { Text = "Los Angeles", Value = "Los Angeles", Disabled = true },
                                     new SelectListItem { Text = "Boston", Value = "Boston", Selected = true }
                                 };
                return this.Json(cities);
            }

            if (model.Country == "UK")
            {
                var cities = new List<SelectListItem>
                                 {
                                     new SelectListItem { Text = "London", Value = "London" },
                                     new SelectListItem { Text = "Cambridge", Value = "Cambridge" },
                                     new SelectListItem { Text = "Manchester - Return Error", Value = "Manchester" }
                                 };
                return this.Json(cities);
            }

            return new BadRequestResult();
        }

        /// <summary>
        /// The get model.
        /// </summary>
        /// <param name="make">
        /// The make.
        /// </param>
        /// <param name="year">
        /// The year.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult GetModel(string make, string year)
        {
            var result = new List<SelectListItem>();
            for (var i = 0; i < 5; i++)
            {
                result.Add(new SelectListItem { Text = string.Format("{0} - {1} - {2}", make, year, i), Value = i.ToString() });
            }

            return this.Json(result);
        }

        /// <summary>
        /// The get streets.
        /// </summary>
        /// <param name="city">
        /// The city.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public async Task<ActionResult> GetStreets(string city)
        {
            var streets = new List<SelectListItem>
                              {
                                  new SelectListItem { Text = city + " Street 1", Value = "1" },
                                  new SelectListItem { Text = city + " Street 2", Value = "2" },
                                  new SelectListItem { Text = city + " Street 3", Value = "3" }
                              };

            if (city == "Manchester")
            {
                return new UnauthorizedResult();
            }

            return this.Json(streets);
        }

        /// <summary>
        /// The index.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult Index()
        {
            var model = new CascadingDropdownsModel
                            {
                                Countries =
                                    new List<SelectListItem>
                                        {
                                            new SelectListItem { Text = "US", Value = "US" },
                                            new SelectListItem { Text = "UK", Value = "UK" }
                                        },
                                SelectedCountry = "US",
                                SelectedCity = "New York"

                                // SelectedStreet = 2
                            };
            return this.View(model);
        }

        /// <summary>
        /// The multiple dependencies.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult MultipleDependencies()
        {
            var model = new MultipleDependenciesModel
                            {
                                Makes =
                                    new List<SelectListItem>
                                        {
                                            new SelectListItem { Text = "Ferrari", Value = "Ferrari" },
                                            new SelectListItem { Text = "Maserati", Value = "Maserati" }
                                        },
                                Years = new List<SelectListItem>
                                            {
                                                new SelectListItem { Text = "2016", Value = "2016" },
                                                new SelectListItem { Text = "2017", Value = "2017" }
                                            }
                            };
            return this.View(model);
        }

        /// <summary>
        /// The multiple dependencies.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public ActionResult MultipleDependencies(MultipleDependenciesModel model)
        {
            return this.Json(model);
        }
    }
}