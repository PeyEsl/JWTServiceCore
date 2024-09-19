using Microsoft.EntityFrameworkCore;
using JWTServiceCore.Configurations;
using JWTServiceCore.Configurations.Extensions;
using JWTServiceCore.Data;

var builder = WebApplication.CreateBuilder(args);

// Load configuration from appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configure strongly typed settings objects
var appSettingSection = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<AppSetting>(appSettingSection);

// Configure JWT authentication
var appSettings = appSettingSection.Get<AppSetting>();

// Set up authentication
builder.Services.AddApplicationAuthentication(appSettings!);

// Configure Identity services
builder.Services.AddApplicationIdentity();

// Add application services
builder.Services.AddApplicationScoped();

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllersWithViews();

// Swagger service properties
builder.Services.AddApplicationSwagger();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "JWT API V1");
    c.RoutePrefix = "swagger-ui";
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
