using Microsoft.AspNetCore.Components;

namespace Orders.Frontend.Shared
{
    //Agregamos la palabra partial que hay 2 clase que significan lo mismo pero cuando se compilan generan una sola
    // GenericList es un componente para listar paises, cuidades o lo que yo quiera
    public partial class GenericList<Titem>
    {
        //Creamos una anotacion que un Parametro
        //Va hacer lo que yo quiero que me muestre mi componente cuando esta cargando
        [Parameter] public RenderFragment? Loading { get; set; }
        // Que voy a mostrar si no hay paises
        [Parameter] public RenderFragment? NoRecords { get; set; }
        // Que voy a mostrar el body es obligatorio
        [EditorRequired, Parameter] public RenderFragment Body { get; set; } = null!;
        // Muestra la lista de <Titem>
        [EditorRequired, Parameter] public List<Titem> MyList { get; set; } = null!;
    }
}
