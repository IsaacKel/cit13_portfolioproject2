using DataLayer;
using DataLayer.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Mapster;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMapster();

// Add services to the container.
// Register MovieDatabase with the dependency injection container
builder.Services.AddDbContext<MovieDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("MovieDatabase")));

// Register IDataService and the DataService implementation
builder.Services.AddSingleton<IDataService, DataService>();

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
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();