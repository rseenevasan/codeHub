using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Time.Core.AzureStorage;

namespace Paycor.Time.Functions.Common
{
    public class BlobHelper<T> where T : class
    {
        private AzureBlobEngine<T> _azureBlobEngine;
        CloudStorageAccount cloudStorageAccount; 

        public BlobHelper(string connectionString)
        {
            _azureBlobEngine = new AzureBlobEngine<T>(new AzureStorageContext(connectionString));
            cloudStorageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("Time-Storage-ConnectionString", EnvironmentVariableTarget.Process));
        }

        public async Task<T> LoadBlobDataAsync(ILogger log, string blobName, string container)
        {
            T blobData = default(T);
            if (!string.IsNullOrEmpty(blobName))
            {
                try
                {
                    blobData = await _azureBlobEngine.RetrieveBlob(blobName, container);
                }
                catch (Exception ex)
                {
                    log.LogError(ex, ex.Message);
                }
            }
            else
            {
                log.LogError("Invalid blob name passed");
            }

            return blobData;
        }

        public async Task<bool> DeleteBlobDataAsync(ILogger log, string blobName, string container)
        {
            bool isDeleted = false;
            if (!string.IsNullOrEmpty(blobName))
            {
                try
                {
                    isDeleted = await _azureBlobEngine.DeleteBlob(blobName, container);
                }
                catch (Exception ex)
                {
                    log.LogError(ex, ex.Message);
                }
            }
            else
            {
                log.LogError("Invalid blob name passed");
            }

            return isDeleted;
        }
        //public async Task<Uri> GetBlobContainerWithSas(string accountName, string containerName, string blobName,ILogger log)
        //{
           
        //        try
        //        {
        //          // return await _azureBlobEngine.GetUserDelegationSasBlob(accountName,containerName,blobName);
        //        }
        //        catch (Exception ex)
        //        {
        //            log.LogError(ex, ex.Message);
        //            return null;
        //        }
           
        //}
        public async Task<CloudBlobContainer> GetBlobContainer(string containerName, ILogger log)
        {
            CloudBlobContainer container = null;
            if (!string.IsNullOrEmpty(containerName))
            {
                try
                {
                    container = await _azureBlobEngine.GetBlobContainer(containerName);
                }
                catch (Exception ex)
                {
                    log.LogError(ex, ex.Message);
                }
            }
            else
            {
                log.LogError("Invalid containerName passed");
            }

            return container;
        }            
    }
}
