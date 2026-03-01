using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TicketNest.Models;

namespace TicketNest.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public DbSet<Organization> Organization { get; set; }

        public DbSet<Product> Product { get; set; }

        public DbSet<Customer> Customer { get; set; }

        public DbSet<Contact> Contact { get; set; }

        public DbSet<SupportAgent> SupportAgent { get; set; }

        public DbSet<SupportEngineer> SupportEngineer { get; set; }

        public DbSet<Ticket> Ticket { get; set; }

        public DbSet<ApplicationUser> ApplicationUser { get; set; }
    }
}
