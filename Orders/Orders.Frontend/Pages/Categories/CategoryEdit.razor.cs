using Blazored.Modal;
using Blazored.Modal.Services;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Orders.Frontend.Pages.Countries;
using Orders.Frontend.Repositories;
using Orders.Frontend.Shared;
using Orders.Shared.Entities;
using System.Net;

namespace Orders.Frontend.Pages.Categories
{
    [Authorize(Roles = "Admin")]
    public partial class CategoryEdit
    {
        //Creamos un objeto de tipo country pero el pais es nulo
        private Category? category;
        //Es la representacion del codigo blazor a mi codigo C# para referenciar el formulario
        // private CategoryForm? categoryForm;

        //Referencia al formulario generico
        private FormWithName<Category>? categoryForm;

        //Inyectamos el repositorio para poder acceder a los paises
        [Inject] private IRepository Repository { get; set; } = null!;
        // Inyectamos el SweetAlertService que es la libreria de alertas
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        // Inyectamos el NavigationManager para direccionar
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [CascadingParameter] BlazoredModalInstance BlazoredModal { get; set; } = default!;
        //Paramtro Id para editar un pais
        [EditorRequired, Parameter] public int Id { get; set; }

        //Metodo que me va a cargar el id del pais
        protected async override Task OnParametersSetAsync()
        {
            //Voy a capturar el pais que yo necesito
            var responseHTTP = await Repository.GetAsync<Category>($"api/categories/{Id}");
            if (responseHTTP.Error)
            {
                //Si el usuario se puso a joder con la respuesta Http
                if (responseHTTP.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo("categories");
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
                category = responseHTTP.Response;
            }
        }

        //Metodo para ediar el pais
        private async Task EditAsync()
        {   //Ejecutamos nuestro metodo PutAsync
            var responseHTTP = await Repository.PutAsync("api/categories", category);

            if (responseHTTP.Error)
            {
                var mensajeError = await responseHTTP.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", mensajeError, SweetAlertIcon.Error);
                return;
            }

            await BlazoredModal.CloseAsync(ModalResult.Ok());
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
            categoryForm!.FormPostedSuccessfully = true;
            //lo ponemos a navegar y lo devolvemos al index de countries
            NavigationManager.NavigateTo("/categories");
        }

    }
}