using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMapper _mapper;

    public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
    {
        _scopeFactory = scopeFactory;
        _mapper = mapper;
    }

    public void ProcessEvent(string message)
    {
        var eventType = DetermineEvent(message);

        switch (eventType)
        {
            case EventType.PlatformPublished:
                AddPlatform(message);
                break;

            default:
                break;
        }
    }

    private EventType DetermineEvent(string notifircationMessage)
    {
        if(string.IsNullOrEmpty(notifircationMessage))
            return EventType.Undetermined;

        Console.WriteLine("--> Determining event.");

        var eventType = JsonSerializer
                        .Deserialize<GenericEventDto>(notifircationMessage).Event;

        switch(eventType)
        {
            case "Platform_Published":
                Console.WriteLine("--> Message event detected.");
                return EventType.PlatformPublished;
            
            default:
            Console.WriteLine("--> Could not determine event type.");
                return EventType.Undetermined;
        }
    }

    private void AddPlatform(string platformPublishedMessage)
    {
        using var scope = _scopeFactory.CreateScope();

        var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

        var platfomPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

        try
        {
            var plat = _mapper.Map<Platform>(platfomPublishedDto);

            if(! repo.ExternalPlatformExists(plat.ExternalID))
            {
                repo.CreatePlatform(plat);
                repo.SaveChanges();   
            }

            Console.WriteLine($"--> Platform {plat.Name} added to the DB with external ID {plat.ExternalID} and internal ID {plat.Id}");
        }
        catch(Exception e)
        {
            Console.WriteLine($"Could not add platform to the DB. {e.Message}");
        }
    }
}

enum EventType
{
    PlatformPublished,
    Undetermined
}