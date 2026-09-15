using AutoMapper;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Xunit;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

/// <summary>
/// Contains unit tests for the <see cref="UpdateSaleHandler"/> class.
/// </summary>
public class UpdateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly UpdateSaleHandler _handler;

    public UpdateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _mediator = Substitute.For<IMediator>();
        _handler = new UpdateSaleHandler(_saleRepository, _mapper, _mediator);
    }

    private static UpdateSaleCommand CreateValidCommand(Guid saleId)
    {
        return new UpdateSaleCommand
        {
            Id = saleId,
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Jane Doe",
            BranchId = Guid.NewGuid(),
            BranchName = "Main Branch",
            Items = new List<SaleItemDto>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Product 1",
                    Quantity = 3,
                    UnitPrice = 15m
                }
            }
        };
    }

    [Fact(DisplayName = "Given valid sale data When updating sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var saleId = Guid.NewGuid();
        var command = CreateValidCommand(saleId);
        var existingSale = new Sale { Id = saleId, SaleNumber = "SALE-0001" };
        var result = new UpdateSaleResult { Id = saleId };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(existingSale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Sale>());
        _mapper.Map<UpdateSaleResult>(Arg.Any<Sale>()).Returns(result);

        // When
        var updateSaleResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        updateSaleResult.Should().NotBeNull();
        updateSaleResult.Id.Should().Be(saleId);
        await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given valid sale data When updating sale Then publishes SaleModifiedEvent")]
    public async Task Handle_ValidRequest_PublishesSaleModifiedEvent()
    {
        // Given
        var saleId = Guid.NewGuid();
        var command = CreateValidCommand(saleId);
        var existingSale = new Sale { Id = saleId, SaleNumber = "SALE-0001" };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(existingSale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Sale>());

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _mediator.Received(1).Publish(
            Arg.Is<SaleModifiedEvent>(e => e.Sale.Id == saleId),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given a non-existent sale When updating sale Then throws KeyNotFoundException")]
    public async Task Handle_SaleNotFound_ThrowsKeyNotFoundException()
    {
        // Given
        var saleId = Guid.NewGuid();
        var command = CreateValidCommand(saleId);

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "Given invalid sale data When updating sale Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new UpdateSaleCommand
        {
            Id = Guid.Empty,
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.Empty,
            CustomerName = string.Empty,
            BranchId = Guid.Empty,
            BranchName = string.Empty,
            Items = new List<SaleItemDto>()
        };

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<ValidationException>();
    }
}
