using Microsoft.EntityFrameworkCore;
using CreditCard.Models;

namespace CreditCard.Data
{
    // Klasa AppDbContext që trashëgon nga DbContext e Entity Framework Core
    public class AppDbContext : DbContext
    {
        // Konstruktori që merr opsionet për konfigurimin e DbContext
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        // DbSet për entitetin User - përfaqëson tabelën Users në databazë
        public DbSet<User> Users { get; set; }
        
        // DbSet për entitetin CreditCard - përfaqëson tabelën CreditCards në databazë
        // Specifikohet namespace i plotë për të shmangur konfliktet e emrave
        public DbSet<CreditCard.Models.CreditCard> CreditCards { get; set; }
    }
}