using System.Threading.Tasks;
using ReusableServices.Common.Messages;
using ReusableServices.Common.RabbitMq;

namespace ReusableServices.Common.Handlers
{
  public interface ICommandHandler<in TCommand> where TCommand : ICommand
  {
    Task HandleAsync(TCommand command, ICorrelationContext context);
  }
}