using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Orders.Frontend.Repositories;
using Orders.Frontend.Shared;
using Orders.Shared.Entities;
using System.Net;

namespace Orders.Frontend.Pages.Countries
{
    [Authorize(Roles = "Admin")]
    public partial class CountryEdit
    {
        //Creamos un objeto de tipo country pero el pais es nulo
        private Country? country;
        //Es la representacion del codigo blazor a mi codigo C# para referenciar el formulario
        //private CountryForm? countryForm;

        //Referencia al formulario generico
        private FormWithName<Country>? countryForm;

        //Inyectamos el repositorio para poder acceder a los paises
        [Inject] private IRepository Repository { get; set; } = null!;
        // Inyectamos el SweetAlertService que es la libreria de alertas
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        // Inyectamos el NavigationManager para direccionar
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        //Paramtro Id para editar un pais
        [EditorRequired, Parameter] public int Id { get; set; }

        //Metodo que me va a cargar el id del pais
        protected async override Task OnParametersSetAsync()
        {
            //Voy a capturar el pais que yo necesito
            var responseHTTP = await Repository.GetAsync<Country>($"api/countries/{Id}");
            if (responseHTTP.Error)
            {
                //Si el usuario se puso a joder con la respuesta Http
                if (responseHTTP.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo("countries");
                }
                else
                {
                    var messageError = await responseHTTP.GetErrorMessageAsync();
                    //Pintamos el error con libreria sweetAlertService
                    await SweetAlertService.FireAsync("Error", messageError, SweetAlertIcon.Error);
                }
            }
            else
            {
                //Me carga el pais
                country = responseHTTP.Response;
            }
        }

        //Metodo para ediar el pais
        private async Task EditAsync()
        {   //Ejecutamos nuestro metodo PutAsync
            var responseHTTP = await Repository.PutAsync("api/countries", country);

            if (responseHTTP.Error)
            {
                var mensajeError = await responseHTTP.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", mensajeError, SweetAlertIcon.Error);
                return;
            }
            //Creamos un Return para salir
            Return();

            //Creamos el Toast para mostrar la alerta del registro con exito
            var toast = SweetAlertService.Mixin(new SweetAlertOptions
            {
                Toast = true,
                Position = SweetAlertPosition.BottomEnd,
                ShowConfirmButton = true,
                Timer = 3000
            });
            await toast.FireAsync(icon: SweetAlertIcon.Success, message: "Cambios guardados con éxito.");
        }
        //Creamos un metodo para salir
        private void Return()
        {
            countryForm!.FormPostedSuccessfully = true;
            //lo ponemos a navegar y lo devolvemos al index de countries
            NavigationManager.NavigateTo("/countries");
        }

    }
}