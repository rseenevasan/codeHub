using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using Time.Core.AzureStorage;

namespace Time.Core.AzureStorage
{
    public class AzureBlobEngine<T> : IAzureBlobEngine<T> where T : class
    {
        private readonly IAzureStorageContext _azureStorageContext;

        public AzureBlobEngine(IAzureStorageContext azureStorageContext)
        {
            _azureStorageContext = azureStorageContext;
        }

        public async Task<CloudBlobContainer> GetBlobContainer(string containerName)
        {
            CloudBlobClient _blobClient = _azureStorageContext.GetCloudBlobClient();

            CloudBlobContainer _container = _blobClient.GetContainerReference(containerName);

            await _container.CreateIfNotExistsAsync();

            return _container;
        }
      
        public async Task<bool> DeleteBlob(string blobName, string containerName)
        {
            CloudBlobContainer _container = await GetBlobContainer(containerName);

            CloudBlockBlob blobReference = _container.GetBlockBlobReference(blobName);

            return await blobReference.DeleteIfExistsAsync();
        }

        public async Task<T> RetrieveBlob(string blobName, string containerName)
        {
            CloudBlobContainer _container = await GetBlobContainer(containerName);

            if(await _container.GetBlockBlobReference(blobName).ExistsAsync())
            {
                CloudBlockBlob blobReference = _container.GetBlockBlobReference(blobName);

                T result;
                using (var stream = new MemoryStream())
                {
                    await blobReference.DownloadToStreamAsync(stream);
                    stream.Position = 0;
                    var serializer = new JsonSerializer();

                    using (var sr = new StreamReader(stream))
                    {
                        string text = sr.ReadToEnd();

                        result = JsonConvert.DeserializeObject<T>(text);
                    }
                }

                return result;
            }
            else
            {
                throw new FileNotFoundException(string.Format("Blob with name ${0} not found", blobName));
            }
        }
    }
}