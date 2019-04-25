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

        public double Precio { get; set; } = 0;

        public int Cantidad { get; set; } = 1;

        public double Descuento { get; set; } = 0;

        public double Subtotal { get { return (Precio * Cantidad)-Descuento; } }

      

        public int ProductoId { get; set; }

        public int VentaId { get; set; }

        public virtual Productos Productos { get; set; }

        public virtual Ventas Ventas { get; set; }

        public DetalleVentas()
        {
            Precio = 0;
            Cantidad = 1;
            Descuento = 0;
        }
    }
}
