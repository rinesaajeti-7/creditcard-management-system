using Microsoft.EntityFrameworkCore;
using CreditCard.Models;

namespace CreditCard.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<CreditCard.Models.CreditCard> CreditCards { get; set; }
    }
}
