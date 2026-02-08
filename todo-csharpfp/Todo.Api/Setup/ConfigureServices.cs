using Microsoft.AspNetCore.HttpLogging;
using Serilog;

namespace Todo.Api.Setup;

using Database;
using Stores;
using Pipelines;

public static class ConfigureServices
{
  
  public static void Configure(WebApplicationBuilder builder)
  {
    builder.Host.UseSerilog((context, services, configuration) => configuration
      .ReadFrom.Configuration(context.Configuration)
      .ReadFrom.Services(services)
      .Enrich.FromLogContext()
      .WriteTo.Console());
    
    if (builder.Environment.IsDevelopment())
    {
      builder.Services.AddHttpLogging(logging =>
        logging.LoggingFields =
          HttpLoggingFields.RequestBody
          | HttpLoggingFields.RequestQuery
          | HttpLoggingFields.RequestPath
          | HttpLoggingFields.ResponseBody
          | HttpLoggingFields.ResponseStatusCode);
    }
    else
    {
      builder.Services.AddHttpLogging(logging =>
        logging.LoggingFields =
          HttpLoggingFields.RequestQuery
          | HttpLoggingFields.RequestPath
          | HttpLoggingFields.ResponseStatusCode);
    }
    
    builder.Services.AddOpenApi(options =>
    {
      options.AddSchemaTransformer((schema, context, cancellationToken) =>
      {
        schema.Title = context.JsonTypeInfo.Type.FullName;
        return Task.CompletedTask;
      });
    });

    builder.Services.AddAuthentication();
    builder.Services.AddAuthorization();

    builder.Services.AddCors(options =>
    {
      // Allow all is only for testing purposes
      options.AddPolicy("AllowAll", policy =>
      {
        policy.AllowAnyOrigin()
          .AllowAnyHeader()
          .AllowAnyMethod();
      });
    });
    
    RegisterServices(builder.Services);
  } 
  
  private static void RegisterServices(IServiceCollection services)
  {
    services.AddSingleton<ITodoStore, TodoDatabase>();
    services.AddSingleton<TodoPipelines>();
  }
}