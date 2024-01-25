using System.Text.Json;
using System.Text.Json.Serialization;
using IYFApi.Repositories;
using IYFApi.Repositories.Interfaces;
using IYFApi.Services;
using IYFApi.Services.Interfaces;

namespace IYFApi;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        AddDependencyInjection(services);
        
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            });

        services.AddDbContext<ApplicationDbContext>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    private static void AddDependencyInjection(IServiceCollection services)
    {
        services.AddSingleton<IPostRepository, PostRepository>();
        services.AddSingleton<IEventRepository, EventRepository>();
        services.AddSingleton<IRegularEventRepository, RegularEventRepository>();
        services.AddSingleton<IVisitorRepository, VisitorRepository>();
        
        services.AddSingleton<IDonationService, DonationService>();
        services.AddSingleton<IAccessManagementService, AccessManagementService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        ExceptionHandling.IsDevelopment = env.IsDevelopment();
        
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = string.Empty;
            });
        }
        else
        {
            app.UseExceptionHandler(ExceptionHandling.ExceptionHandler);
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}