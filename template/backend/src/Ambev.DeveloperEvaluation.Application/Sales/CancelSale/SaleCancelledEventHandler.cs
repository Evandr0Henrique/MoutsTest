using MediatR;
using Microsoft.Extensions.Logging;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

/// <summary>
/// Handles the <see cref="SaleCancelledEvent"/> notification by logging it.
/// </summary>
public class SaleCancelledEventHandler : INotificationHandler<SaleCancelledEvent>
{
    private readonly ILogger<SaleCancelledEventHandler> _logger;

    public SaleCancelledEventHandler(ILogger<SaleCancelledEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(SaleCancelledEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Event: SaleCancelled - {@Event}", notification);
        return Task.CompletedTask;
    }
}
