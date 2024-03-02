using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orders.Backend.Data;
using Orders.Shared.Entities;

namespace Orders.Backend.Controllers
{
    //Anotacion para que la clase sea un Controlador
    [ApiController]
    //Anotacion para Rutear es como lo voy a ver en el Shared
    [Route("api/[controller]")]

    //Herreda de la clase controllerBase
    public class CountriesController : ControllerBase
    {
        //Creo un campo para el DataContext con contrl + . como lectura
        //_context es para diferenciar del atributo privado "_context" del parametro (context)
        private readonly DataContext _context;

        //Inyerta la base de dato por constructor
        public CountriesController(DataContext context)
        {
            _context = context;
        }

        //Metodo para jalar informacion o mostrar
        [HttpGet]
        //Tambien tengo que hacer mi Metodo principal asincronico 
        public async Task<IActionResult> GetAsync()
        {
            //Metodo asincronico para mostrar los paises
            return Ok(await _context.Countries.ToListAsync());
        }

        //Metodo que busca un pais en especifico con su respectivo parametro por ruta
        [HttpGet("{id}")]
        //Tambien tengo que hacer mi Metodo principal asincronico 
        //Hacemos una sobre carga al metodo Get pasando el parametro id
        public async Task<IActionResult> GetAsync(int id)
        {
            // Creo un objeto para buscar el pais y mas el metodo await
            var country = await _context.Countries.FindAsync(id);
            if (country == null)
            {
                // Retonar un error con respuesta 404
                return NotFound();
            }
            //Metodo asincronico para mostrar un pais en especifico
            return Ok(country);
        }

        //Metodo para crear paises
        [HttpPost]
        //Tambien tengo que hacer mi Metodo principal asincronico 
        public async Task<IActionResult> PostAsync(Country country)// Parametro por Boggy
        {
            //Adiciona un pais a la base de datos
            _context.Add(country);
            //Metodo asincronico para guardar los paises y tambien hace el commit
            await _context.SaveChangesAsync();
            return Ok(country);//Respuesta 200 del Http

        }

        //Metodo para modificar un pais
        [HttpPut]
        //Tambien tengo que hacer mi Metodo principal asincronico 
        //
        public async Task<IActionResult> PutAsync(Country country)
        {
            //Actualiza un pais a la base de datos
            _context.Update(country);
            //Metodo asincronico para guardar los paises y tambien hace el commit
            await _context.SaveChangesAsync();
            return NoContent();//Metodo para no devolver el pais


        }

        //Metodo para Eliminar un pais
        [HttpDelete("{id}")]
        //Tambien tengo que hacer mi Metodo principal asincronico 
        //Hacemos una sobre carga al metodo Get pasando el parametro id
        public async Task<IActionResult> DeleteAsync(int id)
        {
            // Creo un objeto para buscar el pais y mas el metodo await
            var country = await _context.Countries.FindAsync(id);
            if (country == null)
            {
                // Retonar un error con respuesta 404
                return NotFound();
            }
            //Elimina un pais a la base de datos
            _context.Remove(country);
            //Metodo asincronico para guardar los paises y tambien hace el commit
            await _context.SaveChangesAsync();
            //Metodo para no devolver el pais
            return NoContent();
        }

    }
}
