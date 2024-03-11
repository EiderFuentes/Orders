using Orders.Shared.Responses;

namespace Orders.Backend.UnitsOfWork.Implementations
{
    // Interfaz Unidad de trabajo
    public interface IGenericUnitOfWork<T> where T : class 
    {
        //Devuelve el id de las entidades
        Task<ActionResponse<T>> GetAsync(int id);

        //Devuelve todas las lista de las entidades
        Task<ActionResponse<IEnumerable<T>>> GetAsync();

        //Agrega las entidades
        Task<ActionResponse<T>> AddAsync(T entity);

        //Elimina las entidades
        Task<ActionResponse<T>> DeleteAsync(int id);

        //Actualiza las entidades
        Task<ActionResponse<T>> UpdateAsync(T entity);
    }
    
}
