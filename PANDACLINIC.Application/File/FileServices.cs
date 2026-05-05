using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
namespace PANDACLINIC.Application.FileService
{  //I/*HostingEnvironment  _env;*/
   public class FileServices : IFileService
    {
        private readonly string _storagePath;

        public FileServices(string storagePath)
        {
            _storagePath = storagePath;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0) return null;

            var targetDirectory = Path.Combine(_storagePath, folderName);

            if (!Directory.Exists(targetDirectory))
                Directory.CreateDirectory(targetDirectory);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var fullPath = Path.Combine(targetDirectory, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);
            return $"/uploads/{folderName}/{fileName}";
        }

        public void DeleteFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return;
            var relativePath = filePath.Replace("/uploads/", "").Replace("/", "\\");
            var fullPath = Path.Combine(_storagePath, relativePath);

            if (File.Exists(fullPath)) File.Delete(fullPath);
        }
    }
}
