using Blog.Data.FileManager;
using Blog.Data.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository _repo;
        private readonly IFileManager _fileManager;

        public HomeController(IRepository repo, IFileManager fileManager)
        {
            _repo = repo;
            _fileManager = fileManager;
        }

        // el nombre de la accion corresponde al archivo index.cshtml en la carpeta Home por Homecontroller
        public IActionResult Index(string category) => View(string.IsNullOrEmpty(category) ? _repo.GetAllPosts() : _repo.GetAllPosts(category)); 

        public IActionResult Post (int id) => View(_repo.GetPost(id));        

        [HttpGet("/Image/{image}")] //its gonna look for this path
        [ResponseCache(CacheProfileName="Monthly")]
        public IActionResult Image(string image)
        {
            var mime = image.Split('.')[1];
            return new FileStreamResult(_fileManager.ImageStream(image), $"image/{mime}");
        }
    }
}
