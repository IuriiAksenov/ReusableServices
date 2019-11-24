using System;
using Newtonsoft.Json;
using ReusableServices.Common.Extensions;

namespace ReusableServices.Common.RabbitMq
{
  public class CorrelationContext : ICorrelationContext
  {
    public static ICorrelationContext Empty => new CorrelationContext();
    public Guid Id { get; }
    public int UserId { get; }
    public int ResourceId { get; }
    public string TraceId { get; }
    public string SpanContext { get; }
    public string ConnectionId { get; }
    public string ExecutionId { get; }
    public string Name { get; }
    public string Origin { get; }
    public string Resource { get; }
    public string Culture { get; }
    public int Retries { get; set; }
    public DateTime CreatedAt { get; }

    public CorrelationContext()
    {
    }

    private CorrelationContext(Guid id)
    {
      Id = id;
    }

    private CorrelationContext(Guid id, int userId)
    {
      Id = id;
      UserId = userId;
    }

    private CorrelationContext(Guid id, int userId, string culture)
    {
      Id = id;
      UserId = userId;

      Culture = culture;
    }

    [JsonConstructor]
    private CorrelationContext(Guid id, int userId, int resourceId, string traceId, string spanContext, string connectionId,
      string executionId, string name, string origin, string resource, string culture, int retries)
    {
      Id = id;
      UserId = userId;

      ResourceId = resourceId;
      TraceId = traceId;
      SpanContext = spanContext;
      ConnectionId = connectionId;
      ExecutionId = executionId;
      Name = string.IsNullOrWhiteSpace(name) ? string.Empty : GetName(name);
      Origin = string.IsNullOrWhiteSpace(origin) ? string.Empty : origin.StartsWith("/") ? origin.Remove(0, 1) : origin;
      Resource = resource;
      Culture = culture;
      CreatedAt = DateTime.UtcNow;
    }

    public static ICorrelationContext FromId(Guid id)
    {
      return new CorrelationContext(id);
    }

    private static string GetName(string name)
    {
      return name.Underscore().ToLowerInvariant();
    }
  }
}