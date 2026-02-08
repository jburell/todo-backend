using Serilog;
using Serilog.Events;
using Todo.Api.Setup;

Log.Logger = new LoggerConfiguration()
  .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning) // Hide standard ASP.NET noise
  .Filter.ByExcluding(log => log.MessageTemplate.Text.Contains("HTTP OPTIONS")) // Hide CORS checks
  .WriteTo.Console()
  .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

ConfigureServices.Configure(builder);

var app = builder.Build();

ConfigureApp.Configure(app);

ConfigureEndpoints.AddApiEndpoints(app);

app.Run();