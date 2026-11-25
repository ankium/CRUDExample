using Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.Options;

namespace CRUDTest;

public class CustomWebApplicationFactory:WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.UseEnvironment("Test");
        builder.ConfigureServices(services =>
        {
            ServiceDescriptor? descriptor = services.SingleOrDefault(temp => temp.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            // Remove SQL Server DbContext
            if (descriptor!=null)
            {
                services.Remove(descriptor);
            }
            // Use EFCore In-Memory
            services.AddDbContext<ApplicationDbContext>(Options =>
            {
                Options.UseInMemoryDatabase("DatabaseForTesting");
            });
        });
    }
}
