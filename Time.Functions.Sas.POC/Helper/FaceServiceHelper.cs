using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Time.Functions.Sas.POC
{
    public static class FaceServiceHelper
    {
        private static string storageConnectionString = Environment.GetEnvironmentVariable(Constants.StorageConnectionStringVariable, EnvironmentVariableTarget.Process);
        private static string hoursToexpire = Environment.GetEnvironmentVariable(Constants.HoursToexpire, EnvironmentVariableTarget.Process);

        public static async Task<List<string>> GetBlobUrisByContainer(CloudBlobContainer blobContainer, string clientId, long employeeUid, ILogger logger)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            var backupBlobClient = storageAccount.CreateCloudBlobClient();
            BlobContinuationToken blobContinuationToken = null;
            List<string> employeeUriList = new List<string>();
            do
            {
                var results = await blobContainer.ListBlobsSegmentedAsync(null, blobContinuationToken);
                blobContinuationToken = results.ContinuationToken;
                foreach (IListBlobItem item in results.Results)
                {
                    if (item.Uri.ToString().Contains(employeeUid.ToString()))
                    {
                       
                        var blobName = item.Uri.ToString().Split('/');
                        var uri = SAStokenGenerator.GetToken(storageConnectionString, item.Container.Name, blobName[blobName.Length - 1],  Convert.ToDouble(hoursToexpire));
                        employeeUriList.Add(uri.ToString());
                    }
                }
            } while (blobContinuationToken != null);
            return employeeUriList;
        }

    }
}
