using AutoParts.Business;
using AutoParts.Business.ServiceRegistrations;
using AutoParts.DataAccess.Contexts;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables("APP_");

#region Service Registration

AppConfig.Configuration = builder.Configuration;
builder.Services.AddServices(builder.Configuration, builder.Environment);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

#endregion

builder.Services.AddHttpClient();

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILogger<Program>>();

#region Auto Migrate
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AutoPartsStoreDb>();

    var appliedMigrations = dbContext.Database.GetAppliedMigrations();
    var pendingMigrations = dbContext.Database.GetPendingMigrations();
    var missingMigrations = pendingMigrations.Except(appliedMigrations).ToList();

    if (missingMigrations.Any())
    {
        logger.LogInformation("There are pending migrations:");
        foreach (var migration in missingMigrations)
        {
            logger.LogInformation("{Migration}", migration);
        }
    }
    else
    {
        logger.LogInformation("Migrations have been successfully applied.");
    }

    try
    {
        dbContext.Database.Migrate();

        logger.LogInformation("Database was successfully migrated.");
    }
    catch (Exception ex)
    {
        logger.LogError("An error occurred while migrating the database. {Message}", ex.Message);
        logger.LogError("{StackTrace}", ex.StackTrace);
    }
}
#endregion

// Enable/Disable Swagger
if (Convert.ToBoolean(builder.Configuration.GetSection("IsSwaggerEnabled").Value!))
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStatusCodePages(async context =>
{
    if (context.HttpContext.Response.StatusCode is 404 or 405)
    {
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(new
        {
            Result = false,
            Message = $"There is no route for this request method : {context.HttpContext.Request.Method}"
        }));
    }
});

app.UseHttpsRedirection();

app.UseCors(builder.Configuration.GetSection("CorsLabel").Value!);
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.UseStaticFiles();

app.Run();