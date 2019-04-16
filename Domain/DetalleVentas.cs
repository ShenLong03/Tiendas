using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
   public class DetalleVentas
    {
        [Key]
        public int DetalleVentasId { get; set; }

        public double Precio { get; set; }

        public int Cantidad { get; set; }

        public double Descuento { get; set; }

        public double Subtotal { get; set; }

        public int ProductoId { get; set; }

        public int VentaId { get; set; }


        public virtual ICollection<Productos> Productos { get; set; }

       


        public virtual Ventas Ventas { get; set; }
    }
}
