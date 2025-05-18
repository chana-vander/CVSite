using CVSite.API.CachServices;
using CVSite.Service;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Octokit;

var builder = WebApplication.CreateBuilder(args);

// Configure GitHub options from config (appsettings.json)
builder.Services.Configure<GitHubIntegrationOptions>(builder.Configuration.GetSection("GitHub"));

// Add memory cache service
builder.Services.AddMemoryCache();

// Register GitHubClient as scoped service
builder.Services.AddScoped<GitHubClient>(provider =>
{
    var options = provider.GetRequiredService<IOptions<GitHubIntegrationOptions>>().Value;
    var client = new GitHubClient(new ProductHeaderValue("CVSite"));
    client.Credentials = new Credentials(options.Token);
    return client;
});

// Register the real GitHubService
builder.Services.AddScoped<GitHubService>();

// Register IGitHubService as CacheGithubService, which wraps GitHubService
builder.Services.AddScoped<IGitHubService>(provider =>
    new CacheGithubService(
        provider.GetRequiredService<GitHubService>(),
        provider.GetRequiredService<IMemoryCache>(),
        provider.GetRequiredService<GitHubClient>()
    )
);

builder.Services.AddControllers();

//Swagger:
builder.Services.AddEndpointsApiExplorer(); // חובה ל-Swagger
builder.Services.AddSwaggerGen();

// OpenAPI helper
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
