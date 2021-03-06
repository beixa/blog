﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoSauce.MagicScaler;

namespace Blog.Data.FileManager
{
    public class FileManager : IFileManager
    {
        private readonly string _imagePath;

        public FileManager(IConfiguration config) //config for the path defined in appsettings
        {
            _imagePath = config["Path:Images"];
        }

        public FileStream ImageStream(string image)
        {
            return new FileStream(Path.Combine(_imagePath, image), FileMode.Open, FileAccess.Read);
        }

        public bool RemoveImage(string image)
        {
            try
            {
                var file = Path.Combine(_imagePath, image);

                if (File.Exists(file))
                    File.Delete(file);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }           
        }

        public async Task<string> SaveImage(IFormFile image)
        {
            try
            {            
                var save_path = Path.Combine(_imagePath); //use this to avoid errors using normal string
                if(!Directory.Exists(save_path))
                {
                    Directory.CreateDirectory(save_path);
                }

                //Internet Explorer Error
                //var fileName = image.FileName;
                                
                var mime = image.FileName.Substring(image.FileName.LastIndexOf('.'));
                var fileName = $"image_{DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss")}{mime}";

                //a Stream is like a tunnel to the folder
                using (var fileStream = new FileStream(Path.Combine(save_path, fileName), FileMode.Create)) //using limits an IDisposable object to this scope
                {
                    //await image.CopyToAsync(fileStream);
                    MagicImageProcessor.ProcessImage(image.OpenReadStream(), fileStream, imageOptions());
                }

                return fileName;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "Error";
            }
        }

        private ProcessImageSettings imageOptions() => new ProcessImageSettings
        {
            Width = 800,
            Height = 500,
            ResizeMode = CropScaleMode.Crop,
            SaveFormat = FileFormat.Jpeg,
            JpegQuality = 100,
            JpegSubsampleMode = ChromaSubsampleMode.Subsample420,
        };
    }
}
