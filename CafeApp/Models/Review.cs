using System;
using System.Collections.Generic;

namespace CafeApp.Models
{
    public partial class Review
    {
        public string Id { get; set; } = null!;
        public string? Comment { get; set; }
        public string ProductId { get; set; } = null!;
        public string UserId { get; set; } = null!;

        public virtual Product Product { get; set; } = null!;
    }
}
