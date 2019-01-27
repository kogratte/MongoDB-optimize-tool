using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Threading.Tasks;

namespace HistoryForwarder.Core
{
    public interface IAzureBlobRepository
    {
        Task<string> StoreAsync(string content, string name);
    }

    public class AzureBlobRepository : IAzureBlobRepository
    {
        private readonly CloudBlobContainer cloudBlobContainer;
        private readonly CloudStorageAccount storageAccount;

        public AzureBlobRepository()
        {
            // Retrieve the connection string for use with the application. The storage connection string is stored
            // in an environment variable on the machine running the application called storageconnectionstring.
            // If the environment variable is created after the application is launched in a console or with Visual
            // Studio, the shell needs to be closed and reloaded to take the environment variable into account.
            string storageConnectionString = Config.BlobStorageConnexionString;

            // Check whether the connection string can be parsed.
            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
                CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                // Create a container called 'quickstartblobs' and append a GUID value to it to make the name unique. 
                this.cloudBlobContainer = cloudBlobClient.GetContainerReference(Config.BlobStorageContainer);
                cloudBlobContainer.CreateIfNotExists();

                // Set the permissions so the blobs are public. 
                BlobContainerPermissions permissions = new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                };
                cloudBlobContainer.SetPermissions(permissions);
            }
            else
            {
                Console.WriteLine(
                    "A connection string has not been defined in the system environment variables. " +
                    "Add a environment variable named 'storageconnectionstring' with your storage " +
                    "connection string as a value.");
            }
        }

        public async Task<string> StoreAsync(string content, string name)
        {
            // Get a reference to the blob address, then upload the file to the blob.
            // Use the value of localFileName for the blob name.
            CloudBlockBlob cloudBlockBlob = this.cloudBlobContainer.GetBlockBlobReference(name);
            await cloudBlockBlob.UploadTextAsync(content);

            return cloudBlockBlob.Uri.ToString();
        }
    }
}
