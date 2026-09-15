using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales;

/// <summary>
/// Command for retrieving a paginated list of sales.
/// </summary>
public class GetSalesCommand : IRequest<GetSalesResult>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public GetSalesCommand()
    {
    }

    public GetSalesCommand(int page, int pageSize)
    {
        Page = page;
        PageSize = pageSize;
    }
}
