using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

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

        public DbSet<Tallas> Tallas { get; set; }

    }
    }

