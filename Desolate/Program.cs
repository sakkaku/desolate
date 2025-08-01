using Desolate.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddConsole();

CoreSetup.Register(builder);

using var app = builder.Build();

await app.StartAsync();
await CoreSetup.Run(app);
await app.StopAsync();