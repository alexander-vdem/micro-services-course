using CommandsService.Models;
using CommandsService.SyncDataServices.Grpc;

namespace CommandsService.Data;

public static class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder builder)
    {
        using var serviceScope = builder.ApplicationServices.CreateScope();

        var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();

        var platforms = grpcClient.ReturnAllPlatforms();

        var repo = serviceScope.ServiceProvider.GetRequiredService<ICommandRepo>();

        SeedData(repo, platforms);
    }

    private static void SeedData(ICommandRepo repo, IEnumerable<Platform> platforms)
    {
        Console.WriteLine("Seeding new platforms ...");

        foreach(var plat in platforms)
        {
            if(!repo.ExternalPlatformExists(plat.ExternalID))
            {
                repo.CreatePlatform(plat);
            }

            repo.SaveChanges();
        }
    }
}