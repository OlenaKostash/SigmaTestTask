using Azure.Storage.Blobs;
using ImageAPI.Contracts;
using Microsoft.Extensions.Options;

namespace ImageAPI.DAL
{
    public enum UploadStatus
    {
        Ok,
        BlobAlreadyExists,
        ContainerNotFound,
        UnhandledEx
    }

    public class AzureBlobImagesRepository(Serilog.ILogger logger, IOptions<AzureBlobSettings> settings, BlobServiceClient blobServiceClient) : IImagesRepository
    {
        private BlobContainerClient _containerClient = blobServiceClient.GetBlobContainerClient(settings.Value.AzureBlobContainerName);

        public async Task<UploadStatus> UploadImage(ImageUploadRequest request)
        {
            try
            {
                await _containerClient.CreateIfNotExistsAsync();

                var fileName = request.Image.FileName;

                var blobClient = _containerClient.GetBlobClient(fileName);

                await using var stream = request.Image.OpenReadStream();
                var status = await blobClient.UploadAsync(stream, overwrite: request.AllowImageOverwrite);
                                
                var metadata = new Dictionary<string, string>
                {
                    { "Name", request.FileName },
                    { "Description", request.Description ?? "" }
                };
                await blobClient.SetMetadataAsync(metadata);

                logger.Debug($"File uploaded successfully: {fileName}");              
                return UploadStatus.Ok;
            }
            catch (Azure.RequestFailedException ex) when (ex.ErrorCode == "ContainerNotFound")
            {
                logger.Error($"Container {settings.Value.AzureBlobContainerName} not found. Exception: {ex.Message}");
                return UploadStatus.ContainerNotFound;
            }
            catch (Azure.RequestFailedException ex) when (ex.ErrorCode == "BlobAlreadyExists")
            {
                logger.Error($"Blob already exists: {request.Image.FileName}. Exception: {ex.Message}");
                return UploadStatus.BlobAlreadyExists;
            }
            catch (Exception ex) 
            { 
                logger.Error(ex.Message, ex);
                return UploadStatus.UnhandledEx;
            }
        }        
    }
}
