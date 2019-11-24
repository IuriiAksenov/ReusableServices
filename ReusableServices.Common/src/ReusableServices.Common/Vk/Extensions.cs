using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReusableServices.Common.Extensions;
using VkNet;
using VkNet.Model;

namespace ReusableServices.Common.Vk
{
  public static class Extensions
  {
    private const string SectionName = "VkApi";

    public static IServiceCollection AddVkApi(this IServiceCollection services)
    {
      if (services is null)
        throw new ArgumentNullException($"{nameof(VkApiOptions)}: {nameof(services)} is null.");

      using var serviceProvider = services.BuildServiceProvider();
      var configuration = serviceProvider.GetService<IConfiguration>();
      var options = configuration.GetOptions<VkApiOptions>(SectionName);
      services.AddSingleton(options);

      var api = new VkApi(services);
      api.Authorize(new ApiAuthParams { AccessToken = options.AccessToken });
      services.AddSingleton(api);

      var filePath = Path.Combine(AppContext.BaseDirectory, options.VkMessageIdFile);
      services.AddSingleton(new VkMessageId(filePath, options.VkMessageIdQueueCapacity));

      return services;
    }
  }

  public class VkMessageId
  {
    private readonly string _filePath;
    private readonly Queue<int> _messageIds = new Queue<int>();
    private readonly int _queueCapacity;

    public VkMessageId(string vkMessageIdFilePath, int queueCapacity)
    {
      _filePath = vkMessageIdFilePath;
      _queueCapacity = queueCapacity;
      GenerateNewIdsAndSave();
    }

    public int GetNewId()
    {
      if (_messageIds.TryDequeue(out var newId))
        return newId;
      GenerateNewIdsAndSave();
      return _messageIds.Dequeue();
    }

    /// <summary>
    ///   Следует проверять права на чтение файла VkMessageId, особенно в Linux.
    /// </summary>
    private void GenerateNewIdsAndSave()
    {
      string lastMessageIdStr;
      using (var reader = new StreamReader(_filePath))
      {
        lastMessageIdStr = reader.ReadLine();
      }

      var lastMessageId = 0;
      if (!string.IsNullOrEmpty(lastMessageIdStr) && int.TryParse(lastMessageIdStr, out var lastId))
        lastMessageId = lastId;

      for (var i = 0; i < _queueCapacity; i++)
        _messageIds.Enqueue(++lastMessageId);

      using var writer = new StreamWriter(_filePath, false);
      writer.WriteLine(lastMessageId);
    }
  }
}