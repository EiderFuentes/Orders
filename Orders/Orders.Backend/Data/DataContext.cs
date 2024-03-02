using Microsoft.EntityFrameworkCore;
using Orders.Shared.Entities;

namespace Orders.Backend.Data
{
    // la clase Datacontext Herreda de la clase DbContext
    public class DataContext : DbContext
    {
        // Creo el constructor para conectarme a la base de datos
        public DataContext(DbContextOptions<DataContext> options) : base(options) 
        {
                
        }

        //Esta propieda llamada Dbset es la encargada de crear y de mapear las entidades de la base de dato
        public DbSet<Country> Countries { get; set; }

        //Para hacer cambios a la base de datos
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //Crear Indices en la base de datos unico
            modelBuilder.Entity<Country>().HasIndex( x => x.Name).IsUnique();
        }

    }
}
