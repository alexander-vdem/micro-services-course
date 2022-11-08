using Microsoft.EntityFrameworkCore;
using PlatformService.AsynDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Grpc;
using PlatformServices.SyncDataService.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

if(builder.Environment.IsDevelopment())
{
    Console.WriteLine("-->Using InMem Db.");
    builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));
}
else
{
    Console.WriteLine("-->Using Sql Server Db.");
    builder.Services.AddDbContext<AppDbContext>(opt => 
        opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn")));
}

builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();

builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();

builder.Services.AddGrpc();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSwaggerGen();

Console.WriteLine($"--> CommandService Endpoint {builder.Configuration["CommandService"]}");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<GrpcPlatformService>();

app.MapGet("/protos/platfprms.proto", async context =>
{
    await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
});

PrepDb.PrepPopulation(app, app.Environment.IsProduction());

app.Run();