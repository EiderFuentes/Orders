using Microsoft.EntityFrameworkCore;
using Orders.Backend.Data;
using Orders.Backend.Repositories.Interfaces;
using Orders.Shared.Responses;

namespace Orders.Backend.Repositories.Implementations
{
    //Esta clase repositorio generica va implementa la IGenericRepository
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        //Atributo para inyectarle el Datacontext
        private readonly DataContext _context;
        //Atributo para Mapea las entidades
        private readonly DbSet<T> _entity;

        //Creamos el constructor para inyectarle el Datacontext
        public GenericRepository(DataContext context)
        {
            _context = context;
            _entity = _context.Set<T>();
        }

        //Metodo para Agregar en repositorio generico
        public virtual async Task<ActionResponse<T>> AddAsync(T entity)
        {
            //agrega mi entity que yo le estoy mandando
            _context.Add(entity);
            try
            {
                //Guadar los cambio de forma async
                await _context.SaveChangesAsync();
                return new ActionResponse<T>
                {
                    WasSuccess = true,
                    Result = entity
                };
            }
            catch (DbUpdateException)
            {
                return DbUpdateExceptionActionResponse();
            }
            catch (Exception exception)
            {
                return ExceptionActionResponse(exception);
            }
        }

        //Metodo para Eliminar en repositorio generico
        public virtual async Task<ActionResponse<T>> DeleteAsync(int id)
        {
            //Busca el id para borarlo
            var row = await _entity.FindAsync(id);
            if (row == null)
            {
                return new ActionResponse<T>
                {
                    WasSuccess = false,
                    Message = "Registro no encontrado"
                };
            }

            try
            {
                _entity.Remove(row);
                await _context.SaveChangesAsync();
                return new ActionResponse<T>
                {
                    WasSuccess = true,
                };
            }
            catch
            {
                return new ActionResponse<T>
                {
                    WasSuccess = false,
                    Message = "No se puede borrar, porque tiene registros relacionados"
                };
            }

        }
        //Metodo que busca en objeto en el repositorio generico
        public virtual async Task<ActionResponse<T>> GetAsync(int id)
        {
            //Busca el id para mostralo
            var row = await _entity.FindAsync(id);
            if (row == null)
            {
                return new ActionResponse<T>
                {
                    WasSuccess = false,
                    Message = "Registro no encontrado"
                };
            }

            return new ActionResponse<T>
            {
                WasSuccess = true,
                Result = row
            };
        }

        //Metodo que devuelve la lista en repositorio generico
        public virtual async Task<ActionResponse<IEnumerable<T>>> GetAsync()
        {
            return new ActionResponse<IEnumerable<T>>
            {
                WasSuccess = true,
                Result = await _entity.ToListAsync()
            };

        }

        //Metodo para actulizar en repositorio generico
        public virtual async Task<ActionResponse<T>> UpdateAsync(T entity)
        {
            _context.Update(entity);
            try
            {
                await _context.SaveChangesAsync();
                return new ActionResponse<T>
                {
                    WasSuccess = true,
                    Result = entity
                };
            }
            catch (DbUpdateException)
            {
                return DbUpdateExceptionActionResponse();
            }
            catch (Exception exception)
            {
                return ExceptionActionResponse(exception);
            }

        }

        //Metodo para validad registros duplicados
        private ActionResponse<T> DbUpdateExceptionActionResponse()
        {
            return new ActionResponse<T>
            {
                WasSuccess = false,
                Message = "Ya existe el registro que estas intentando crear."
            };
        }

        private ActionResponse<T> ExceptionActionResponse(Exception exception)
        {
            return new ActionResponse<T>
            {
                WasSuccess = false,
                Message = exception.Message
            };
        }
    }
}
