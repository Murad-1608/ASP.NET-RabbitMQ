using Microsoft.EntityFrameworkCore;

namespace ASP.NET_RabbitMQ.Watermark.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Product> Products { get; set; }
    }
}
