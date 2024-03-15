using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components;
using Orders.Shared.Entities;

namespace Orders.Frontend.Pages.Categories
{
    public partial class CategoryForm
    {
        //Contexto de edicion es un objeto
        private EditContext editContext = null!;

        //Le pasamos el pais que vamos a editar se lo pasamos por parametro
        [EditorRequired, Parameter] public Category Category { get; set; } = null!;
        //Al parametro EventCallback le vamos a pasar codigo de esta forma lo guardamos
        [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }
        //Al parametro EventCallback le vamos a pasar codigo de esta forma lo cancelamos
        [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }
        // Inyectamos el SweetAlertService que es la libreria de alertas
        [Inject] public SweetAlertService SweetAlertService { get; set; } = null!;
        //Propiedad para poster el formulario o no
        public bool FormPostedSuccessfully { get; set; }

        //Metodo para cargar el contexto
        protected override void OnInitialized()
        {
            editContext = new(Category);
        }

        //Creamos este metodo para preguntrale si yo me sali del formulario sin guardar los cambios
        private async Task OnBeforeInternalNavigation(LocationChangingContext context)
        {
            //Creamos una variable si el formulario fue editado
            var formWasEdited = editContext.IsModified();
            //Prefuntamos si el formulario no fue editado Ó formulari fue posteado
            if (!formWasEdited || FormPostedSuccessfully)
            {
                return;
            }
            //Creamos la alerta con la libreria SweetAlertServic
            var result = await SweetAlertService.FireAsync(new SweetAlertOptions
            {
                Title = "Confirmación",
                Text = "¿Deseas abandonar la página y perder los cambios?",
                Icon = SweetAlertIcon.Question,
                ShowCancelButton = true
            });

            //Mandamos la alerta de la libreria SweetAlertServic
            //Si esto no fue vacio
            var confirm = !string.IsNullOrEmpty(result.Value);
            //preguntamos si el usuario quiere peder los cambio
            if (confirm)
            {
                return;
            }
            //metodo para no peder los cambio en el formulario
            context.PreventNavigation();
        }
    }
}