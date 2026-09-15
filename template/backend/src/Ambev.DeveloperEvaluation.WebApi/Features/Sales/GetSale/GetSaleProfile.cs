using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Common;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;

/// <summary>
/// Profile for mapping GetSale feature requests/results to commands/responses
/// </summary>
public class GetSaleProfile : Profile
{
    public GetSaleProfile()
    {
        CreateMap<Guid, Application.Sales.GetSale.GetSaleCommand>()
            .ConstructUsing(id => new Application.Sales.GetSale.GetSaleCommand(id));

        CreateMap<SaleItemResult, SaleItemResponse>();
        CreateMap<Application.Sales.GetSale.GetSaleResult, GetSaleResponse>();
    }
}
