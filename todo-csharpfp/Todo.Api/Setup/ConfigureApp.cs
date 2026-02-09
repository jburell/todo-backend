using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;

namespace Todo.Api.Setup;

public static class ConfigureApp
{
  public static void Configure(WebApplication app)
  {
    ConfigureRequestLogging(app);
    app.UseHttpLogging();
    app.MapOpenApi();
    
    if (app.Environment.IsDevelopment())
    {
      app.MapScalarApiReference();
    }
    else
    {
      app.UseHttpsRedirection();
    }

    app.UseCors("AllowAll");
    app.UseAuthentication();
    app.UseAuthorization();
  }

  private static void ConfigureRequestLogging(IApplicationBuilder app)
  {
    app.Use(async (context, next) =>
    {
      context.Request.EnableBuffering();
      await next();
    });

    app.UseSerilogRequestLogging(options =>
    {
      options.MessageTemplate = "HTTP {RequestMethod} {RequestPath}{QueryString} Body: {RequestBody} responded {StatusCode} in {Elapsed:0.0000} ms";
      options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
      {
        var request = httpContext.Request;
        var query = request.QueryString.HasValue ? request.QueryString.Value : "";
        diagnosticContext.Set("QueryString", query);
        var bodyContent = "(empty)";

        switch (request.Method)
        {
          case "POST" or "PUT" or "PATCH"
            when request is { ContentLength: > 0, ContentType: not null }
              && (request.ContentType.Contains("application/json") || request.ContentType.Contains("text/plain")):
            try
            {
              request.Body.Position = 0;
              using var reader = new StreamReader(request.Body, leaveOpen: true);
              var content = reader.ReadToEnd(); // Synchronous read, fix?
              bodyContent = content.Length > 500 ? content[..500] + "..." : content;
              request.Body.Position = 0;
            }
            catch
            {
              bodyContent = "(read-error)";
            }

            break;
          case "OPTIONS" or "GET":
            bodyContent = "(none)";
            break;
        }
        diagnosticContext.Set("RequestBody", bodyContent);
      };
      
      options.GetLevel = (httpContext, _, ex) =>
      {
        if (ex != null || httpContext.Response.StatusCode >= 500)
        {
          return LogEventLevel.Error;
        }

        return httpContext.Request.Method == "OPTIONS" 
          ? LogEventLevel.Verbose 
          : LogEventLevel.Information;
      };
    });
  }
}