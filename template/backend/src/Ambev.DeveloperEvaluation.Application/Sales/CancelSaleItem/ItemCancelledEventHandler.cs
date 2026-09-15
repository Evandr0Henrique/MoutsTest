using MediatR;
using Microsoft.Extensions.Logging;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;

/// <summary>
/// Handles the <see cref="ItemCancelledEvent"/> notification by logging it.
/// </summary>
public class ItemCancelledEventHandler : INotificationHandler<ItemCancelledEvent>
{
    private readonly ILogger<ItemCancelledEventHandler> _logger;

    public ItemCancelledEventHandler(ILogger<ItemCancelledEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(ItemCancelledEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Event: ItemCancelled - {@Event}", notification);
        return Task.CompletedTask;
    }
}
