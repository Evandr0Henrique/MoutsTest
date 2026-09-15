using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Profile for mapping between Sale entity and CreateSale command/result.
/// </summary>
public class CreateSaleProfile : Profile
{
    public CreateSaleProfile()
    {
        CreateMap<Sale, CreateSaleResult>();
        CreateMap<SaleItem, Ambev.DeveloperEvaluation.Application.Sales.Common.SaleItemResult>();
    }
}
