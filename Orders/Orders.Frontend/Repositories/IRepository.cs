namespace Orders.Frontend.Repositories
{
    public interface IRepository
    {
        //Metodo asincronico Get la T es generico
        Task<HttpResponseWrapper<T>> GetAsync<T>(string url);

        //Metodo post no devuelve respuesta
        Task<HttpResponseWrapper<object>> PostAsync<T>(string url, T model);

        //Metodo post devuelve respuesta
        Task<HttpResponseWrapper<TActionResponse>> PostAsync<T, TActionResponse>(string url, T model);

        //Metodo Delete no devuelve respuesta
        Task<HttpResponseWrapper<object>> DeleteAsync<T>(string url);

        //Metodo put no devuelve respuesta
        Task<HttpResponseWrapper<object>> PutAsync<T>(string url, T model);

        //Metodo post devuelve respuesta
        Task<HttpResponseWrapper<TActionResponse>> PutAsync<T, TActionResponse>(string url, T model);
    }
}