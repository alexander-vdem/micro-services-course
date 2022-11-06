using CommandsService.Models;

namespace CommandsService.Data;

public class CommandRepo : ICommandRepo
{
    private readonly AppDbContext _context;

    public CommandRepo(AppDbContext context)
    {
        _context = context;
    }

    public void CreateCommand(int platformId, Command command)
    {
        if(command == null)
        {
            throw new ArgumentNullException();
        }

        command.PlatformId = platformId;
        _context.Commands.Add(command);
    }

    public void CreatePlatform(Platform plat)
    {
        if(plat == null)
        {
            throw new ArgumentNullException();
        }

        _context.Platforms.Add(new Platform
        {
            Name = plat.Name,
            Commands = plat.Commands,
            ExternalID = plat.ExternalID
        });
    }

    public bool ExternalPlatformExists(int externalId)
    {
        return _context.Platforms.Any(p=> p.ExternalID == externalId);        
    }

    public IEnumerable<Platform> GetAllPlatforms()
    {
        return _context.Platforms.ToList();
    }

    public Command GetCommand(int platformId, int commandId)
    {
        var command = (from c in _context.Commands
            where c.Id == commandId && c.PlatformId == platformId
            select c).FirstOrDefault();
        
        return command;    
    }

    public IEnumerable<Command> GetCommandsForPlatform(int platformId)
    {
        return(
            from c in _context.Commands
            where c.PlatformId == platformId
            orderby c.Platform.Name
            select new Command()).AsEnumerable<Command>();
    }

    public bool PlatformExists(int platformId)
    {
        return _context.Platforms.Any(p=> p.Id == platformId);
    }

    public bool SaveChanges()
    {
        return _context.SaveChanges() >= 0;
    }
}