using System.Threading.Tasks;
using ReusableServices.Common.Messages;
using ReusableServices.Common.RabbitMq;

namespace ReusableServices.Common.Handlers
{
  public interface IEventHandler<in TEvent> where TEvent : IEvent
  {
    Task HandleAsync(TEvent @event, ICorrelationContext context);
  }
}