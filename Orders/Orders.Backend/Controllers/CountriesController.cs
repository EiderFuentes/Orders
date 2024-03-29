﻿using Microsoft.AspNetCore.Mvc;
using Orders.Backend.UnitsOfWork.Implementations;
using Orders.Shared.Entities;

namespace Orders.Backend.Controllers
{
    //Anotacion para que la clase sea un Controlador
    [ApiController]
    //Anotacion para Rutear es como lo voy a ver en el Shared
    [Route("api/[controller]")]

    //Herredamos de la clase GenericController<Country>
    public class CountriesController : GenericController<Country>
    {
        //Creamos el constructor para el controlador generico de paises
        public CountriesController(IGenericUnitOfWork<Country> unitOfWork) : base(unitOfWork)
        {
        }
    }
}
