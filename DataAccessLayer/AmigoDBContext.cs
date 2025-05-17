using Amigos.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Amigos.DataAccessLayer
{
    public class AmigoDBContext : DbContext
    {
        public DbSet<Amigo>? Amigos { get; set; }

        public AmigoDBContext(DbContextOptions<AmigoDBContext> options)
           : base(options)
        {
        }
    }
}
