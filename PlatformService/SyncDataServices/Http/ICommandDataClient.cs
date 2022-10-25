using PlatformService.Dtos;

namespace PlatformServices.SyncDataService.Http;

public interface ICommandDataClient
{
    Task SendPlatformToCommand(PlatformReadDto plat);
}