using Ambev.DeveloperEvaluation.Application.Sales.GetSale;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales;

/// <summary>
/// Represents a page of sales results.
/// </summary>
public class GetSalesResult
{
    public List<GetSaleResult> Items { get; set; } = new();
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
}
