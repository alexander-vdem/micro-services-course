using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsynDataServices;

public class MessageBusClient : IMessageBusClient
{
    private readonly IConfiguration _config;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    private readonly string exchangeName = "trigger";

    public MessageBusClient(IConfiguration config)
    {
        _config = config;

        var factory = new ConnectionFactory()
        {
            HostName = _config["RabbitMQHost"],
            Port = int.Parse(_config["RabbitMqPort"])
        };

        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout);
            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutDown;

            Console.WriteLine("--> Connected to message bus.");
        }
        catch(Exception e)
        {
            Console.Write($"--> Could not connect to the message bus: {e.Message}");
            throw;
        }
    }

    public void PublishNewPlatform(PlatformPublishedDto platform)
    {
        var message = JsonSerializer.Serialize(platform);

        if(_connection.IsOpen)
        {
            Console.WriteLine("--> RabbitMQ connection Open, sending message...");
            SendMessage(message);
        }
        else
        {
            Console.WriteLine("--> RabbitMQ connection Closeed, not sending.");
        }
    }

    private void SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(exchangeName, routingKey: "", basicProperties: null, body: body);

        Console.WriteLine($"--> Message sent {message}");
    }

    public void Dispose()
    {
        Console.WriteLine("MessageBus Disposed");

        if(_channel.IsOpen)
        {
            _channel.Close();
            _connection.Close();
        }
    }

    private void RabbitMQ_ConnectionShutDown(object? sender, ShutdownEventArgs e)
    {
        Console.WriteLine("--> RabbitMQ Connection Shutdown.");
    }
}