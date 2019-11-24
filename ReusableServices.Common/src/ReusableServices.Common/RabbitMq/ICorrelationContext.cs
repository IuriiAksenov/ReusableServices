using System;

namespace ReusableServices.Common.RabbitMq
{
  public interface ICorrelationContext
  {
    Guid Id { get; }
    int UserId { get; }
    int ResourceId { get; }
    string TraceId { get; }
    string SpanContext { get; }
    string ConnectionId { get; }
    string ExecutionId { get; }
    string Name { get; }
    string Origin { get; }
    string Resource { get; }
    string Culture { get; }
    int Retries { get; set; }
    DateTime CreatedAt { get; }
  }
}