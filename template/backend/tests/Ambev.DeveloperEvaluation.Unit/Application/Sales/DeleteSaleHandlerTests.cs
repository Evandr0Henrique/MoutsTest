using FluentAssertions;
using MediatR;
using NSubstitute;
using Xunit;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

/// <summary>
/// Contains unit tests for the <see cref="DeleteSaleHandler"/> class.
/// </summary>
public class DeleteSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMediator _mediator;
    private readonly DeleteSaleHandler _handler;

    public DeleteSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mediator = Substitute.For<IMediator>();
        _handler = new DeleteSaleHandler(_saleRepository, _mediator);
    }

    [Fact(DisplayName = "Given an existing sale When deleting sale Then returns success response")]
    public async Task Handle_ExistingSale_ReturnsSuccessResponse()
    {
        // Given
        var saleId = Guid.NewGuid();
        var command = new DeleteSaleCommand(saleId);

        _saleRepository.DeleteAsync(saleId, Arg.Any<CancellationToken>()).Returns(true);

        // When
        var response = await _handler.Handle(command, CancellationToken.None);

        // Then
        response.Should().NotBeNull();
        response.Success.Should().BeTrue();
        await _saleRepository.Received(1).DeleteAsync(saleId, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given an existing sale When deleting sale Then publishes SaleDeletedEvent")]
    public async Task Handle_ExistingSale_PublishesSaleDeletedEvent()
    {
        // Given
        var saleId = Guid.NewGuid();
        var command = new DeleteSaleCommand(saleId);

        _saleRepository.DeleteAsync(saleId, Arg.Any<CancellationToken>()).Returns(true);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _mediator.Received(1).Publish(
            Arg.Is<SaleDeletedEvent>(e => e.SaleId == saleId),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given a non-existent sale When deleting sale Then throws KeyNotFoundException")]
    public async Task Handle_SaleNotFound_ThrowsKeyNotFoundException()
    {
        // Given
        var saleId = Guid.NewGuid();
        var command = new DeleteSaleCommand(saleId);

        _saleRepository.DeleteAsync(saleId, Arg.Any<CancellationToken>()).Returns(false);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>();
        await _mediator.DidNotReceive().Publish(Arg.Any<SaleDeletedEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given an empty sale ID When deleting sale Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new DeleteSaleCommand(Guid.Empty);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<ValidationException>();
    }
}
