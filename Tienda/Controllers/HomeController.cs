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
            var Producto = db.Productos.Where(p => p.ProductoId == 1).ToList();
            foreach (var item in Producto)
            {
                Productos productos = new Productos();
                productos.CodigoId = item.CodigoId;
                productos.Cantidad = item.Cantidad;
                productos.Descripcion = item.Descripcion;
                productos.DetalleVentas = item.DetalleVentas;
                productos.Precio = item.Precio;
                productos.ProductoId = item.ProductoId;
                productos.TallaId = item.TallaId;
                productos.Tallas = item.Tallas;
                return Json(productos, JsonRequestBehavior.AllowGet);
            }
               

        

            var persona = new Persona() { Nombre = "Brainer", Edad = 25 };



           
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