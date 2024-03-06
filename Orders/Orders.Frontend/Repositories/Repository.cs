
using System.Text;
using System.Text.Json;

namespace Orders.Frontend.Repositories
{
    //la clase repository Implementa la interfaz IRepository
    public class Repository : IRepository
    {
        //Los atributos privados van con _ y es readonly porque son propiedades que solo se asigna el contructor
        private readonly HttpClient _httpClient;
        //El formato json es un estandar de java
        private JsonSerializerOptions _jsonDefaultOptions => new JsonSerializerOptions
        { //Mapea en Json independidente si viene en mayuscula o minuscula
           PropertyNameCaseInsensitive = true
        };

        //Creamos el constructor para inyertalo con HttpClient
        public Repository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        //Metodo asincronico async Get super generico que lo podemos utilizar en los proyectos
        public async Task<HttpResponseWrapper<T>> GetAsync<T>(string url)
        {
            //Creo una variable llamada responseHttp y hago un GetAsync
            var responseHttp = await _httpClient.GetAsync(url);
            //la respuesta es un OK
            if (responseHttp.IsSuccessStatusCode)
            {                       //Metodo privado UnserializeAnswer con ctr.
                var response = await UnserializeAnswer<T>(responseHttp);
                return new HttpResponseWrapper<T>(response, false, responseHttp);
            }
            //Cuando el get saque un error
            return new HttpResponseWrapper<T>(default, true, responseHttp);
        }
        //Metodo post que no devuelve respuesta
        public async Task<HttpResponseWrapper<object>> PostAsync<T>(string url, T model)
        {
            //Serealizar es coger en objeto en memoria y  volver un string en formato json
            var messageJSON = JsonSerializer.Serialize(model);
            //Codificamos nuestro codigo en Español UTF8 con la clase StringContent
            var messageContent = new StringContent(messageJSON, Encoding.UTF8, "application/json");
            //Hagame un post a esa url
            var responseHttp = await _httpClient.PostAsync(url, messageContent); 
            //Cuando el get saque un error
            return new HttpResponseWrapper<object>(null, !responseHttp.IsSuccessStatusCode, responseHttp);
        }
        //Copio todo el codigo del metodo get y lo pego en el post este si me devuelve una respuesta
        public async Task<HttpResponseWrapper<TActionResponse>> PostAsync<T, TActionResponse>(string url, T model)
        {
            //Serealizar es coger en objeto en memoria y  volver un string en formato json
            var messageJSON = JsonSerializer.Serialize(model);
            //Codificamos nuestro codigo en Español UTF8 con la clase StringContent
            var messageContent = new StringContent(messageJSON, Encoding.UTF8, "application/json");
            //Hagame un post a esa url
            var responseHttp = await _httpClient.PostAsync(url, messageContent);
            //la respuesta es un OK
            if (responseHttp.IsSuccessStatusCode)
            {      //Mando un TActionResponse para deserializar
                var response = await UnserializeAnswer<TActionResponse>(responseHttp);
                return new HttpResponseWrapper<TActionResponse>(response, false, responseHttp);
            }
            //Cuando el get saque un error
            return new HttpResponseWrapper<TActionResponse>(default, true, responseHttp);
        }
        //Generamos el metodo privado UnserializeAnswer con ctr. Sirve para Deserializar el objeto
        private async Task<T> UnserializeAnswer<T>(HttpResponseMessage responseHttp)
        {   //Este metodo lo que hace el leer el json
            var response = await responseHttp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(response, _jsonDefaultOptions)!;

        }
    }
}
