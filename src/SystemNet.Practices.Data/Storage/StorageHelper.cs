using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SystemNet.Practices.Data.Storage.Models;

namespace SystemNet.Practices.Data.Storage
{
    public class StorageHelper
    {

        public async Task<JsonImage> Upload(Stream stream, string nameFile, StorageConfig storageConfig)
        {
            var url = await UploadFileToStorage(stream, nameFile, storageConfig);

            return new JsonImage()
            {
                Url = url,
            };
        }

        private static async Task<string> UploadFileToStorage(Stream fileStream, string fileName,
            StorageConfig storageConfig)
        {
            var storageCredentials = new StorageCredentials(storageConfig.AccountName, storageConfig.AccountKey);
            var storageAccount = new CloudStorageAccount(storageCredentials, true);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(storageConfig.ImageContainer);
            var blockBlob = container.GetBlockBlobReference(fileName);

            await blockBlob.UploadFromStreamAsync(fileStream);

            
            return blockBlob.SnapshotQualifiedStorageUri.PrimaryUri.ToString();
        }

        public async  Task DeleteBlobData(string fileUrl, StorageConfig storageConfig)
        {
            Uri uriObj = new Uri(fileUrl);
            var storageCredentials = new StorageCredentials(storageConfig.AccountName, storageConfig.AccountKey);
            CloudStorageAccount cloudStorageAccount = new CloudStorageAccount(storageCredentials, true);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(storageConfig.ImageContainer);

            // get block blob refarence    
            CloudBlockBlob blockBlob = cloudBlobContainer.GetBlockBlobReference(Path.GetFileName(uriObj.LocalPath));

            // delete blob from container        
            await blockBlob.DeleteAsync();
        }

        public bool IsImage(string nameFile)
        {
            string[] formats = new string[] { ".jpg", ".png", ".gif", ".jpeg" };
            return formats.Any(item => nameFile.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }
    }
}
