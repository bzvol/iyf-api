using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using IYFApi.Exceptions;
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

        services.Configure<RouteOptions>(options =>
        {
            options.LowercaseUrls = true;
            options.LowercaseQueryStrings = true;
        });

        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            });

        services.AddDbContext<ApplicationDbContext>();

        FirebaseApp.Create(new AppOptions
        {
            Credential = LoadFirebaseCredentials(),
            ProjectId = "iyfhu-caaf9"
        });

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

    private static GoogleCredential LoadFirebaseCredentials()
    {
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            return GoogleCredential.FromFile("~/iyfhu-firebase-adminsdk.json");
        
        var config = new AmazonSecretsManagerConfig
            { RegionEndpoint = RegionEndpoint.GetBySystemName("eu-central-1") };
        var client = new AmazonSecretsManagerClient(config);

        var request = new GetSecretValueRequest
            { SecretId = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS_SECRET_NAME") };
        var response = client.GetSecretValueAsync(request).Result;
        var secret = response.SecretString;
        
        if (string.IsNullOrEmpty(secret)) throw new ConfigurationException("Firebase credentials not found");

        return GoogleCredential.FromJson(secret);
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