using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
   public class MediosPago
    {

        [KeyAttribute()]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MedioPagoId { get; set; }

        public string FormaPago { get; set; }

       
    }
}
