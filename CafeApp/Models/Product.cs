using System;
using System.Collections.Generic;

namespace CafeApp.Models
{
    public partial class Product
    {
        public Product()
        {
            Reviews = new HashSet<Review>();
        }

        public string Id { get; set; } = null!;
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string BrandId { get; set; } = null!;
        public string CategoryId { get; set; } = null!;

        public virtual Brand Brand { get; set; } = null!;
        public virtual Category Category { get; set; } = null!;
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
