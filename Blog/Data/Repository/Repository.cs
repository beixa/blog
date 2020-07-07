using Blog.Models;
using Blog.Models.Comments;
using Blog.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Data.Repository
{
    public class Repository : IRepository
    {
        private readonly AppDbContext _context;

        public Repository(AppDbContext context)
        {
            _context = context;
        }
        public void AddPost(Post post)
        {
            _context.Posts.Add(post);
        }

        public List<Post> GetAllPosts()
        {
            return _context.Posts.ToList();
        }

        public IndexViewModel GetAllPosts(int pageNumber, string category)
        {
            int pageSize = 5;
            int skipAmount = pageSize * (pageNumber - 1);
            //var query = _context.Posts
            //                    .Skip()
            //                    .Take(pageSize)
            //                    .ToList();

            var query = _context.Posts.AsQueryable();

            if (!String.IsNullOrEmpty(category))
                query = query.Where(x => x.Category.ToLower().Equals(category.ToLower()));

            var postCount = query.Count();
            
            return new IndexViewModel
            {
                PageNumber = pageNumber,
                NextPage = postCount > pageSize * (pageNumber - 1) + pageSize,
                Category = category,
                Posts = query.Skip(skipAmount).Take(pageSize).ToList(),
            };
        }

        public Post GetPost(int id)
        {
            return _context.Posts
                .Include(p => p.MainComments)
                    .ThenInclude(mc => mc.SubComments)
                .FirstOrDefault(p => p.Id == id);
        }

        public void RemovePost(int id)
        {
            _context.Posts.Remove(GetPost(id));
        }        

        public void UpdatePost(Post post)
        {
            _context.Posts.Update(post);
        }

        public async Task<bool> SaveChangesAsync()
        {
            if( await _context.SaveChangesAsync() > 0) //aqui es donde se guarda en base de datos los Add, Remove y demas solo hacen un seguimiento
            {
                return true;
            }
            return false;
        }

        public void AddSubComment(SubComment comment)
        {
            _context.SubComments.Add(comment);
        }
    }
}
