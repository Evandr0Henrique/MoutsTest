using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Sales.GetSales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSales;

/// <summary>
/// Profile for mapping GetSales feature requests/results to commands/responses
/// </summary>
public class GetSalesProfile : Profile
{
    public GetSalesProfile()
    {
        CreateMap<GetSalesRequest, GetSalesCommand>();
        CreateMap<Application.Sales.GetSale.GetSaleResult, GetSaleResponse>();
    }
}
