using System.Text;
using System.Text.Json;
using PlatformService.Dtos;

namespace PlatformServices.SyncDataService.Http;

public class HttpCommandDataClient : ICommandDataClient
{
    private readonly HttpClient _client;
    private readonly IConfiguration _config;

    public HttpCommandDataClient(HttpClient client, IConfiguration config)
    {
        _client = client;
        _config = config;
    }

    public async Task SendPlatformToCommand(PlatformReadDto plat)
    {
        var httpContent = new StringContent(
            JsonSerializer.Serialize(plat),
            Encoding.UTF8,
            "application/json");

        var response = await _client.PostAsync(_config["CommandService"], httpContent);

        if(response.IsSuccessStatusCode)
        {
            Console.WriteLine("Successfully called CommandsService through PlatformService");
        }
        else
        {
            Console.WriteLine("Calling CommandsService was not ok.");
        }
    }
}