using Microsoft.AspNetCore.Http;

namespace CloudAndCO.Services
{
    public interface IBlobService
    {
        Task<string> UploadFileAsync(IFormFile file);
        Task DeleteFileAsync(string fileName);
    }
}