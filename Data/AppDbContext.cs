using Microsoft.EntityFrameworkCore;
using SampleProject.Models;

namespace SampleProject.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<PaymentCard> PaymentCards { get; set; }
    }
}
