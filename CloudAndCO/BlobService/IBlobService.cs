using Microsoft.AspNetCore.Http;

namespace CloudAndCO.Services
{
    // Interface for BlobService
    public interface IBlobService
    {
        Task<string> UploadFileAsync(IFormFile file);
        Task DeleteFileAsync(string fileName);
    }
}