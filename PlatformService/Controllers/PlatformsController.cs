using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Controllers;

[Route("api/platforms")]
[ApiController]
public class PlatformsController : ControllerBase
{
    private readonly IPlatformRepo _repo;
    private readonly IMapper _mapper;

    public PlatformsController(IPlatformRepo repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
    {
        var platformItems = _repo.GetAllPlatforms();

        return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
    }

    [HttpGet("{id}", Name = "GetPlatformById")]
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
    public ActionResult<PlatformReadDto> CreatePlatform(PlatformCreateDto platform)
    {
        var plat = _mapper.Map<Platform>(platform);

        _repo.CreatePlatform(plat);
        _repo.SaveChanges();
        
        var createdPlatform = _mapper.Map<PlatformReadDto>(plat);

        return CreatedAtRoute(nameof(GetPlatformById), new {Id = createdPlatform.Id}, createdPlatform);
    }
}