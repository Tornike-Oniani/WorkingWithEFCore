// See https://aka.ms/new-console-template for more information
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using WorkingWithEFCore;

Console.WriteLine($"Using {ProjectConstants.DatabaseProvider} database provider");

QueryingProducts();

static void QueryingCategories()
{
    using (Northwind db = new())
    {
        ILoggerFactory loggerFactory = db.GetService<ILoggerFactory>();
        loggerFactory.AddProvider(new ConsoleLoggerProvider());

        Console.WriteLine("Categories and how many productes they have:");

        // a query to get all categories and their related products
        IQueryable<Category>? categories = db.Categories?.Include(c => c.Products);

        if (categories is null)
        {
            Console.WriteLine("No categories found.");
            return;
        }

        // execute query and enumerate results
        foreach (Category c  in categories)
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