using ArtworkProvider.Backend.Models;
using ArtworkProvider.Backend.Services;
using Microsoft.OpenApi.Models;
using VueCliMiddleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDB"));
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<CompanyService>();
builder.Services.AddSingleton<CampaignService>();
builder.Services.AddSingleton<SprintService>();
builder.Services.AddSingleton<TaskService>();



// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("access_token", new OpenApiSecurityScheme()
    {
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Name = "API_KEY",
        Description = "API Key Authentication",
    });
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
                      policy =>
                      {
                          policy.WithOrigins("*")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                      });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.RoutePrefix = string.Empty;
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API version1");
    });
}

// app.UseHttpsRedirection();
// app.UseSpaStaticFiles();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
