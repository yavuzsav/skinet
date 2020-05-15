using Core.Helper;
using Microsoft.AspNetCore.Http;

namespace Core.Interfaces
{
    public interface IPhotoService
    {
        PhotoUploadResult AddPhoto(IFormFile file);
        bool DeletePhoto(string publicId);
    }
}