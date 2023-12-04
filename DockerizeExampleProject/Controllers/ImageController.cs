using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace DockerizeExampleProject.Controllers
{
   public class ImageController : Controller
   {
      IFileProvider _fileProvider;
        IConfiguration _configuration;
      public ImageController(IFileProvider fileProvider, IConfiguration configuration)
      {
         _fileProvider = fileProvider;
            _configuration=configuration;
      }
      public IActionResult Index()
      {
            var dbName= _configuration["ConnectionStrings:DB"].ToString();
            ViewData["DBName"] = dbName;
         List<string> imageNames = _fileProvider.GetDirectoryContents("wwwroot/images").Select(x => x.Name).ToList();
         return View(imageNames);
      }
      public IActionResult Add()
      {
         return View();
      }
      [HttpPost]
      public async Task<IActionResult> Add(IFormFile imagefile)
      {
         if (imagefile.Length > 0)
         {
            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", $"{Guid.NewGuid().ToString().Replace("-", "")}{Path.GetExtension(imagefile.FileName)}");
            using FileStream stream = new FileStream(imagePath, FileMode.Create);
            await imagefile.CopyToAsync(stream);
         }

         return RedirectToAction("Index");
      }
      public IActionResult Remove(string name)
      {
         string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", name);
         FileInfo file = new FileInfo(imagePath);
         file.Delete();

         return RedirectToAction("Index");
      }
   }
}