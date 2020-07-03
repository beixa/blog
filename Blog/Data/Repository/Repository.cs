using Blog.Models;
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

        public List<Post> GetAllPosts(string category)
        {
            return _context.Posts.Where(post => post.Category.ToLower().Equals(category.ToLower())).ToList();
        }

        public Post GetPost(int id)
        {
            return _context.Posts.FirstOrDefault(p => p.Id == id);
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
       
    }
}
