using DataLayer;
using DataLayer.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mapster;
using WebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddMapster();

// Add services to the container.
// Register MovieDatabase with the dependency injection container
var connectionString = builder.Configuration.GetConnectionString("imdbDatabase");
if (string.IsNullOrEmpty(connectionString))
{
  throw new InvalidOperationException("The ConnectionString property has not been initialized.");
}
else
{
  Console.WriteLine($"Connection String: {connectionString}");
}

builder.Services.AddDbContext<MovieDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register IDataService and the DataService implementation
builder.Services.AddSingleton<IDataService, DataService>();
builder.Services.AddSingleton(new Hashing());

var secret = "asdajodaoijfiw�afji�p�l���l��l�lDSADSADWADWADWADJKfhaiufwhuihfuwihuiapwkaepoaeopwaeopwapek212";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
        ClockSkew = TimeSpan.Zero
    }
    );






builder.Services.AddScoped<IDataService, DataService>();

// Mapster configuration
builder.Services.AddMapster();

builder.Services.AddControllers();

// Swagger configuration for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage(); // Enable detailed error pages
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();