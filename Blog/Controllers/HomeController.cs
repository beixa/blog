using Blog.Data.FileManager;
using Blog.Data.Repository;
using Blog.Models.Comments;
using Blog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public IActionResult Index(int pageNumber, string category)
        {
            if (pageNumber < 1)
                return RedirectToAction("Index", new { pageNumber = 1, category});

            var viewModel = _repo.GetAllPosts(pageNumber, category);

            return View(viewModel);
        }

        public IActionResult Post (int id) => View(_repo.GetPost(id));        

        [HttpGet("/Image/{image}")] //its gonna look for this path
        [ResponseCache(CacheProfileName="Monthly")]
        public IActionResult Image(string image)
        {
            var mime = image.Split('.')[1];
            return new FileStreamResult(_fileManager.ImageStream(image), $"image/{mime}");
        }

        [HttpPost]
        public async Task<IActionResult> Comment ( CommentViewModel viewModel)
        {
            if(!ModelState.IsValid)
                return RedirectToAction("Post", new { id = viewModel.PostId });

            var post = _repo.GetPost(viewModel.PostId);

            if (viewModel.MainCommentId == 0)
            {
                post.MainComments = post.MainComments ?? new List<MainComment>();

                post.MainComments.Add(new MainComment
                {
                    Message = viewModel.Message,
                    Created = DateTime.Now,
                });

                _repo.UpdatePost(post);
            }
            else
            {
                var comment = new SubComment
                {
                    MainCommentId = viewModel.MainCommentId,
                    Message = viewModel.Message,
                    Created = DateTime.Now, 
                };
                _repo.AddSubComment(comment);
            }

            await _repo.SaveChangesAsync();

            return RedirectToAction("Post", new { id = viewModel.PostId });
        }
    }
}
