using Inventory.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Application Services
        services.AddScoped<OrderService>();
        services.AddScoped<VendorService>();
        services.AddScoped<InventoryService>();

        // MediatR for CQRS (if needed in future)
        // services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        return services;
    }
}
