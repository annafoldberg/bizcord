using System.Text.Json.Serialization;
using ChannelService.Infrastructure.Messaging;
using ChannelService.Infrastructure.Persistence;
using ChannelService.Application;
using ChannelService.Presentation.Middleware;

var builder = WebApplication.CreateBuilder(args);

await builder.Services.AddPersistenceAsync(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddRabbitMQMessaging(builder.Configuration);
builder.Services.AddHealthChecks();

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter()));

builder.Services.AddSwaggerGen();

var app = builder.Build();

await app.Services.ApplyMigrationsAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<IdentityLoggingMiddleware>();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();