namespace Orders.Backend.Helpers
{
    public interface IFileStorage
    {
        Task<string> SaveFileAsync(byte[] content, string extention, string containerName);

        Task RemoveFileAsync(string path, string containerName);

        //Esta es una interfaz que tiene implementaccion por defecto
        async Task<string> EditFileAsync(byte[] content, string extention, string containerName, string path)
        {
            if (path is not null)
            {
                await RemoveFileAsync(path, containerName);
            }

            return await SaveFileAsync(content, extention, containerName);
        }
    }
}
