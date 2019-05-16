using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tienda.Models
{
    public class DetalleVentaViewModel:DetalleVentas
    {
        public List<Productos>GetProductos{ get; set; }
     
        public  string Foto { get; set; }

    }
}