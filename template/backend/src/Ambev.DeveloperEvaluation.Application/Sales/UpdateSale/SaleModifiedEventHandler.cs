using MediatR;
using Microsoft.Extensions.Logging;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Handles the <see cref="SaleModifiedEvent"/> notification by logging it.
/// </summary>
public class SaleModifiedEventHandler : INotificationHandler<SaleModifiedEvent>
{
    private readonly ILogger<SaleModifiedEventHandler> _logger;

    public SaleModifiedEventHandler(ILogger<SaleModifiedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(SaleModifiedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Event: SaleModified - {@Event}", notification);
        return Task.CompletedTask;
    }
}
