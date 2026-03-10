using Microsoft.EntityFrameworkCore;
using COS4040A.Models;
using System.Collections.Generic;

namespace COS4040A.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}