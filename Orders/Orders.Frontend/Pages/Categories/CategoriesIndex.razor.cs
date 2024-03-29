﻿using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Orders.Frontend.Repositories;
using Orders.Shared.Entities;
using System.Net;

namespace Orders.Frontend.Pages.Categories
{
    public partial class CategoriesIndex
    {
        //Creamo una directiva llamada Inject para asi inyectar un repositorio
        [Inject] private IRepository Repository { get; set; } = null!;
        // Inyectamos el SweetAlertService que es la libreria de alertas
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        // Inyectamos el NavigationManager para direccionar
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;

        //Hacemos el llamado del reposity, el country puede ser null con el ?
        public List<Category>? Categories { get; set; }

        // Ciclo OnInitializedAsync significa cuando la pagina carge automaticamente se va a ejecutar 
        protected override async Task OnInitializedAsync()
        {
            //Creamos este nuevo metodo 
            await LoadAsybc();
        }

        private async Task LoadAsybc()
        {
            //Estamos yendo al backend para así obtener una lista de países
            var responseHppt = await Repository.GetAsync<List<Category>>("api/categories");
            if (responseHppt.Error)
            {
                var message = await responseHppt.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            //Lista de paises que yo obtuve
            Categories = responseHppt.Response;

        }
        //Metodo para eliminar un pais
        private async Task DeleteAsync(Category category)
        {
            var result = await SweetAlertService.FireAsync(new SweetAlertOptions
            {
                Title = "Confirmación",
                Text = $"¿Esta seguro que quieres borrar la categoría: {category.Name}?",
                Icon = SweetAlertIcon.Question,
                ShowCancelButton = true
            });
            var confirm = string.IsNullOrEmpty(result.Value);
            if (confirm)
            {
                return;
            }

            var responseHTTP = await Repository.DeleteAsync($"api/categories/{category.Id}");
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
                return;
            }

            //Recargamos la pagina para que no me muestre el pais que borramos
            await LoadAsybc();
            //Creamos el Toast para mostrar la alerta del registro con exito
            var toast = SweetAlertService.Mixin(new SweetAlertOptions
            {
                Toast = true,
                Position = SweetAlertPosition.BottomEnd,
                ShowConfirmButton = true,
                Timer = 3000
            });

            await toast.FireAsync(icon: SweetAlertIcon.Success, message: "Registro borrado con éxito.");
        }
    }
}

