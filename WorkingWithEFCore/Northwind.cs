using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WorkingWithEFCore
{
  public class Northwind : DbContext
  {
    public DbSet<Category>? Categories { get; set; }
    public DbSet<Product>? Products { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      string connection =
          "Data Source=.;" +
          "Initial Catalog=Northwind;" +
          "Integrated Security=true;" +
          "MultipleActiveResultSets=true;";
      optionsBuilder.UseSqlServer(connection);
      optionsBuilder.UseLazyLoadingProxies();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      // example of using Fluent API isntead of attributes
      // to limit the length of a ctageory name to 15
      modelBuilder.Entity<Category>()
          .Property(category => category.CategoryName)
          .IsRequired() // NOT NULL
          .HasMaxLength(15);

      // global filter to remove discontinued products
      modelBuilder.Entity<Product>()
        .HasQueryFilter(p => !p.Discontinued);
    }
  }
}
