namespace CafeApp.AppModels
{
    public class ProductModel
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public string BrandName { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
    }
    public class ProductDetails
    {
        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public string? ProductImageUrl { get; set; }
        public string? BrandName { get; set; }
        public string? BrandDescription { get; set; }
        public string? CategoryName { get; set; }
        public string? CategoryDescription { get; set; }
    }

}
