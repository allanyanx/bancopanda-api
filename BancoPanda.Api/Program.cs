using System.Text;
using System.Text.Json.Serialization;
using BancoPanda.Api.Data;
using BancoPanda.Api.Features.Accounts;
using BancoPanda.Api.Features.Auth;
using BancoPanda.Api.Features.Cards;
using BancoPanda.Api.Features.Movements;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using BancoPanda.Api.Features.Users;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"))
           .UseSnakeCaseNamingConvention());

builder.Services.AddOptions<JwtOptions>()
    .BindConfiguration(JwtOptions.Seccion)
    .Validate(jwt => jwt.Clave.Length >= 32,
        "Falta la clave del token. Ejecuta: dotnet user-secrets set \"Jwt:Clave\" \"<mínimo 32 caracteres>\"")
    .ValidateOnStart();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<IOptions<JwtOptions>>((bearer, jwtOptions) =>
    {
        var jwt = jwtOptions.Value;
        bearer.MapInboundClaims = false;
        bearer.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = jwt.Emisor,
            ValidAudience = jwt.Audiencia,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Clave)),
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UsersService>();
builder.Services.AddScoped<AccountsService>();
builder.Services.AddScoped<CardsService>();
builder.Services.AddScoped<MovementsService>();

builder.Services.AddProblemDetails();

builder.Services.AddCors();
builder.Services.AddControllers()
    .AddJsonOptions(json => json.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.ConfigureHttpJsonOptions(json => json.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddOpenApi();

var app = builder.Build();

// Crea o actualiza las tablas al arrancar (se activa con Database__AplicarMigraciones=true, p. ej. en Docker/Render)
if (app.Configuration.GetValue<bool>("Database:AplicarMigraciones"))
{
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
}

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
