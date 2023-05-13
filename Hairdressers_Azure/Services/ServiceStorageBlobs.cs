using Azure.Storage.Blobs;

namespace Hairdressers_Azure.Services {
    public class ServiceStorageBlobs {

        private BlobServiceClient client;

        public ServiceStorageBlobs(BlobServiceClient client) {
            this.client = client;
        }

        //public async Task<string> GetBlobAsync(string containerName, string nameBlob) {
        //    BlobContainerClient containerClient = this.client.GetBlobContainerClient(containerName);
        //    BlobClient blobClient = containerClient.GetBlobClient(nameBlob);
        //    return blobClient.Uri.AbsoluteUri;
        //}

        public async Task UploadBlobAsync(string containerName, string blobName, Stream stream) {
            BlobContainerClient containerClient = this.client.GetBlobContainerClient(containerName);
            await containerClient.UploadBlobAsync(blobName, stream);
        }

        public async Task DeleteBlobAsync(string containerName, string blobName) {
            BlobContainerClient containerClient = this.client.GetBlobContainerClient(containerName);
            await containerClient.DeleteBlobAsync(blobName);
        }

        //public async Task<BlobContainerClient> GetContainerAsync(string containerName) {
        //    BlobContainerClient container = this.client.GetBlobContainerClient(containerName);
        //    return container;
        //}

    }
}
