using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
   public class Ventas
    {
        [Key]
        public int VentaId { get; set; }

     
        public DateTime Fecha { get; set; }

        public double TotalOrden { get; set; } = 0;

        public double CantidadPagada { get; set; } = 0;

        public double CantidadPendiente { get; set; } = 0;

        public int? ClienteId { get; set; }

        public string Nombre { get; set; }

       

        public virtual ICollection<DetalleVentas> DetalleVentas { get; set; }

       

        public virtual Cliente Cliente { get; set; }


        public virtual ICollection<Pagos> Pagos { get; set; }

        public Ventas()
        {
            Fecha = DateTime.Today;
            TotalOrden = 0;


        }

    }
}
