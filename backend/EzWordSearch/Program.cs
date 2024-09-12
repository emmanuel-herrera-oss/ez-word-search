using EzWordSearch.Auth;
using EzWordSearch.Domain.Matches;
using EzWordSearch.Domain.MatchManagement;
using EzWordSearch.Matches;
using EzWordSearch.Persistence.Contract;
using EzWordSearch.Persistence.EF;
using EzWordSearch.Profile;
using EzWordSearch.Service;
using EzWordSearch.Service.Contract;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using EzWordSearch.Leaderboard;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Ready settings from appsettings & environment
builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection(AuthOptions.Name));
builder.Services.Configure<MatchOptions>(builder.Configuration.GetSection(MatchOptions.Name));

// Postgres DB
builder.Services.AddDbContext<EzDbContext>(c => c.UseNpgsql(builder.Configuration.GetConnectionString("EzWordSearch")));

// Services
builder.Services.AddHttpLogging(o => o.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All);
builder.Services.AddSingleton<IUserIdProvider, SignalRUserIdProvider>(); // SignalR User Id will be PlayerId from claims
builder.Services.AddSignalR();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

// Custom Services
builder.Services.AddScoped<IIdentityService, DefaultIdentityService>();
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IMatchRepository, MatchRepository>();
builder.Services.AddSingleton<MatchEventHandlers>();
builder.Services.AddSingleton<IMatchFactory, DefaultMatchFactory>();
builder.Services.AddSingleton<MatchManager>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateAudience = false,
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration.GetValue<string>("Auth:TokenIssuer"),
        ValidateLifetime = true,
        NameClaimType = "sub"
    };
    o.Authority = builder.Configuration.GetValue<string>("Auth:TokenAuthority");
    // Get token from cookie
    o.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (!string.IsNullOrEmpty(context.Request.Cookies["WS_ACCESS_TOKEN"]))
            {
                context.Token = context.Request.Cookies["WS_ACCESS_TOKEN"];
            }
            return Task.CompletedTask;
        }
    };
    o.RequireHttpsMetadata = builder.Configuration.GetValue<bool>("Auth:RequireHttpsMetadata");
    o.MapInboundClaims = false;
});

var allowedOrigins = builder.Configuration.GetValue<string>("AllowedOrigins")?.Split(';').ToArray() ?? throw new Exception("Missing AllowedOrigins");
builder.Services.AddCors(o => 
    o.AddPolicy(name: "DefaultEz", 
    p => p.WithOrigins(allowedOrigins)
    .AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

var app = builder.Build();

app.UseCors("DefaultEz");
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/api/v1/auth/login", AuthEndpoint.Login).AllowAnonymous();
app.MapGet("/api/v1/profile", ProfileEndpoint.GetProfile).RequireAuthorization();
app.MapPost("/api/v1/match", MatchEndpoint.FindMatch).RequireAuthorization();
app.MapGet("/api/v1/match/count", MatchEndpoint.GetPlayerCount).AllowAnonymous();
app.MapGet("/api/v1/match/history", MatchEndpoint.GetHistory).RequireAuthorization();
app.MapPost("/api/v1/auth/logout", AuthEndpoint.Logout).AllowAnonymous();
app.MapGet("/api/v1/leaderboard", LeaderboardEndpoint.GetLeaderboard).RequireAuthorization();

app.MapHub<MatchHub>("/server");

app.Run();

