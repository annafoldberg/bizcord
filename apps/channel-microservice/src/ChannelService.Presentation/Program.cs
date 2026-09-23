using System.Text.Json.Serialization;
using ChannelService.Infrastructure.Messaging;
using ChannelService.Infrastructure.Persistence;
using ChannelService.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddRabbitMQMessaging(builder.Configuration);

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
app.MapControllers();

app.Run();