using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Common;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// Profile for mapping between Application and API UpdateSale requests/responses
/// </summary>
public class UpdateSaleProfile : Profile
{
    public UpdateSaleProfile()
    {
        CreateMap<SaleItemRequest, SaleItemDto>();
        CreateMap<UpdateSaleRequest, UpdateSaleCommand>();

        CreateMap<SaleItemResult, SaleItemResponse>();
        CreateMap<UpdateSaleResult, UpdateSaleResponse>();
    }
}
