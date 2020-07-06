using Blog.Data.FileManager;
using Blog.Data.Repository;
using Blog.Models;
using Blog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PanelController : Controller
    {
        private readonly IRepository _repo;
        private readonly IFileManager _fileManager;

        public PanelController(IRepository repo, IFileManager fileManager)
        {
            _repo = repo;
            _fileManager = fileManager;
        }
        
        public IActionResult Index() // el nombre de la accion corresponde al archivo index.cshtml en la carpeta Panel por Panelcontroller
        {
            var posts = _repo.GetAllPosts();
            return View(posts);
        }

        [HttpGet]
        public IActionResult Edit(int? id) // when we are creating can be null so we make it nullable
        {
            if (id == null)
            {
                return View(new PostViewModel());
            }
            else
            {
                var post = _repo.GetPost((int)id); //we need to cast it 
                return View(new PostViewModel 
                {
                    Id = post.Id,
                    Title = post.Title,
                    Body = post.Body,
                    Description = post.Description,
                    Tags = post.Tags,
                    Category = post.Category,
                    CurrentImage = post.Image,                    
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PostViewModel viewModel)
        {
            var post = new Post
            {
                Id = viewModel.Id,
                Title = viewModel.Title,
                Body = viewModel.Body,
                Description = viewModel.Description,
                Tags = viewModel.Tags,
                Category = viewModel.Category,
            };

            if(viewModel.Image == null)
            {
                post.Image = viewModel.CurrentImage;
            }
            else
            {
                if (!string.IsNullOrEmpty(viewModel.CurrentImage))
                    _fileManager.RemoveImage(viewModel.CurrentImage);
               post.Image = await _fileManager.SaveImage(viewModel.Image);
            }

            if (post.Id > 0)
                _repo.UpdatePost(post);
            else
                _repo.AddPost(post);

            if (await _repo.SaveChangesAsync())
                return RedirectToAction("Index");
            else
                return View(post);
        }

        [HttpGet]
        public async Task<IActionResult> Remove(int id)
        {
            _repo.RemovePost(id);
            await _repo.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
