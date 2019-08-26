using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;

namespace Time.Functions.Sas.POC
{
    public static class SAStokenGenerator
    {

        public static SasDto GetToken(string storageConnectionString,string containerName,double hoursToexpire)
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
                    Permissions = SharedAccessBlobPermissions.List | SharedAccessBlobPermissions.Write
                };

            //Generate the shared access signature on the container, setting the constraints directly on the signature.
            string sasContainerToken = container.GetSharedAccessSignature(sasConstraints);
           
            //Return the URI string for the container, including the SAS token.    
            return new SasDto { Uri = container.Uri.ToString(), Token = sasContainerToken, ExpireDate = sharedAccessExpiryTime.ToString() };
        }
    }
}
