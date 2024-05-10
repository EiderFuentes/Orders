﻿using Blazored.Modal;
using Blazored.Modal.Services;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Orders.Frontend.Repositories;
using Orders.Shared.Entities;
using System.Net;

namespace Orders.Frontend.Pages.Categories
{
    [Authorize(Roles = "Admin")]
    public partial class CategoriesIndex
    {
        private int currentPage = 1;
        private int totalPages;

        //Creamo una directiva llamada Inject para asi inyectar un repositorio
        [Inject] private IRepository Repository { get; set; } = null!;
        // Inyectamos el SweetAlertService que es la libreria de alertas
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        // Inyectamos el NavigationManager para direccionar
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;

        [Parameter, SupplyParameterFromQuery] public string Page { get; set; } = string.Empty;
        [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;
        [Parameter, SupplyParameterFromQuery] public int RecordsNumber { get; set; } = 10;
        [CascadingParameter] IModalService Modal { get; set; } = default!;

        //Hacemos el llamado del reposity, el country puede ser null con el ?
        public List<Category>? Categories { get; set; }

        // Ciclo OnInitializedAsync significa cuando la pagina carge automaticamente se va a ejecutar 
        protected override async Task OnInitializedAsync()
        {
            //Creamos este nuevo metodo 
            await LoadAsync();
        }
        private async Task ShowModalAsync(int id = 0, bool isEdit = false)
        {
            IModalReference modalReference;

            if (isEdit)
            {
                modalReference = Modal.Show<CategoryEdit>(string.Empty, new ModalParameters().Add("Id", id));
            }
            else
            {
                modalReference = Modal.Show<CategoryCreate>();
            }

            var result = await modalReference.Result;
            if (result.Confirmed)
            {
                await LoadAsync();
            }
        }

        private async Task SelectedRecordsNumberAsync(int recordsnumber)
        {
            RecordsNumber = recordsnumber;
            int page = 1;
            await LoadAsync(page);
            await SelectedPageAsync(page);
        }

        //Filtro Genérico
        private async Task FilterCallBack(string filter)
        {
            Filter = filter;
            await ApplyFilterAsync();
            StateHasChanged();//llamo el metodo para que refresque la interfaz
        }

        private async Task SelectedPageAsync(int page)
        {
            currentPage = page;
            await LoadAsync(page);
        }
        //Metodo que me carge la lista
        private async Task LoadAsync(int page = 1)
        {
            if (!string.IsNullOrWhiteSpace(Page))
            {
                page = Convert.ToInt32(Page);
            }

            var ok = await LoadListAsync(page);
            if (ok)
            {
                await LoadPagesAsync();
            }
        }
        private void ValidateRecordsNumber(int recordsnumber)
        {
            if (recordsnumber == 0)
            {
                RecordsNumber = 10;
            }
        }

        private async Task<bool> LoadListAsync(int page)
        {
            ValidateRecordsNumber(RecordsNumber);
            var url = $"api/categories?page={page}&recordsnumber={RecordsNumber}";
            if (!string.IsNullOrEmpty(Filter))
            {
                url += $"&filter={Filter}";
            }

            var responseHttp = await Repository.GetAsync<List<Category>>(url);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return false;
            }
            Categories = responseHttp.Response;
            return true;
        }

        private async Task ApplyFilterAsync()
        {
            int page = 1;
            await LoadAsync(page);
            await SelectedPageAsync(page);
        }

        private async Task LoadPagesAsync()
        {
            ValidateRecordsNumber(RecordsNumber);
            var url = $"api/categories/totalPages?recordsnumber={RecordsNumber}";
            if (!string.IsNullOrEmpty(Filter))
            {
                url += $"&filter={Filter}";
            }

            var responseHttp = await Repository.GetAsync<int>(url);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            totalPages = responseHttp.Response;
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

            var responseHTTP = await Repository.DeleteAsync<Category>($"api/categories/{category.Id}");
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
            await LoadAsync();
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

