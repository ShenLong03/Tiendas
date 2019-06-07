using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace Domain
{
  public class Productos
    {

        [Key]
        public int ProductoId { get; set; }

        [Display(Name = "Codigo")]
        public string CodigoId { get; set; }

        public string Descripcion { get; set; } = "";

        public string Talla { get; set; } = "";

        public double Precio { get; set; } = 0;

        public int CategoriaId { get; set; }

        public double Cantidad { get; set; } = 0;

        //[NotMapped]
        //public HttpPostedFileBase FotoFile { get; set; }

        public string Foto { get; set; }

        [NotMapped]
        public string FotoFull { get { return !string.IsNullOrEmpty(Foto.Trim())? string.Concat(WebConfigurationManager.AppSettings["URL"], Foto.Substring(1)):string.Empty; } }

        public virtual ICollection<DetalleVentas> DetalleVentas { get; set; }

        public virtual Categorias Categorias { get; set; }

    }
}
