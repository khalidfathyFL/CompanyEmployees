using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Configurations;

namespace Repository
{
  public class RepositoryContext : DbContext
  {
    public DbSet<Employee>? Employees { get; set; }  
    public DbSet<Company>? Companies { get; set; }  

    public RepositoryContext(DbContextOptions<RepositoryContext> options) :base(options)
    {
      
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.ApplyConfiguration(new CompanyConfiguration());
      modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
    }
  }
}
