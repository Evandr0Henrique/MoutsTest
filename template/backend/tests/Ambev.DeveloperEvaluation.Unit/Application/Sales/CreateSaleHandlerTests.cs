using AutoMapper;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Xunit;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

/// <summary>
/// Contains unit tests for the <see cref="CreateSaleHandler"/> class.
/// </summary>
public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _mediator = Substitute.For<IMediator>();
        _handler = new CreateSaleHandler(_saleRepository, _mapper, _mediator);
    }

    private static CreateSaleCommand CreateValidCommand()
    {
        return new CreateSaleCommand
        {
            SaleNumber = "SALE-0001",
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "John Doe",
            BranchId = Guid.NewGuid(),
            BranchName = "Main Branch",
            Items = new List<SaleItemDto>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Product 1",
                    Quantity = 2,
                    UnitPrice = 10m
                }
            }
        };
    }

    [Fact(DisplayName = "Given valid sale data When creating sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = CreateValidCommand();
        var createdSale = new Sale { Id = Guid.NewGuid(), SaleNumber = command.SaleNumber };
        var result = new CreateSaleResult { Id = createdSale.Id, SaleNumber = createdSale.SaleNumber };

        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(createdSale);
        _mapper.Map<CreateSaleResult>(createdSale).Returns(result);

        // When
        var createSaleResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        createSaleResult.Should().NotBeNull();
        createSaleResult.Id.Should().Be(createdSale.Id);
        await _saleRepository.Received(1).CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given valid sale data When creating sale Then publishes SaleCreatedEvent")]
    public async Task Handle_ValidRequest_PublishesSaleCreatedEvent()
    {
        // Given
        var command = CreateValidCommand();
        var createdSale = new Sale { Id = Guid.NewGuid(), SaleNumber = command.SaleNumber };

        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(createdSale);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _mediator.Received(1).Publish(
            Arg.Is<Ambev.DeveloperEvaluation.Domain.Events.SaleCreatedEvent>(e => e.Sale == createdSale),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given an empty sale number When creating sale Then generates a sale number automatically")]
    public async Task Handle_EmptySaleNumber_GeneratesSaleNumber()
    {
        // Given
        var command = CreateValidCommand();
        command.SaleNumber = string.Empty;

        Sale? capturedSale = null;
        _saleRepository.CreateAsync(Arg.Do<Sale>(s => capturedSale = s), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Sale>());

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        capturedSale.Should().NotBeNull();
        capturedSale!.SaleNumber.Should().NotBeNullOrWhiteSpace();
        capturedSale.SaleNumber.Should().StartWith("SALE-");
    }

    [Fact(DisplayName = "Given invalid sale data When creating sale Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new CreateSaleCommand
        {
            SaleNumber = "SALE-0002",
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
