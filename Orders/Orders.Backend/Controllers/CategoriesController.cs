using Microsoft.AspNetCore.Mvc;
using Orders.Backend.UnitsOfWork.Implementations;
using Orders.Shared.Entities;

namespace Orders.Backend.Controllers
{
    //Anotacion para que la clase sea un Controlador
    [ApiController]
    //Anotacion para Rutear es como lo voy a ver en el Shared
    [Route("api/[controller]")]

    public class CategoriesController : GenericController<Category>
    {
        public CategoriesController(IGenericUnitOfWork<Category> unitOfWork) : base(unitOfWork)
        {
        }
    }
}
