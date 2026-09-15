using FluentAssertions;
using MediatR;
using NSubstitute;
using Xunit;
using Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

/// <summary>
/// Contains unit tests for the <see cref="CancelSaleItemHandler"/> class.
/// </summary>
public class CancelSaleItemHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMediator _mediator;
    private readonly CancelSaleItemHandler _handler;

    public CancelSaleItemHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mediator = Substitute.For<IMediator>();
        _handler = new CancelSaleItemHandler(_saleRepository, _mediator);
    }

    private static Sale CreateSaleWithItem(out Guid itemId)
    {
        var sale = new Sale { Id = Guid.NewGuid(), SaleNumber = "SALE-0001" };
        var item = sale.AddItem(Guid.NewGuid(), "Product 1", 2, 10m);
        item.Id = Guid.NewGuid();
        itemId = item.Id;
        return sale;
    }

    [Fact(DisplayName = "Given an existing sale item When cancelling item Then returns success response")]
    public async Task Handle_ExistingItem_ReturnsSuccessResponse()
    {
        // Given
        var sale = CreateSaleWithItem(out var itemId);
        var command = new CancelSaleItemCommand(sale.Id, itemId);

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Sale>());

        // When
        var response = await _handler.Handle(command, CancellationToken.None);

        // Then
        response.Should().NotBeNull();
        response.Success.Should().BeTrue();
        sale.Items.Single(i => i.Id == itemId).IsCancelled.Should().BeTrue();
    }

    [Fact(DisplayName = "Given an existing sale item When cancelling item Then publishes ItemCancelledEvent")]
    public async Task Handle_ExistingItem_PublishesItemCancelledEvent()
    {
        // Given
        var sale = CreateSaleWithItem(out var itemId);
        var command = new CancelSaleItemCommand(sale.Id, itemId);

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Sale>());

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _mediator.Received(1).Publish(
            Arg.Is<ItemCancelledEvent>(e => e.Sale.Id == sale.Id && e.Item.Id == itemId),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given a non-existent sale When cancelling item Then throws KeyNotFoundException")]
    public async Task Handle_SaleNotFound_ThrowsKeyNotFoundException()
    {
        // Given
        var saleId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var command = new CancelSaleItemCommand(saleId, itemId);

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "Given invalid IDs When cancelling item Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new CancelSaleItemCommand(Guid.Empty, Guid.Empty);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<ValidationException>();
    }
}
