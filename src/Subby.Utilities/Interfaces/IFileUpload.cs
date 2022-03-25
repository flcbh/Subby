using Microsoft.AspNetCore.Http;

namespace Subby.Utilities.Interfaces
{
    public interface IFileUpload
    {
        string Upload(IFormFile file);
    }
}