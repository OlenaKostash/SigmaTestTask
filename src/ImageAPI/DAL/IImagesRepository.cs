using ImageAPI.Contracts;
using static ImageAPI.DAL.AzureBlobImagesRepository;

namespace ImageAPI.DAL
{
    public interface IImagesRepository
    {
        Task<UploadStatus> UploadImage(ImageUploadRequest request);
    }
}
