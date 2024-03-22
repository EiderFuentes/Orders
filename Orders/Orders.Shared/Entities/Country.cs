using Orders.Shared.Repositories;
using System.ComponentModel.DataAnnotations;

namespace Orders.Shared.Entities
{
    public class Country : IEntityWithName
    {
        //Propiedades
        public int Id { get; set; }

        //DataAnnotations funciona para que el usuario ve el campo pais en español
        [Display(Name = "País")]
        //DataAnnotations para agregar un maxlength en la base de dato y no un nchar
        [MaxLength(100, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
        //DataAnnotations para validar que el campo ingrese vacio a la base de dato
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Name { get; set; } = null!;
    }
}