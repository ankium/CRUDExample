using ServiceContracts;
using Services;
using Microsoft.EntityFrameworkCore;
using Entities; 

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

// Use singleton lifetime so in-memory data stored in service instance persists across requests
builder.Services.AddSingleton<ICountriesService, CountriesService>();
builder.Services.AddSingleton<IPersonsService, PersonsService>();

// Add DbContext with SQL Server provider
builder.Services.AddDbContext<PersonsDbContext>(options =>
{
    options.UseSqlServer();
});

var app = builder.Build();

if (app.Environment.IsDevelopment()){
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();
