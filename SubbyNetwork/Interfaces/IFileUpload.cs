using Microsoft.AspNetCore.Http;

namespace SubbyNetwork.Interfaces
{
    public interface IFileUpload
    {
        string Upload(IFormFile file);
    }
}