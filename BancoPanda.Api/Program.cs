using System.Text;
using BancoPanda.Api.Common;
using BancoPanda.Api.Data;
using BancoPanda.Api.Features.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using BancoPanda.Api.Features.Users;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"))
           .UseSnakeCaseNamingConvention());

// JWT: la clave viene de User Secrets y se valida al arrancar la API.
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
        bearer.MapInboundClaims = false; // Conserva los nombres originales de los claims ("sub", "email")
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

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ManejadorErrores>();

builder.Services.AddCors();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Solo en desarrollo: permite probar la app desde el navegador (flutter run -d chrome).
    // La app Android no necesita CORS.
    app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
