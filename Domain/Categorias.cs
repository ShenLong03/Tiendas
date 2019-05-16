using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
   public class Categorias
    {
        [Key]
        public int CategoriaId { get; set; }

        [Required(ErrorMessage = "El {0} es un campo requerido")]
        [Display(Name = "Categoria")]
        public string Nombre { get; set; }

        public string Prefijo { get; set; }

        public bool Activo { get; set; } = true;

        public virtual ICollection<Productos> Productos { get; set; }
    }
}
