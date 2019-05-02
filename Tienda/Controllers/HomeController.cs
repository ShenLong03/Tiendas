using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tienda.Models;

namespace Tienda.Controllers
{
    public class HomeController : Controller
    {
        private DataContextLocal db = new DataContextLocal();

        public class Persona
        {
            public string Nombre { get; set; }
            public int Edad { get; set; }
        }

        public ActionResult Index()
        {
            

           
            return View();
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
    }
}