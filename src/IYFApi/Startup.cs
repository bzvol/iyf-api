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
    private const string FirebaseProjectId = "iyfhu-caaf9";

    private static readonly string[] AllowedOrigins =
        ["http://localhost:3000", "https://iyf.hu", "https://admin.iyf.hu"];

    public void ConfigureServices(IServiceCollection services)
    {
        AddDependencyInjection(services);

        services.Configure<RouteOptions>(options =>
        {
            options.LowercaseUrls = true;
            options.LowercaseQueryStrings = true;
        });

        services.AddCors(options =>
            options.AddDefaultPolicy(policy => policy.WithOrigins(AllowedOrigins).AllowAnyHeader().AllowAnyMethod()));

        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            });

        services.AddDbContext<ApplicationDbContext>(ServiceLifetime.Transient);

        FirebaseApp.Create(new AppOptions
        {
            Credential = LoadFirebaseCredentials(),
            ProjectId = FirebaseProjectId
        });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    private static void AddDependencyInjection(IServiceCollection services)
    {
        services.AddSingleton<IPostRepository, PostRepository>();
        services.AddSingleton<IEventRepository, EventRepository>();
        services.AddSingleton<IRegularEventRepository, RegularEventRepository>();
        services.AddSingleton<IGuestRepository, GuestRepository>();

        services.AddSingleton<IInfoService, InfoService>();
        services.AddSingleton<IDonationService, DonationService>();
        services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<IMailService, MailService>();
    }

    private static GoogleCredential LoadFirebaseCredentials()
    {
        var firebaseCredentialsFile = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS_FILE");
        if (firebaseCredentialsFile != null)
            return GoogleCredential.FromFile(firebaseCredentialsFile);

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

        app.UseCors();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}