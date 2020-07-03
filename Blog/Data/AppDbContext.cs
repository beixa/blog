using Blog.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Blog.Data
{
    public class AppDbContext : IdentityDbContext //IdentityDbContext has all the dbsets for identity
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }//the base will send the options to the original constructor

        public DbSet<Post> Posts { get; set; } //Basically says: Create a table and each row equals to Post
    }
}
