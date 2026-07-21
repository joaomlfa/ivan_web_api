using IvanWeb.Application.Interfaces;
using IvanWeb.Application.Rules;
using IvanWeb.Application.Rules.Trophies;
using IvanWeb.Application.Services;
using IvanWeb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<ITrophyEngineService, TrophyEngineService>();
builder.Services.AddScoped<ITrophyRule, ScoreTrophyRule>();
builder.Services.AddScoped<ITrophyRule, FirstTimeTrophyRule>();
builder.Services.AddScoped<ITrophyRule, QuizTrophyRule>();
builder.Services.AddScoped<ITrophyRule, ClockTrophyRule>();
builder.Services.AddScoped<ITrophyRule, LojaTrophyRule>();

builder.Services.AddMemoryCache();

// 1. Ler as origens permitidas da configuração
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

// 2. Adicionar o serviço de CORS
builder.Services.AddCors(options =>
{
    // Política de Produção (Trava nos domínios oficiais)
    options.AddPolicy("ProductionPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins!)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });

    // Política de Desenvolvimento (Libera a festa localmente)
    options.AddPolicy("DevPolicy", policy =>
    {
        policy.AllowAnyOrigin() // Aceita qualquer porta (35723, 5000, etc)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseCors("DevPolicy");
}
else
{
    app.UseCors("ProductionPolicy");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        await DbInitializer.SeedAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Um erro ocorreu ao popular o banco de dados inicial.");
    }
}

app.Run();
