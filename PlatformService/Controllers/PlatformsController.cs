using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsynDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformServices.SyncDataService.Http;

namespace PlatformService.Controllers;

[Route("api/platforms")]
[ApiController]
public class PlatformsController : ControllerBase
{
    private readonly IPlatformRepo _repo;
    private readonly IMapper _mapper;
    private readonly ICommandDataClient _client;
    private readonly IMessageBusClient _messageBus;

    public PlatformsController(
        IPlatformRepo repo, 
        IMapper mapper,
        ICommandDataClient client,
        IMessageBusClient messageBus)
    {
        _repo = repo;
        _mapper = mapper;
        _client = client;
        _messageBus = messageBus;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
    {
        var platformItems = _repo.GetAllPlatforms();

        return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
    }

    [HttpGet("{id}", Name = nameof(GetPlatformById))]
    public ActionResult<PlatformReadDto> GetPlatformById(int id)
    {
        var platform = _repo.GetPlatformById(id);

        if(platform == null)
        {
            return NotFound(id);
        }

        return Ok(_mapper.Map<PlatformReadDto>(platform));
    }

    [HttpPost]
    public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platform)
    {
        var plat = _mapper.Map<Platform>(platform);

        _repo.CreatePlatform(plat);
        _repo.SaveChanges();
        
        var createdPlatform = _mapper.Map<PlatformReadDto>(plat);

        // Send sync message
        try
        {
            await _client.SendPlatformToCommand(createdPlatform);
        }
        catch(Exception e)
        {
            Console.WriteLine($"--> Could not send synchrously: {e.Message}");
        }

        // Send async message 
        try
        {
            var PlatformPublishedDto = _mapper.Map<PlatformPublishedDto>(createdPlatform);
            PlatformPublishedDto.Event = "Platform_Published";
            _messageBus.PublishNewPlatform(PlatformPublishedDto);
        }
        catch(Exception e)
        {
            Console.WriteLine($"--> Platform Controller Failed while sending a message: {e.Message}.");
        }

        return CreatedAtRoute(nameof(GetPlatformById), new {Id = createdPlatform.Id}, createdPlatform);
    }
}