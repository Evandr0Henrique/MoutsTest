using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSales;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

/// <summary>
/// Contains unit tests for the <see cref="GetSalesHandler"/> class.
/// </summary>
public class GetSalesHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly GetSalesHandler _handler;

    public GetSalesHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetSalesHandler(_saleRepository, _mapper);
    }

    [Fact(DisplayName = "Given existing sales When getting sales list Then returns paginated result")]
    public async Task Handle_ExistingSales_ReturnsPaginatedResult()
    {
        // Given
        var sales = new List<Sale>
        {
            new() { Id = Guid.NewGuid(), SaleNumber = "SALE-0001", SaleDate = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), SaleNumber = "SALE-0002", SaleDate = DateTime.UtcNow.AddDays(-1) }
        };

        var command = new GetSalesCommand(1, 10);

        _saleRepository.GetAllQueryable().Returns(sales.AsAsyncQueryable());
        _mapper.Map<GetSaleResult>(Arg.Any<Sale>()).Returns(callInfo =>
        {
            var sale = callInfo.Arg<Sale>();
            return new GetSaleResult { Id = sale.Id, SaleNumber = sale.SaleNumber };
        });

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.CurrentPage.Should().Be(1);
    }

    [Fact(DisplayName = "Given page size smaller than total When getting sales list Then paginates correctly")]
    public async Task Handle_PageSizeSmallerThanTotal_PaginatesCorrectly()
    {
        // Given
        var sales = Enumerable.Range(1, 5)
            .Select(i => new Sale { Id = Guid.NewGuid(), SaleNumber = $"SALE-{i:D4}", SaleDate = DateTime.UtcNow.AddDays(-i) })
            .ToList();

        var command = new GetSalesCommand(1, 2);

        _saleRepository.GetAllQueryable().Returns(sales.AsAsyncQueryable());
        _mapper.Map<GetSaleResult>(Arg.Any<Sale>()).Returns(callInfo =>
        {
            var sale = callInfo.Arg<Sale>();
            return new GetSaleResult { Id = sale.Id, SaleNumber = sale.SaleNumber };
        });

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(5);
        result.TotalPages.Should().Be(3);
    }

    [Fact(DisplayName = "Given invalid pagination data When getting sales list Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new GetSalesCommand(0, 0);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<ValidationException>();
    }
}
