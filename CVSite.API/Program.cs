using CVSite.Service;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<GitHubService>();
builder.Services.AddControllers();

//Swagger:
builder.Services.AddEndpointsApiExplorer(); // חובה ל-Swagger
builder.Services.AddSwaggerGen();

//cache:
builder.Services.AddMemoryCache();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.Configure<GitHubSettings>(builder.Configuration.GetSection("GitHub"));
builder.Services.AddScoped<GitHubService>();

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
