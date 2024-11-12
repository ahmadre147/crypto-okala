using dotenv.net;
using MessagePack;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Okala.Crypto.Domain.Dtos;
using Okala.Crypto.Services;
using Okala.Crypto.Utils.Cache;

var builder = WebApplication.CreateBuilder(args);

DotEnv.Load();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

#region MessagePackOptions
MessagePackSerializer.DefaultOptions = MessagePack.Resolvers.ContractlessStandardResolver.Options;
#endregion

#region Configuration
builder.Services.Configure<AppSettings>(builder.Configuration);
var settings = builder.Configuration.Get<AppSettings>();
#endregion

#region Logging
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Logging.AddConsole();
#endregion

#region Register Services
builder.Services.AddServices();

var redis = ConnectionMultiplexer.Connect(settings?.Redis.Url);
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);
builder.Services.AddSingleton<ICacheProvider>(new RedisCacheProvider(redis, TimeSpan.FromSeconds(10)));
#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
