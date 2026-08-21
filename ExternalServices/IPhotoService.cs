namespace RestaurantManagement.API.ExternalServices;
public interface IPhotoService
{
    Task<string> AddPhotoAsync(IFormFile file);
}