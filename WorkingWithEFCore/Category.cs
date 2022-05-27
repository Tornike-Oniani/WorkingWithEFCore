using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkingWithEFCore
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        [Column(TypeName = "ntext")]
        public string? Description { get; set; }

        // defines a navigation property for related rows
        public virtual ICollection<Product> Products { get; set; }

        public Category()
        {
            // to enable developers to add products to a Category we must
            // initialize the navigation property to an empty collection
            Products = new HashSet<Product>();
        }
    }
}
