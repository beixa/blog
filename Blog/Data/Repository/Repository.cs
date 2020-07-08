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
            int pageSize = 2;
            int skipAmount = pageSize * (pageNumber - 1);

            var query = _context.Posts.AsQueryable();

            if (!String.IsNullOrEmpty(category))
                query = query.Where(x => x.Category.ToLower().Equals(category.ToLower()));

            var postCount = query.Count();
            var pageCount = (int)Math.Ceiling((double)postCount / pageSize);

            return new IndexViewModel
            {
                PageNumber = pageNumber,
                PageCount = pageCount,
                NextPage = postCount > pageSize * (pageNumber - 1) + pageSize,
                Pages = PageNumbers(pageNumber, pageCount),
                Category = category,
                Posts = query.Skip(skipAmount).Take(pageSize).ToList(),
            };
        }

        private IEnumerable<int> PageNumbers(int pageNumber, int pageCount)
        {
            int midPoint = pageNumber;

            if (midPoint < 3)
                midPoint = 3;
            else if (midPoint > pageCount - 2)
                if (pageCount - 2 < 3)
                    midPoint = 3;
                else
                    midPoint = pageCount - 2;

            int lowerBound = midPoint - 2;
            int upperBound = midPoint + 2;           

            if (lowerBound != 1)
            {
                yield return 1;
                if (lowerBound -1 > 1)
                {
                    yield return -1;
                }
            }

            for (int i = midPoint - 2; i <= midPoint + 2; i++)
            {
                if (i <= pageCount)
                    yield return i;
            }

            if (upperBound != pageCount && upperBound < pageCount)
            {
                if (pageCount - upperBound > 1)
                {
                    yield return -1;
                }
                yield return pageCount;
            }
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
