using AutoMapper;
using FluentValidation;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Handler for processing CreateSaleCommand requests
/// </summary>
public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public CreateSaleHandler(ISaleRepository saleRepository, IMapper mapper, IMediator mediator)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = new Sale
        {
            SaleNumber = string.IsNullOrWhiteSpace(command.SaleNumber)
                ? GenerateSaleNumber()
                : command.SaleNumber,
            SaleDate = command.SaleDate,
            CustomerId = command.CustomerId,
            CustomerName = command.CustomerName,
            BranchId = command.BranchId,
            BranchName = command.BranchName
        };

        foreach (var item in command.Items)
            sale.AddItem(item.ProductId, item.ProductName, item.Quantity, item.UnitPrice);

        var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);

        await _mediator.Publish(new SaleCreatedEvent(createdSale), cancellationToken);

        return _mapper.Map<CreateSaleResult>(createdSale);
    }

    private static string GenerateSaleNumber()
    {
        return $"SALE-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";
    }
}
