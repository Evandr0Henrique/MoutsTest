using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Events
{
    public class SaleDeletedEvent : INotification
    {
        public Guid SaleId { get; }

        public SaleDeletedEvent(Guid saleId)
        {
            SaleId = saleId;
        }
    }
}
