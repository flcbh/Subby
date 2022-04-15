using System.IO;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using SubbyNetwork.Extensions;
using SubbyNetwork.Interfaces;

namespace SubbyNetwork.Services
{
    public class FileUpload : IFileUpload
    {

        private readonly Account _account;

        public FileUpload()
        {
            _account = new Account(
                "subbynetwork",
                "566515418817742",
                "x2cCbnaUCeXE1L2GwNZJ4C10z74");
        }

        public string Upload(IFormFile file)
        {
            Stream stream = new MemoryStream(file.GetBytes());
            var cloudinary = new Cloudinary(_account);
            
            if (file.FileName.Contains(".pdf") || file.FileName.Contains(".doc") || file.FileName.Contains(".docx"))
            {
                var uploadParams = new RawUploadParams(){
                    File = new FileDescription(file.FileName, stream),
                    RawConvert = "aspose",
                    Folder = "subby-network"
                };
                
                var uploadResult = cloudinary.UploadAsync(uploadParams).Result;
                return uploadResult.Uri.ToString();
            }
            else
            {
                var uploadParams = new  ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "subby-network",
                    Transformation = new Transformation().Width(400).Height(300).Crop("fill")
                };

                var uploadResult = cloudinary.UploadAsync(uploadParams).Result;
                return uploadResult.Uri.ToString();
            }
        }
    }
}