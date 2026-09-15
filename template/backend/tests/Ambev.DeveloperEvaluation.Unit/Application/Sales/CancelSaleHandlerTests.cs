using FluentAssertions;
using MediatR;
using NSubstitute;
using Xunit;
using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

/// <summary>
/// Contains unit tests for the <see cref="CancelSaleHandler"/> class.
/// </summary>
public class CancelSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMediator _mediator;
    private readonly CancelSaleHandler _handler;

    public CancelSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mediator = Substitute.For<IMediator>();
        _handler = new CancelSaleHandler(_saleRepository, _mediator);
    }

    [Fact(DisplayName = "Given an existing sale When cancelling sale Then returns success response")]
    public async Task Handle_ExistingSale_ReturnsSuccessResponse()
    {
        // Given
        var saleId = Guid.NewGuid();
        var command = new CancelSaleCommand(saleId);
        var sale = new Sale { Id = saleId, SaleNumber = "SALE-0001" };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Sale>());

        // When
        var response = await _handler.Handle(command, CancellationToken.None);

        // Then
        response.Should().NotBeNull();
        response.Success.Should().BeTrue();
        sale.IsCancelled.Should().BeTrue();
    }

    [Fact(DisplayName = "Given an existing sale When cancelling sale Then publishes SaleCancelledEvent")]
    public async Task Handle_ExistingSale_PublishesSaleCancelledEvent()
    {
        // Given
        var saleId = Guid.NewGuid();
        var command = new CancelSaleCommand(saleId);
        var sale = new Sale { Id = saleId, SaleNumber = "SALE-0001" };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Sale>());

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _mediator.Received(1).Publish(
            Arg.Is<SaleCancelledEvent>(e => e.Sale.Id == saleId),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given a non-existent sale When cancelling sale Then throws KeyNotFoundException")]
    public async Task Handle_SaleNotFound_ThrowsKeyNotFoundException()
    {
        // Given
        var saleId = Guid.NewGuid();
        var command = new CancelSaleCommand(saleId);

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "Given an empty sale ID When cancelling sale Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new CancelSaleCommand(Guid.Empty);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<ValidationException>();
    }
}
