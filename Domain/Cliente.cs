using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class Cliente
    {
        [Key]
        public int ClienteId { get; set; }

        public string Nombre { get; set; } = "";
        public string Cedula { get; set; } = "";
        public string TelefonoPrincipal { get; set; } = "";
       
      
        public virtual ICollection<Ventas> Ventas { get; set; }

        public virtual ICollection<Pagos> Pagos { get; set; }
    }
}
