using Orders.Shared.DTOs;

namespace Orders.Backend.Helpers
{
    //Metodo de Extension
    public static class QueryableExtensions
    {
        //Iqueryable: Es una consulta no materializada
        public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, PaginationDTO pagination)
        {
            return queryable
                .Skip((pagination.Page - 1) * pagination.RecordsNumber)//Cuantos te vas a saltar
                .Take(pagination.RecordsNumber);//Cuantos registro vas a tomar
        }

    }
}
