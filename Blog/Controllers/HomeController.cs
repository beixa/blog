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

        public IActionResult Index() // el nombre de la accion corresponde al archivo index.cshtml en la carpeta Home por Homecontroller
        {
            var posts = _repo.GetAllPosts();
            return View(posts);
        }

        public IActionResult Post(int id)
        {
            var post = _repo.GetPost(id);
            return View(post);
        }

        [HttpGet("/Image/{image}")] //its gonna look for this path
        public IActionResult Image(string image)
        {
            var mime = image.Split('.')[1];
            return new FileStreamResult(_fileManager.ImageStream(image), $"image/{mime}");
        }
    }
}
