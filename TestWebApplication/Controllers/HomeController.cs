using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrderWebApplication.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Displays the home page.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
    }
}