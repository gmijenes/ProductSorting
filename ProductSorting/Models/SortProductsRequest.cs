namespace ProductSorting.Models
{
    public class SortProductsRequest
    {
        public double SalesWeight { get; set; }
        public double StockWeight { get; set; }
        public List<ProductSales> ProductSales { get; set; } = null;
        public List<ProductStock> ProductStock { get; set; } = null;
    }

    public class ProductSales
    {
        public string ProductId { get; set; }
        public double Sales { get; set; }
    }

    public class ProductStock
    {
        public string ProductId { get; set; }
        public int Stock { get; set; }
    }
}
