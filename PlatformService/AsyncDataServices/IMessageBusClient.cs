using PlatformService.Dtos;

namespace PlatformService.AsynDataServices;

public interface IMessageBusClient
{
    void PublishNewPlatform(PlatformPublishedDto platform);
}