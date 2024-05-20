using DemoMicroService.Services;
using DemoSharedLib;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
services.AddGrpc();
services.AddAuth();

var app = builder.Build();

app.UseRouting();
app.UseAuth();

app.MapGrpcService<AuthService>();

app.Run();
