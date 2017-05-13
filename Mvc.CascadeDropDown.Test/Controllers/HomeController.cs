using System.Collections.Generic;
using System.Web.Mvc;
using Mvc.CascadeDropDown.Test.Models;

namespace Mvc.CascadeDropDown.Test.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult MultipleDependencies()
        {
            var model = new MultipleDependenciesModel
            {
                Makes = new List<SelectListItem> { new SelectListItem
                    {
                        Text = "Ferrari",
                        Value = "Ferrari"
                    },
                    new SelectListItem
                    {
                        Text = "Maserati",
                        Value = "Maserati"
                    },
                 },
                Years = new List<SelectListItem> { new SelectListItem
                    {
                        Text = "2016",
                        Value = "2016"
                    },
                    new SelectListItem
                    {
                        Text = "2017",
                        Value = "2017"
                    },
                 },
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult MultipleDependencies(MultipleDependenciesModel model)
        {
            return this.Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetModel(string make, string year)
        {
            var result = new List<SelectListItem>();
            for (int i = 0; i < 5; i++)
            {
                result.Add(new SelectListItem { Text = string.Format("{0} - {1} - {2}", make, year, i), Value = i.ToString() });
            }
            
            
            return  Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            var model = new CascadingDropdownsModel
            {
                Countries =
                    new List<SelectListItem>
                    {
                        new SelectListItem
                        {
                            Text = "US",
                            Value = "US"
                        },
                        new SelectListItem
                        {
                            Text = "UK",
                            Value = "UK"
                        },
                    },
                    SelectedCountry = "US",
                    SelectedCity = "New York",
                    //SelectedStreet = 2
            };
            return View(model);
        }

        public ActionResult GetCities(string country, string currency)
        {
            if (country == "US")
            {
                var cities = new List<SelectListItem>
                {
                    new SelectListItem {Text = "New York", Value = "New York"},
                    new SelectListItem {Text = "Los Angeles", Value = "Los Angeles", Disabled = true},
                    new SelectListItem {Text = "Boston", Value = "Boston", Selected = true}
                };
                return Json(cities, JsonRequestBehavior.AllowGet);
            }

            if (country == "UK")
            {
                var cities = new List<SelectListItem>
                {
                    new SelectListItem {Text = "London", Value = "London"},
                    new SelectListItem {Text = "Cambridge", Value = "Cambridge"},
                    new SelectListItem {Text = "Manchester - Return Error", Value = "Manchester"}
                };
                return Json(cities, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        public ActionResult GetStreets(string city)
        {
            var streets = new List<SelectListItem>
            {
                new SelectListItem {Text = city + " Street 1", Value = "1"},
                new SelectListItem {Text = city + " Street 2", Value = "2"},
                new SelectListItem {Text = city + " Street 3", Value = "3"}
            };

            if(city== "Manchester")
            {
                Response.StatusCode = 503;
                Response.StatusDescription = "Manchester Should Return Error";
                Response.Flush();
                return null;
            }
            else
            {
                return Json(streets, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult CascadingDropDownTest(CascadingDropdownsModel model)
        {
            if(!ModelState.IsValid)
            {
                return this.View(model);
            }
            return RedirectToAction("CascadingDropDownTest");
        }
    }
}