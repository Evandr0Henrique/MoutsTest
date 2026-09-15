using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Common;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// Profile for mapping between Application and API CreateSale requests/responses
/// </summary>
public class CreateSaleProfile : Profile
{
    public CreateSaleProfile()
    {
        CreateMap<SaleItemRequest, SaleItemDto>();
        CreateMap<CreateSaleRequest, CreateSaleCommand>();

        CreateMap<SaleItemResult, SaleItemResponse>();
        CreateMap<CreateSaleResult, CreateSaleResponse>();
    }
}
