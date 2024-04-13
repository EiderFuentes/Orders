using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Orders.Shared.Entities;

namespace Orders.Backend.Data
{
    // la clase Datacontext Herreda de la clase DbContext
    public class DataContext : IdentityDbContext<User>
    {
        // Creo el constructor para conectarme a la base de datos
        public DataContext(DbContextOptions<DataContext> options) : base(options) 
        {
                
        }
        //Esta propiedad llamada Dbset es la encargada de crear y de mapear las entidades de la base de dato
        public DbSet<Category> Categories { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<City> Cities { get; set; }

        //Para hacer cambios a la base de datos
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //Crear Indices en la base de datos unico
            modelBuilder.Entity<Category>().HasIndex(x => x.Name).IsUnique();
            modelBuilder.Entity<Country>().HasIndex( x => x.Name).IsUnique();
            modelBuilder.Entity<State>().HasIndex(s => new { s.CountryId, s.Name }).IsUnique();//Indice compuesto por estados
            modelBuilder.Entity<City>().HasIndex(c => new { c.StateId, c.Name }).IsUnique();//Indice compuesto por cuidades
            DisableCascadingDelete(modelBuilder);//Deshabilita el borrado en cascada

        }
        // Metodo para Deshabilitar el borrado en cascada
        private void DisableCascadingDelete(ModelBuilder modelBuilder)
        {
            var relationships = modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys());
            foreach (var relationship in relationships)
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

        }
    }
}
