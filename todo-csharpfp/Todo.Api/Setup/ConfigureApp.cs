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
      // usage of {QueryString} and {RequestBody} requires them to ALWAYS be set
      options.MessageTemplate =
        "HTTP {RequestMethod} {RequestPath}{QueryString} Body: {RequestBody} responded {StatusCode} in {Elapsed:0.0000} ms";

      options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
      {
        var request = httpContext.Request;

        // --- FIX 1: Always set QueryString ---
        // If no query, set it to empty string "" so the template doesn't break
        var query = request.QueryString.HasValue ? request.QueryString.Value : "";
        diagnosticContext.Set("QueryString", query);

        // --- FIX 2: Always set RequestBody ---
        var bodyContent = "(empty)"; // Default value

        // Only try to read if it's a POST/PUT and has JSON content
        if (request.Method is "POST" or "PUT" or "PATCH" 
            && request.ContentLength > 0 
            && request.ContentType != null 
            && (request.ContentType.Contains("application/json") || request.ContentType.Contains("text/plain")))
        {
          try
          {
            request.Body.Position = 0;
            // Use a StreamReader synchronously since the body is already buffered in memory
            using var reader = new StreamReader(request.Body, leaveOpen: true);
            var content = reader.ReadToEnd(); // Synchronous read
            
            bodyContent = content.Length > 500 ? content.Substring(0, 500) + "..." : content;
            
            request.Body.Position = 0;
          }
          catch
          {
            bodyContent = "(read-error)";
          }
        }
        else if (request.Method is "OPTIONS" or "GET")
        {
          bodyContent = "(none)";
        }

        // CRITICAL: This .Set() must happen every time!
        diagnosticContext.Set("RequestBody", bodyContent);
      };
      
      options.GetLevel = (httpContext, elapsed, ex) =>
      {
        // 1. If an actual error occurred (Exception or 500+), always log as ERROR
        if (ex != null || httpContext.Response.StatusCode >= 500)
        {
          return LogEventLevel.Error;
        }

        // 2. If it is an OPTIONS request, downgrade it to VERBOSE
        // (This hides it from the default Console view)
        if (httpContext.Request.Method == "OPTIONS")
        {
          return LogEventLevel.Verbose;
        }

        // 3. For everything else (GET, POST, etc.), keep it as INFORMATION
        return LogEventLevel.Information;
      };
    });
  }
}