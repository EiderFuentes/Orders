using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Orders.Frontend.Repositories;
using Orders.Shared.Entities;

namespace Orders.Frontend.Pages.Countries
{
    //Agregamos la palabra parcial para que se compilen en una sola
    public partial class CountryCreate
    {
        //Creamos un objeto de tipo country
        private Country country = new();

        //Es la representacion del codigo blazor a mi codigo C# para referenciar el formulario
        private CountryForm? countryForm;

        //Inyectamos el repositorio para poder acceder a los paises
        [Inject] private IRepository repository { get; set; } = null!;

        // Inyectamos el SweetAlertService que es la libreria de alertas
        [Inject] private SweetAlertService sweetAlertService { get; set; } = null!;

        // Inyectamos el NavigationManager para direccionar
        [Inject] private NavigationManager navigationManager { get; set; } = null!;

        //Metodo para crear un pais
        private async Task CreateAsync()
        {
            //Ejecuta el reposirio para crer el pais con el metodo PostAsync
            var responseHttp = await repository.PostAsync("/api/countries", country);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await sweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            //Creamos un Return para salir
            Return();

            //Creamos el Toast para mostrar la alerta del registro con exito
            var toast = sweetAlertService.Mixin(new SweetAlertOptions
            {
                Toast = true,
                Position = SweetAlertPosition.BottomEnd,
                ShowConfirmButton = true,
                Timer = 3000
            });
            await toast.FireAsync(icon: SweetAlertIcon.Success, message: "Registro creado con éxito.");
        }

        //Creamos un metodo para salir
        private void Return()
        {
            countryForm!.FormPostedSuccessfully = true;
            //lo ponemos a navegar y lo devolvemos al index de countries
            navigationManager.NavigateTo("/countries");
        }
    }
}