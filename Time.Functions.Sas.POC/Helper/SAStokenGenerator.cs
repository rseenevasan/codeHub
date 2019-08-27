
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;


namespace Time.Functions.Sas.POC
{
    public static class SAStokenGenerator
    {
        public static Uri GetToken(string storageConnectionString,string containerName,string blobName,double hoursToexpire)
        {
            //Parse the connection string and return a reference to the storage account.
            CloudStorageAccount storageAccount =
                CloudStorageAccount.Parse(storageConnectionString);

            //Create the blob client object.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            //Get a reference to a container to use for the sample code, and create it if it does not exist.
            //_blobContainerName = epoch
            CloudBlobContainer container =
                blobClient.GetContainerReference(containerName);

            //Set the expiry time and permissions for the container.
            //In this case no start time is specified, so the shared access signature becomes valid immediately.
            var sharedAccessExpiryTime = DateTimeOffset.UtcNow.AddHours(hoursToexpire);
            SharedAccessBlobPolicy sasConstraints =
                new SharedAccessBlobPolicy
                {
                    SharedAccessExpiryTime = sharedAccessExpiryTime,
                    Permissions = SharedAccessBlobPermissions.Read
                };

            //Generate the shared access signature on the container, setting the constraints directly on the signature.
            string sasContainerToken = container.GetSharedAccessSignature(sasConstraints);
            UriBuilder fullUri = new UriBuilder()
            {
                Scheme = "https",
                Host = string.Format("{0}.blob.core.windows.net", storageAccount.Credentials.AccountName),
                Path = string.Format("{0}/{1}", containerName, blobName),
                Query = sasContainerToken
            };
            return fullUri.Uri;
         }
    }
}
