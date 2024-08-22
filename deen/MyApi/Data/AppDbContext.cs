using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;



public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<HeadAdmin> HeadAdmin { get; set; }

    public DbSet<Admin> Admin { get; set; }

    public DbSet<Employee> Employee { get; set; }

    public DbSet<UserRegistration> UserRegistration { get; set; }
    public DbSet<Product> Product { get; set; }
    

}
