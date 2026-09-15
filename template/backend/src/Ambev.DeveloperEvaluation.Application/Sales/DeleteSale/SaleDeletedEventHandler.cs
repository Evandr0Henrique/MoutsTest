using MediatR;
using Microsoft.Extensions.Logging;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

/// <summary>
/// Handles the <see cref="SaleDeletedEvent"/> notification by logging it.
/// </summary>
public class SaleDeletedEventHandler : INotificationHandler<SaleDeletedEvent>
{
    private readonly ILogger<SaleDeletedEventHandler> _logger;

    public SaleDeletedEventHandler(ILogger<SaleDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(SaleDeletedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Event: SaleDeleted - {@Event}", notification);
        return Task.CompletedTask;
    }
}
