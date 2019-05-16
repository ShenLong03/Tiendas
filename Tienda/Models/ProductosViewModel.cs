using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tienda.Models
{
    public class ProductosViewModel:Productos
    {
        public List<Categorias> GetCategorias { get; set; }
    }
}