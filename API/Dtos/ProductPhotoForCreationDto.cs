using System.ComponentModel.DataAnnotations;
using API.Helpers;
using Microsoft.AspNetCore.Http;

namespace API.Dtos
{
    public class ProductPhotoForCreationDto
    {
        [MaxFileSize(2 * 1024 * 1024)]
        [AllowedExtensions(new []{".jpg", ".png", ".jpeg"})]
        [Required]
        public IFormFile Photo { get; set; }
    }
}