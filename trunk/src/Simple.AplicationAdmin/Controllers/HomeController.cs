using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Simple.ApplicationAdmin.Client;

namespace Simple.AplicationAdmin.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Manage";
            var client = new ApplicationManagerClient();
            //client.CreateApplication("App1");
            //client.CreateApplication("App2");
            //client.CreateApplication("App3");
            //Thread.Sleep(1000);
            var applications = client.GetApplications();
            return View(applications);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
