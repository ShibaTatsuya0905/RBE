using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
namespace RestaurantManagement.API.ExternalServices;
public class PhotoService : IPhotoService
{
    private readonly Cloudinary _cloudinary;
    public PhotoService(IConfiguration config)
    {
        var acc = new Account(config["CloudinarySettings:CloudName"], config["CloudinarySettings:ApiKey"], config["CloudinarySettings:ApiSecret"]);
        _cloudinary = new Cloudinary(acc);
    }
    public async Task<string> AddPhotoAsync(IFormFile file)
    {
        if (file.Length > 0)
        {
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams { File = new FileDescription(file.FileName, stream), Transformation = new Transformation().Height(500).Width(500).Crop("fill") };
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl.ToString();
        }
        return string.Empty;
    }
}