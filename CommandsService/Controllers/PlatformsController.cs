using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[Route("api/c/[controller]")]
[Controller]
public class PlatformsController : ControllerBase
{
    private readonly ICommandRepo _repo;
    private readonly IMapper _mapper;

    public PlatformsController(ICommandRepo repo,  IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetAllPlatforms()
    {
        Console.WriteLine("--> Getting Platforms from CommandsService");
        
        var result = _mapper.Map<IEnumerable<PlatformReadDto>>(_repo.GetAllPlatforms());
        return Ok(result);
    }

    [HttpPost]
    public ActionResult TestInboundConnection()
    {
        Console.WriteLine("Hello from Commands Service, PlatformsController");

        return Ok("Inbound test of Platforms Controller");
    }
}