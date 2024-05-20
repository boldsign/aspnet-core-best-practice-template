using DemoApi;
using Microsoft.AspNetCore;

var webHostBuilder = WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();
var webApplication = webHostBuilder.Build();
webApplication.Run();
