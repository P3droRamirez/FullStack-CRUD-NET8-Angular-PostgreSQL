using BackendAppProducto.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendAppProducto.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
    public DbSet<Product> Products { get; set; }
    }
}
