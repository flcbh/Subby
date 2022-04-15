using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SubbyNetwork.Extensions
{
    public static class FormFileExtensions
    {
        public static byte[] GetBytes(this IFormFile formFile)
        {
            using (var memoryStream = new MemoryStream())
            {
                formFile.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}