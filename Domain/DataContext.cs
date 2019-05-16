using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System;

namespace Domain
{
    public class DataContext : DbContext
    {
        public DataContext() : base("DefaultConnection")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

     

        public DbSet<Cliente> Clientes { get; set; }

       

        public DbSet<MediosPago> MediosPago { get; set; }

        


        public DbSet<Usuario> Usuarios { get; set; }


        
        public DbSet<Pagos> Pagos { get; set; }

       

        public DbSet<CierreCaja> CierreCajas { get; set; }

       

        public DbSet<Productos>  Productos { get; set; }


        public DbSet<Ventas>  Ventas { get; set; }


        public DbSet<DetalleVentas> DetalleVentas { get; set; }

        public System.Data.Entity.DbSet<Domain.Categorias> Categorias { get; set; }




        //public virtual ObjectResult<sp_CargarDataFacuras_Result> sp_CargarDataFacuras(Nullable<int> idVenta)
        //{
        //    var idVentaParameter = idVenta.HasValue ?
        //        new ObjectParameter("idVenta", idVenta) :
        //        new ObjectParameter("idVenta", typeof(int));

        //    return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_CargarDataFacuras_Result>("sp_CargarDataFacuras", idVentaParameter);
        //}

        //public partial class sp_CargarDataFacuras_Result
        //{
        //    public int VentaId { get; set; }
        //    public string Nombre { get; set; }
        //    public double TotalOrden { get; set; }
        //    public System.DateTime Fecha { get; set; }
        //    public int Cantidad { get; set; }
        //    public double Descuento { get; set; }
        //    public double SubTotal { get; set; }
        //    public double Precio { get; set; }
        //    public int DetalleVentasId { get; set; }
        //    public string Descripcion { get; set; }
        //    public string Abreviatura { get; set; }
        //}

    }
    }

