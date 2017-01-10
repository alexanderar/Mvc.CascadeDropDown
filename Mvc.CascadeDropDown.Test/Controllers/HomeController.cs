using System.Collections.Generic;
using System.Web.Mvc;
using Mvc.CascadeDropDown.Test.Models;

namespace Mvc.CascadeDropDown.Test.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View("PureJavaScriptTest");
        }

        public ActionResult CascadingDropDownTest()
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
                    //SelectedCountry = "US",
                    //SelectedCity = "New York",
                    //SelectedStreet = 2
            };
            return View(model);
        }

        public ActionResult GetCities(string country)
        {
            if (country == "US")
            {
                var cities = new List<SelectListItem>
                {
                    new SelectListItem {Text = "New York", Value = "New York"},
                    new SelectListItem {Text = "Los Angeles", Value = "Los Angeles"},
                    new SelectListItem {Text = "Boston", Value = "Boston"},
                    new SelectListItem {Text = "Boston Disabled", Value = "Boston Disabled", Disabled = true}
                };
                return Json(cities, JsonRequestBehavior.AllowGet);
            }

            if (country == "UK")
            {
                var cities = new List<SelectListItem>
                {
                    new SelectListItem {Text = "London", Value = "London"},
                    //new SelectListItem {Text = "Cambridge", Value = "Cambridge"},
                    //new SelectListItem {Text = "Manchester", Value = "Manchester"}
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
                //new SelectListItem {Text = city + " Street 2", Value = "2"},
                //new SelectListItem {Text = city + " Street 3", Value = "3"}
            };

            return Json(streets, JsonRequestBehavior.AllowGet);
        }
    }
}