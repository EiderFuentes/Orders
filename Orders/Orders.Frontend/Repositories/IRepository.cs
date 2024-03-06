namespace Orders.Frontend.Repositories
{
    //Creamos una interfaz
    public interface IRepository
    {
        //Creamos 3 Metodos
        //Metodo asincronico Get la T es generica
        Task<HttpResponseWrapper<T>> GetAsync<T>(string url);
        //Metodo post no devuelve respuesta
        Task<HttpResponseWrapper<object>> PostAsync<T>(string url, T model);
        //Metodo post devuelve respuesta
        Task<HttpResponseWrapper<TActionResponse>> PostAsync<T, TActionResponse>(string url, T model);

    }
}
