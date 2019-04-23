using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tienda.Models
{
    public class PagoViewModel:Pagos
    {
        public double Vuelto { get; set; }

        public double TotalNeto { get; set; }
    }
}