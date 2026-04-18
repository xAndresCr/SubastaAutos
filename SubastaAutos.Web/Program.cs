using Microsoft.EntityFrameworkCore;
using SubastaAutos.Application.Profiles;
using SubastaAutos.Application.Services.Implementations;
using SubastaAutos.Application.Services.Interfaces;
using SubastaAutos.Infraestructure.Data;
using SubastaAutos.Infraestructure.Repository.Implementations;
using SubastaAutos.Infraestructure.Repository.Interfaces;
using SubastaAutos.Web.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Cache en memoria (requerido por Session)
builder.Services.AddDistributedMemoryCache();

// ← Solo UNA vez AddSession
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".SubastaAutos.Session";
});

builder.Services.AddControllersWithViews();
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});

// Repositories
builder.Services.AddTransient<IRepositoryRolUsuario, RepositoryRolUsuario>();
builder.Services.AddTransient<IRepositoryUsuario, RepositoryUsuario>();
builder.Services.AddTransient<IRepositoryAuto, RepositoryAuto>();
builder.Services.AddTransient<IRepositorySubasta, RepositorySubasta>();
builder.Services.AddTransient<IRepositoryPuja, RepositoryPuja>();
builder.Services.AddTransient<IRepositoryCategoria, RepositoryCategoria>();
builder.Services.AddTransient<IRepositoryCondicionAuto, RepositoryCondicionAuto>();
builder.Services.AddTransient<IRepositoryEstadoAuto, RepositoryEstadoAuto>();

// Services
builder.Services.AddTransient<IServiceRolUsuario, ServiceRolUsuario>();
builder.Services.AddTransient<IServiceUsuario, ServiceUsuario>();
builder.Services.AddTransient<IServiceAuto, ServiceAuto>();
builder.Services.AddTransient<IServiceSubasta, ServiceSubasta>();
builder.Services.AddTransient<IServicePuja, ServicePuja>();
builder.Services.AddTransient<IServiceCategoria, ServiceCategoria>();
builder.Services.AddTransient<IServiceCondicionAuto, ServiceCondicionAuto>();
builder.Services.AddTransient<IServiceEstadoAuto, ServiceEstadoAuto>();

// AutoMapper
builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<RolUsuarioProfile>();
    config.AddProfile<UsuarioProfile>();
    config.AddProfile<CategoriaProfile>();
    config.AddProfile<CondicionAutoProfile>();
    config.AddProfile<EstadoAutoProfile>();
    config.AddProfile<AutoImagenProfile>();
    config.AddProfile<AutoProfile>();
    config.AddProfile<EstadoSubastaProfile>();
    config.AddProfile<PujaProfile>();
    config.AddProfile<SubastaProfile>();
});

// SQL Server
var connectionString = builder.Configuration.GetConnectionString("SqlServerDataBase");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "No se encontró la cadena de conexión 'SqlServerDataBase'.");
}
builder.Services.AddDbContext<SubastaAutosContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure();
    });
    if (builder.Environment.IsDevelopment())
        options.EnableSensitiveDataLogging();
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// ← Orden correcto
app.UseSession();       // ← antes de UseRouting


app.UseWebSockets();
app.UseRouting();
app.UseAuthorization();

app.MapHub<SubastaHub>("/subastaHub");
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();