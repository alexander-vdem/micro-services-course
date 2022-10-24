using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[Route("api/c/[controller]")]
[Controller]
public class PlatformsController : ControllerBase
{
    private readonly IMapper _mapper;

    public PlatformsController(IMapper mapper)
    {
        _mapper = mapper;
    }

    [HttpPost]
    public ActionResult TestInboundConnection()
    {
        Console.WriteLine("Hello from Commands Service, PlatformsController");

        return Ok("Inbound test of Platforms Controller");
    }
}