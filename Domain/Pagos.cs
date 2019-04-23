using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
   public class Pagos
    {
        [Key]
        public int PagoId { get; set; }

        public DateTime Fecha { get; set; }

        public double Monto { get; set; } = 0;

        public int? ClienteId { get; set; }

        public int MedioPagoId { get; set; }

        public int VentaId { get; set; }


        public virtual MediosPago MediosPago { get; set; }


        public virtual Ventas Ventas { get; set; }

        public virtual Cliente Cliente { get; set; }

        public Pagos()
        {
            Fecha = DateTime.Today;
            Monto = 0;

        }

    }
}
