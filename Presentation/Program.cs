using Application.Services;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configurar serviços
builder.Services.AddControllers();
builder.Services.AddEndpointUSUARIOpiExplorer();
// Adicione o CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // Configurar segurança para JWT
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement{
    {
        new OpenApiSecurityScheme{
            Reference = new OpenApiReference{
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        new string[]{}
    }});
});

// Configurar o DbContext com SQL Server
builder.Services.AddDbContext<HubDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HUB")));

builder.Services.AddDbContext<Eletrofast_2019DbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Eletrofast_2019")));

// Registrar repositórios e serviços
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddSingleton<PasswordHasher>();

// Adicione a chave JWT
var jwtSecret = builder.Configuration.GetValue<string>("JwtSecret");
if (string.IsNullOrEmpty(jwtSecret))
{
    throw new ArgumentNullException("JwtSecret", "JWT Secret is not configured.");
}

builder.Services.AddScoped<AuthenticationService>(provider =>
{
    var userRepository = provider.GetRequiredService<IUserRepository>();
    var passwordHasher = provider.GetRequiredService<PasswordHasher>();
    return new AuthenticationService(userRepository, passwordHasher, jwtSecret);
});

builder.Services.AddScoped<IOrcamentoRepository, OrcamentoRepository>();
builder.Services.AddScoped<OrcamentoService>();

var key = Encoding.ASCII.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false, // Pode ser alterado para true se necessário
        ValidateAudience = false, // Pode ser alterado para true se necessário
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Habilitar detalhes de exceções e configurar Swagger
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty; // Define Swagger UI na raiz (opcional)
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseCors();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication(); // Habilitar autenticação
app.UseAuthorization();
app.MapControllers();
app.Run();
