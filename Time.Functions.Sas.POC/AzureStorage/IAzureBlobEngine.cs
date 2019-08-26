using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Time.Core.AzureStorage
{
    public interface IAzureBlobEngine<T> where T : class
    {
        Task<CloudBlobContainer> GetBlobContainer(string containerName);

        Task<bool> DeleteBlob(string BlobName, string ContainerName);

        Task<T> RetrieveBlob(string BlobName, string ContainerName);

    }
}
