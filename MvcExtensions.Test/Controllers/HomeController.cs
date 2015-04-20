using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcExtensions.Test.Models;

namespace MvcExtensions.Test.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View("PureJavaScriptTest");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
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

            };
            return this.View(model);
        }

        public ActionResult GetCities(string country)
        {

            if (country == "US")
            {
                var cities = new List<SelectListItem>
                                 {
                                     new SelectListItem { Text = "New York", Value = "New York" },
                                     new SelectListItem { Text = "Los Angeles", Value = "Los Angeles" },
                                     new SelectListItem { Text = "Boston", Value = "Boston" }
                                 };
                return Json(cities, JsonRequestBehavior.AllowGet);
            }

            if (country == "UK")
            {
                var cities = new List<SelectListItem>
                                 {
                                     new SelectListItem { Text = "London", Value = "London" },
                                     new SelectListItem { Text = "Cambridge", Value = "Cambridge" },
                                     new SelectListItem { Text = "Manchester", Value = "Manchester" }
                                 };
                return Json(cities, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        public ActionResult GetStreets(string city)
        {

            var streets = new List<SelectListItem>
                                 {
                                     new SelectListItem { Text = city +" Street 1", Value = city +" Street 1" },
                                     new SelectListItem { Text = city +" Street 2", Value = city +" Street 2" },
                                     new SelectListItem { Text = city +" Street 3", Value = city +" Street 3" }
                                 };

            return Json(streets, JsonRequestBehavior.AllowGet);

        }
    }
}