namespace Orders.Shared.Repositories
{
    //Interface generica para editar cualquier formulario que yo necesite el nombre
    public interface IEntityWithName
    {
        string Name { get; set; }
    }
}
