using Microsoft.EntityFrameworkCore;
using SubastaAutos.Application.Profiles;
using SubastaAutos.Application.Services.Implementations;
using SubastaAutos.Application.Services.Interfaces;
using SubastaAutos.Infraestructure.Data;
using SubastaAutos.Infraestructure.Repository.Implementations;
using SubastaAutos.Infraestructure.Repository.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//***********
// =======================
// Configurar Dependency Injection
// =======================
//*** Repositories
builder.Services.AddTransient<IRepositoryRolUsuario, RepositoryRolUsuario>();
builder.Services.AddTransient<IRepositoryUsuario, RepositoryUsuario>();
//*** Services
builder.Services.AddTransient<IServiceRolUsuario, ServiceRolUsuario>();
builder.Services.AddTransient<IServiceUsuario, ServiceUsuario>();
// =======================
// Configurar AutoMapper
// =======================
builder.Services.AddAutoMapper(config =>
{
    //*** Profiles
    config.AddProfile<RolUsuarioProfile>();
    config.AddProfile<UsuarioProfile>();
});
// =======================
// Configurar SQL Server DbContext
// =======================
var connectionString = builder.Configuration.GetConnectionString("SqlServerDataBase");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
    "No se encontró la cadena de conexión 'SqlServerDataBase' en appsettings.json /appsettings.Development.json." );
}
builder.Services.AddDbContext<SubastaAutosContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        // Reintentos ante fallos transitorios (recomendado)
        sqlOptions.EnableRetryOnFailure();
    });
    if (builder.Environment.IsDevelopment())
        options.EnableSensitiveDataLogging();
});
//**********

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.UseStaticFiles();

app.Run();
