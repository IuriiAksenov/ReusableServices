using System;
using ReusableServices.Common.Exceptions;
using ReusableServices.Common.Messages;

namespace ReusableServices.Common.RabbitMq
{
  public interface IBusSubscriber
  {
    IBusSubscriber SubscribeCommand<TCommand>(string @namespace = null, string queueName = null,
      Func<TCommand, OCSException, IRejectedEvent> onError = null) where TCommand : ICommand;

    IBusSubscriber SubscribeEvent<TEvent>(string @namespace = null, string queueName = null,
      Func<TEvent, OCSException, IRejectedEvent> onError = null) where TEvent : IEvent;
  }
}