using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tienda.Models
{
    public class VentaViewModel:Ventas
    {
        public double TotalBruto { get; set; }

        public double TotalNeto { get; set; }

   
        public string Mensaje { get; set; }

    }
}