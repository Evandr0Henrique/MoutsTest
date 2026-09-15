namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSales;

/// <summary>
/// Request model for retrieving a paginated list of sales
/// </summary>
public class GetSalesRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
