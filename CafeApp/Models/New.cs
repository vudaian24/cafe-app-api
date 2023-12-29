using System;
using System.Collections.Generic;

namespace CafeApp.Models
{
    public partial class New
    {
        public string Id { get; set; } = null!;
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }
}
