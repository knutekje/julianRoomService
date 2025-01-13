using System.Text;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using RoomService;
using RoomService.Data;
using RoomService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RoomService.Middleware;


var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ApiGatewayPolicy", builder =>
    {
        builder.WithOrigins("http://nginx", "https://nginx")    
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddDbContext<RoomDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<AuthServiceClient>();

builder.Services.AddHttpClient<AuthServiceClient>(client =>
{
    client.BaseAddress = new Uri("https://auth-service:8443");
}); 

builder.Services.AddScoped<IMessagePublisher, MassTransitPublisher>();



builder.WebHost.ConfigureKestrel((context, options) =>
{
    var env = context.HostingEnvironment;

    if (env.IsDevelopment())
    {
        options.ListenAnyIP(8081); 
        options.ListenAnyIP(8444, listenOptions =>
        {
            listenOptions.UseHttps(); 
        });
    }
    else
    {
        var certPath = context.Configuration["ROOMSERVICE_CERT_PATH"];
        var certPassword = context.Configuration["ROOMSERVICE_CERT_PASSWORD"];

        if (string.IsNullOrEmpty(certPath) || string.IsNullOrEmpty(certPassword))
        {
            throw new InvalidOperationException(
                $"Certificate path or password is not configured. Path: {certPath}, Password: {(string.IsNullOrEmpty(certPassword) ? "Not Provided" : "Provided")}");
        }

        options.ListenAnyIP(8081); 
        options.ListenAnyIP(8444, listenOptions =>
        {
            listenOptions.UseHttps(certPath, certPassword);
        });
    }
});


builder.Services.AddMassTransit(config =>
{
    config.UsingRabbitMq((context, cfg) =>
    {
        var rabbitHost = builder.Configuration["RabbitMQ__Host"] ?? "localhost";
        var rabbitUser = builder.Configuration["RabbitMQ__UserName"] ?? "guest";
        var rabbitPass = builder.Configuration["RabbitMQ__Password"] ?? "guest";

        cfg.Host(rabbitHost, "/", hostConfig =>
        {
            hostConfig.Username(rabbitUser);
            hostConfig.Password(rabbitPass);
        });
    });
});


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("ReceptionistOrAdmin", policy => policy.RequireRole("Receptionist", "Admin"));
});

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
       
        var jwtSettings = builder.Configuration.GetSection("AuthSettings");
        var secret = jwtSettings.GetValue<string>("Secret");

        if (string.IsNullOrWhiteSpace(secret))
        {
            throw new InvalidOperationException("JWT Secret is not configured properly.");
        }

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.GetValue<string>("Issuer"),
            ValidAudience = jwtSettings.GetValue<string>("Audience"),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
        };
    });

builder.Services.AddAuthentication();




builder.Services.AddScoped<IRoomService, RoomService.Services.RoomService>();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<RoomDbContext>();
    dbContext.Database.Migrate();
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<JwtMiddleware>();


app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
app.UseCors("ApiGatewayPolicy");

app.Run();

