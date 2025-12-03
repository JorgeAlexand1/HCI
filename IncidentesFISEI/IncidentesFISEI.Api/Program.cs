using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using IncidentesFISEI.Application.Interfaces;
using IncidentesFISEI.Application.Services;
using IncidentesFISEI.Domain.Interfaces;
using IncidentesFISEI.Infrastructure.Data;
using IncidentesFISEI.Infrastructure.Repositories;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configuración de Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "🎓 IncidentesFISEI API",
        Version = "v1.0.0",
        Description = "API REST para la gestión integral de incidentes de la Facultad de Ingeniería en Sistemas de la UTA basado en ITIL v3",
        Contact = new OpenApiContact
        {
            Name = "Soporte Técnico FISEI",
            Email = "soporte@fisei.uta.edu.ec"
        }
    });

    // Configuración para incluir comentarios XML
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Configuración avanzada de UI
    c.EnableAnnotations();
    c.DescribeAllParametersInCamelCase();
    c.CustomSchemaIds(type => type.FullName?.Replace("+", "."));

    // Configuración de seguridad JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "🔐 JWT Authorization header usando el esquema Bearer. Ejemplo: 'Bearer {token}'"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configuración de Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuración de JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey no configurado");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins("http://localhost:5001", "https://localhost:5001", "http://localhost:5000", "https://localhost:7000", "https://localhost:7001")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Registro de repositorios
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IIncidenteRepository, IncidenteRepository>();
builder.Services.AddScoped<ICategoriaIncidenteRepository, CategoriaIncidenteRepository>();
builder.Services.AddScoped<IArticuloConocimientoRepository, ArticuloConocimientoRepository>();
builder.Services.AddScoped<IComentarioRepository, ComentarioRepository>();
builder.Services.AddScoped<IArchivoAdjuntoRepository, ArchivoAdjuntoRepository>();
builder.Services.AddScoped<IServicioDITICRepository, ServicioDITICRepository>();
builder.Services.AddScoped<IEtiquetaConocimientoRepository, EtiquetaConocimientoRepository>();
builder.Services.AddScoped<IVersionArticuloRepository, VersionArticuloRepository>();
builder.Services.AddScoped<IValidacionArticuloRepository, ValidacionArticuloRepository>();
builder.Services.AddScoped<INotificacionRepository, NotificacionRepository>();
builder.Services.AddScoped<IPlantillaEncuestaRepository, PlantillaEncuestaRepository>();
builder.Services.AddScoped<IPreguntaEncuestaRepository, PreguntaEncuestaRepository>();
builder.Services.AddScoped<IEncuestaRepository, EncuestaRepository>();
builder.Services.AddScoped<IRespuestaEncuestaRepository, RespuestaEncuestaRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();

// Registro de servicios de aplicación
builder.Services.AddScoped<IAuthService, IncidentesFISEI.Infrastructure.Services.AuthService>();
builder.Services.AddScoped<IEmailService, IncidentesFISEI.Infrastructure.Services.EmailService>();
builder.Services.AddScoped<IAsignacionService, IncidentesFISEI.Application.Services.AsignacionService>();
builder.Services.AddScoped<IEscalacionService, IncidentesFISEI.Application.Services.EscalacionService>();
builder.Services.AddScoped<IMetricasService, IncidentesFISEI.Application.Services.MetricasService>();
builder.Services.AddScoped<IConocimientoService, IncidentesFISEI.Application.Services.ConocimientoService>();
builder.Services.AddScoped<INotificacionService, IncidentesFISEI.Application.Services.NotificacionService>();
builder.Services.AddScoped<IEncuestaService, IncidentesFISEI.Application.Services.EncuestaService>();
builder.Services.AddScoped<IAuditLogService, IncidentesFISEI.Application.Services.AuditLogService>();
// TODO: Implementar servicios de aplicación restantes
// builder.Services.AddScoped<IIncidenteService, IncidenteService>();
// builder.Services.AddScoped<IComentarioService, ComentarioService>();
// builder.Services.AddScoped<ICategoriaService, CategoriaService>();
// builder.Services.AddScoped<IUsuarioService, UsuarioService>();

// Registro de Background Services (ITIL v4)
builder.Services.AddHostedService<IncidentesFISEI.Infrastructure.Services.SLAMonitoringService>();
builder.Services.AddHostedService<IncidentesFISEI.Infrastructure.Services.EscalacionAutomaticaService>();
builder.Services.AddHostedService<IncidentesFISEI.Infrastructure.Services.RecurrenciaDetectionService>();
builder.Services.AddHostedService<IncidentesFISEI.Infrastructure.Services.AuditLogCleanupService>();

// AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
// Swagger habilitado para todos los ambientes (Development y Production)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "IncidentesFISEI API v1");
    c.RoutePrefix = "swagger"; // Swagger UI en /swagger
    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
    c.DefaultModelsExpandDepth(-1);
    c.DisplayRequestDuration();
});

app.UseHttpsRedirection();
app.UseCors("AllowBlazorClient");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Ejecutar migraciones automáticamente en desarrollo
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        await context.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error aplicando migraciones de base de datos");
    }
}

app.Run();