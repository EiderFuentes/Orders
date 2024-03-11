namespace Orders.Shared.Responses
{
    //Clase Generica
    public class ActionResponse<T>
    {
        //Si la accion que yo ejecute fue exitosa o no
        public bool WasSuccess { get; set; }
        //Devuelve Mensage de error
        public string? Message { get; set; }
        //Para devolver el resultado
        public T? Result { get; set; }
    }
}