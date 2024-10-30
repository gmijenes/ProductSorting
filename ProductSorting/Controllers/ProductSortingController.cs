using Microsoft.AspNetCore.Mvc;
using ProductSorting.Models;

namespace ProductSorting.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ProductSortingController : ControllerBase
    {
        [HttpPost("sort-products")]
        public IActionResult SortProducts(SortProductsRequest request)
        {
            if (request.SalesWeight + request.StockWeight != 1)
                {
                    return BadRequest( new
                    {
                        type = "Weight format error",
                        title = "One or more validation errors occurred.",
                        status = 400,
                        errors = new
                        {
                            request = new[] { "The sum of salesWeight and stockWeight must be equal to 1." }
                        }
                    });
                }

            // Left Join desde ProductStock hacia ProductSales
            var leftJoinProducts = request.ProductStock
                .GroupJoin(
                    request.ProductSales,
                    stock => stock.ProductId,
                    sales => sales.ProductId,
                    (stock, sales) => new { stock.ProductId, Stock = stock.Stock, Sales = sales.DefaultIfEmpty() }
                )
                .SelectMany(
                    result => result.Sales.Select(sale => new
                    {
                        ProductId = result.ProductId,
                        Stock = result.Stock,
                        Sales = sale?.Sales ?? 0 // Si sales es null, asigna 0
            })
                );

            // Calcula la prioridad según los pesos de ventas y stock
            var sortedProducts = leftJoinProducts
                .OrderByDescending(product => (product.Sales * request.SalesWeight) + (product.Stock * request.StockWeight))
                .Select(product => product.ProductId)
                .ToList();

            // Devuelve la lista ordenada de IDs de productos
            return Ok(sortedProducts);
        }

    }

}
