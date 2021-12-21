using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Vesuvio.WebAPI.Helpers
{
    public static class CommonHelper
    {
        private static readonly string urlPattern = "[^a-zA-Z0-9-.]";

        public static string SaveImage(string path, IFormFile file)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var fileName = CreateSafeName(file.FileName);
            var filePath = Path.Combine(path, fileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            return fileName;
        }

        public static void RemoveFile(string path, string fileName)
        {
            var filePath = Path.Combine(path, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        private static string CreateSafeName(string fileName)
        {
            fileName = fileName.UrlEncode();

            var extension = fileName.Split('.').Last();
            var newName = $"{fileName.Replace($".{extension}", "")}_{DateTime.Now.Ticks}.{extension}";

            return newName;
        }

        private static string UrlEncode(this string url)
        {
            var friendlyUrl = Regex.Replace(url, @"\s", "-").ToLower();
            friendlyUrl = Regex.Replace(friendlyUrl, urlPattern, string.Empty);
            return friendlyUrl;
        }
    }
}