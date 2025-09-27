using LeOne.API.SignalR;
using LeOne.Application;
using LeOne.Infrastructure.Auth;
using LeOne.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("Local"))
{
    builder.Configuration.AddUserSecrets<Program>();
}

var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddSignalR()
    .AddJsonProtocol(options =>
    {
        options.PayloadSerializerOptions.Converters.Add(new DomainEventNotificationJsonConverter());
    });


builder.Services
    .AddApplicationServices()
    .AddDataServices(configuration)
    .AddAuthServices(configuration);

builder.Services.AddHostedService<DomainEventSignalRBridge>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
if (!app.Environment.IsEnvironment("Local"))
{
    app.UseHttpsRedirection();
}
app.UseAuthorization();
app.MapControllers();
app.MapHub<DomainEventsHub>("/hubs/domain-events");
app.Run();
