using Microsoft.EntityFrameworkCore;

namespace CreditAccountDAL
{
    public class AccountContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }

        public AccountContext(DbContextOptions<AccountContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().Property(x => x.Id).UseIdentityColumn(int.MinValue+ 1);
            modelBuilder.Entity<Account>().Property(x => x.Id).UseIdentityColumn(int.MinValue + 1);
            modelBuilder.Entity<Account>().HasIndex(x => new { x.UserId, x.CurrencyCode }).IncludeProperties(x => x.Money).IsUnique();
            modelBuilder.Entity<User>().HasData(new User { Name = "Fedor", Id = long.MinValue });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLazyLoadingProxies();
        }
    }
}
