using FincaFenix.EFCore;
using FincaFenix.EFCore.Context;
using FincaFenix.WebApi.Configurations;
using FincaFenixControllers.Middleware;
using QuestPDF.Infrastructure;
using System.Globalization;

var defaultCulture = "es-AR";
var culture = new CultureInfo(defaultCulture);
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);
builder.Services.AddIdentityDataBase(builder.Configuration);
builder.Services.AddServicesContainer(builder.Configuration);
builder.Services.AddSwaggerConfiguration();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Development", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FincaFenixContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    await SeedDataBase.SeedAddPoliciesAsync(context, logger);
    await SeedDataBase.SeedInventoryUnitsAsync(context, logger);
    await SeedDataBase.SeedTaskRendimientoModesAsync(context, logger);
}

if (app.Environment.IsDevelopment())
{
    app.UseCors("Development");
}

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FincaFenix API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
