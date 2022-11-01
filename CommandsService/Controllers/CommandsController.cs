using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[Route("api/c/platforms/{platformId}/[controller]")]
[ApiController]
public class CommandsController : ControllerBase
{
    private readonly ICommandRepo _repo;
    private readonly IMapper _mapper;

    public CommandsController(ICommandRepo repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    [HttpGet]
    public ActionResult<IEnumerable<CommandReadDto>> GetPlatformCommands(int platformId)
    {
        Console.WriteLine($"-->Hitting {nameof(GetPlatformCommands)} with {platformId}");

        if(!_repo.PlatformExists(platformId))
        {
            return NotFound($"Platform Id {platformId} not found.");
        }

        var commands = _repo.GetCommandsForPlatform(platformId);

        return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
    }

    [HttpGet("{comandId}", Name = nameof(GetCommand))]
    public ActionResult<CommandReadDto> GetCommand(int platformId, int commandId)
    {
        Console.WriteLine($"--> Hitting {nameof(GetCommand)} for commandId {commandId} and plaformId {platformId}");

        if(!_repo.PlatformExists(platformId))
        {
            return NotFound($"Platform Id {platformId} not found.");
        }

        var command = _repo.GetCommand(platformId, commandId);

        if(command == null)
        {
            return NotFound($"Could not find command id {commandId} for platform id {platformId} not found.");
        }

        return Ok(_mapper.Map<CommandReadDto>(command));
    }

    [HttpPost]
    public ActionResult<CommandReadDto> CreateCommand(CommandCreateDto cmd, int platformId)
    {
        Console.WriteLine($"--> Hitting {nameof(CreateCommand)} for plaformId {platformId}.");

        if(!_repo.PlatformExists(platformId))
        {
            return NotFound($"Platform Id {platformId} not found.");
        }

        var command = _mapper.Map<Command>(cmd);

        _repo.CreateCommand(platformId, command);
        _repo.SaveChanges();

        var createCommandDto = _mapper.Map<CommandReadDto>(command);

        return CreatedAtRoute(nameof(GetCommand), new {platformId = platformId, commandId = command.Id}, createCommandDto);
    }

}