using MediatR;
using Microsoft.Extensions.Logging;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Handles the <see cref="SaleCreatedEvent"/> notification by logging it.
/// </summary>
public class SaleCreatedEventHandler : INotificationHandler<SaleCreatedEvent>
{
    private readonly ILogger<SaleCreatedEventHandler> _logger;

    public SaleCreatedEventHandler(ILogger<SaleCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(SaleCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Event: SaleCreated - {@Event}", notification);
        return Task.CompletedTask;
    }
}
