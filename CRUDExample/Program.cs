using ServiceContracts;
using Services;
using Microsoft.EntityFrameworkCore;
using Entities; 
using RepositoryContracts;
using Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

// Use dependency injection to register services and repositories
builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IPersonsService, PersonsService>();
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();

// Add DbContext with SQL Server provider
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=PersonsDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;Command Timeout=30
var app = builder.Build();

if (app.Environment.IsDevelopment()){
    app.UseDeveloperExceptionPage();
}

Rotativa.AspNetCore.RotativaConfiguration.Setup(builder.Environment.WebRootPath, "Rotativa");
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();
