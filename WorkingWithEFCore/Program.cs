using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using WorkingWithEFCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

Console.WriteLine($"Using {ProjectConstants.DatabaseProvider} database provider");

int deleted = DeleteProducts("Bob");
Console.WriteLine($"{deleted} product(s) were deleted.");

// Read
static void QueryingCategories()
{
  using (Northwind db = new())
  {
    ILoggerFactory loggerFactory = db.GetService<ILoggerFactory>();
    loggerFactory.AddProvider(new ConsoleLoggerProvider());

    Console.WriteLine("Categories and how many productes they have:");

    // a query to get all categories and their related products
    IQueryable<Category>? categories;
    // = db.Categories; 
    // .Include(c => c.Products);

    db.ChangeTracker.LazyLoadingEnabled = false;

    Console.Write("Enable eage loading? (Y/N): ");
    bool eagerLoading = Console.ReadKey().Key == ConsoleKey.Y;
    bool explicitLoading = false;
    Console.WriteLine();

    if (eagerLoading)
    {
      categories = db.Categories?.Include(c => c.Products);
    }
    else
    {
      categories = db.Categories;

      Console.WriteLine("Enable explicit loading? (Y/N): ");
      explicitLoading = (Console.ReadKey().Key == ConsoleKey.Y);
      Console.WriteLine();
    }

    if (categories is null)
    {
      Console.WriteLine("No categories found.");
      return;
    }

    // execute query and enumerate results
    foreach (Category c in categories)
    {
      Console.WriteLine($"{c.CategoryName} has {c.Products.Count} products");
    }
  }
}
static void FilteredIncludes()
{
  using (Northwind db = new())
  {
    Console.WriteLine("Enter a minimum for units in stock:");
    string unitsInStock = Console.ReadLine() ?? "10";
    int minStock = int.Parse(unitsInStock);

    IQueryable<Category>? categories = db.Categories?.Include(c => c.Products.Where(p => p.Stock >= minStock));

    if (categories is null)
    {
      Console.WriteLine("No categories found.");
      return;
    }

    // Console.WriteLine($"ToQueryString: {categories.ToQueryString()}");

    foreach (Category category in categories)
    {
      Console.WriteLine($"{category.CategoryName} has {category.Products.Count} products with a minimum of {minStock} units in stock");

      foreach (Product product in category.Products)
      {
        Console.WriteLine($"{product.ProductName} has {product.Stock} units in stock");
      }
    }
  }
}
static void QueryingProducts()
{
  using (Northwind db = new())
  {
    ILoggerFactory loggerFactory = db.GetService<ILoggerFactory>();
    loggerFactory.AddProvider(new ConsoleLoggerProvider());

    Console.WriteLine("Products that cost more thank a price, highest at top.");
    string? input;
    decimal price;

    do
    {
      Console.WriteLine("Enter a product price: ");
      input = Console.ReadLine();
    } while (!decimal.TryParse(input, out price));

    IQueryable<Product>? products = db.Products?
        .Where(product => product.Cost > price)
        .OrderByDescending(product => product.Cost);

    if (products is null)
    {
      Console.WriteLine("No products found.");
      return;
    }

    foreach (Product product in products)
    {
      Console.WriteLine("{0} : {1} costs {2:$#,##0.00} and has {3} in stock.",
          product.ProductId, product.ProductName, product.Cost, product.Stock);
    }
  }
}
static void QueryingWithLike()
{
  using (Northwind db = new())
  {
    ILoggerFactory loggerFactory = db.GetService<ILoggerFactory>();
    loggerFactory.AddProvider(new ConsoleLoggerProvider());

    Console.WriteLine("Enter part of a product name: ");
    string? input = Console.ReadLine();

    IQueryable<Product>? products = db.Products?
      .Where(p => EF.Functions.Like(p.ProductName, $"%{input}%"));

    if (products is null)
    {
      Console.WriteLine("No products found.");
      return;
    }

    foreach (Product p in products)
    {
      Console.WriteLine($"{p.ProductName} has {p.Stock} units in stock. Disconinued? {p.Discontinued}");
    }
  }
}
static void ListProcuts()
{
  using (Northwind db = new())
  {
    Console.WriteLine("{0,-3} {1,-35} {2,8} {3,5} {4}", 
      "Id", "Product Name", "Cost", "Stock", "Disc.");

    foreach (Product product in db.Products
      .OrderByDescending(product => product.Cost))
    {
      Console.WriteLine("{0:000} {1,-35} {2,8:$#,##0.00} {3,5} {4}", 
        product.ProductId, product.ProductName, product.Cost, product.Stock, product.Discontinued);
    }
  }
}
// Create
static bool AddProduct(int categoryId, string productName, decimal? price)
{
  using (Northwind db = new())
  {
    Product p = new() 
    { 
      CategoryId = categoryId, 
      ProductName = productName, 
      Cost = price 
    };

    // mark product as added in change tracking
    db.Products.Add(p);

    // save tracked change to database
    int affected = db.SaveChanges();
    return affected == 1;
  }
}
// Update
static bool IncreaseProductPrice(string productNameStartsWith, decimal amount)
{
  using (Northwind db = new())
  {
    // get first product whose name starts with input
    Product updateProcut = db.Products.First(p => p.ProductName.StartsWith(productNameStartsWith));

    updateProcut.Cost += amount;

    int affected = db.SaveChanges();
    return affected == 1;
  }
}
// Delete
static int DeleteProducts(string productNameStartsWith)
{
  using (Northwind db = new())
  {
    using (IDbContextTransaction t = db.Database.BeginTransaction())
    {
      Console.WriteLine("Transaction isolation level: {0}",
        arg0: t.GetDbTransaction().IsolationLevel);

      IQueryable<Product>? products = db.Products?
        .Where(p => p.ProductName.StartsWith(productNameStartsWith));

      if (products is null)
      {
        Console.WriteLine("No products found to delete.");
        return 0;
      }

      db.Products.RemoveRange(products);
      int affected = db.SaveChanges();
      t.Commit();
      return affected;
    }
  }
}