
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Orders.Backend.Helpers
{
    public class FileStorage : IFileStorage
    {
        private readonly string _connectionString;

        public FileStorage(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AzureStorage")!;
        }

        public async Task RemoveFileAsync(string path, string containerName)
        {
            var client = new BlobContainerClient(_connectionString, containerName);
            await client.CreateIfNotExistsAsync();
            var fileName = Path.GetFileName(path);
            var blob = client.GetBlobClient(fileName);
            await blob.DeleteIfExistsAsync();
        }

        public async Task<string> SaveFileAsync(byte[] content, string extention, string containerName)
        {
            var client = new BlobContainerClient(_connectionString, containerName);
            await client.CreateIfNotExistsAsync();//Creamos el contenedor llamdo users
            client.SetAccessPolicy(PublicAccessType.Blob);//Es una la clave de 16 carracteres que es inrepetible
            var fileName = $"{Guid.NewGuid()}{extention}";
            var blob = client.GetBlobClient(fileName);

            //Metodo para subir en azure
            using (var ms = new MemoryStream(content))
            {
                await blob.UploadAsync(ms);
            }

            return blob.Uri.ToString();
        }
    }
}
