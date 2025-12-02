using FISEI.Incidentes.Core.Interfaces.IRepositories;
using FISEI.Incidentes.Core.Interfaces.IServices;
using FISEI.Incidentes.Infrastructure.Data;
using FISEI.Incidentes.Infrastructure.Data.Repositories;
using FISEI.Incidentes.Presentation.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FISEI.Incidentes.Application.Services;        
using Microsoft.OpenApi.Models;
using FISEI.Incidentes.Infrastructure.Security;
using Microsoft.AspNetCore.Components.Authorization;
using FISEI.Incidentes.Presentation.Hubs;

namespace FISEI.Incidentes
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ---------------------------------------------------------
            // 1️⃣ Configurar la cadena de conexión a SQL Server + Identity
            // ---------------------------------------------------------
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            Console.WriteLine("[STARTUP] Connection string usado: " + connectionString);
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Sin ASP.NET Identity: autenticación gestionada por `Usuarios` + JWT

            // ---------------------------------------------------------
            // JWT Authentication
            // ---------------------------------------------------------
            var jwtSection = builder.Configuration.GetSection("Jwt");
            var secretKey = jwtSection.GetValue<string>("Key") ?? "CHANGEME_SUPER_SECRET_KEY"; // fallback
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSection.GetValue<string>("Issuer"),
                    ValidAudience = jwtSection.GetValue<string>("Audience"),
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
                };
            });

            // ---------------------------------------------------------
            // 📦 Registrar Repositorios (Dependency Injection)
            // ---------------------------------------------------------
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IIncidenteRepository, IncidenteRepository>();
            builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            builder.Services.AddScoped<IAsignacionRepository, AsignacionRepository>();
            builder.Services.AddScoped<INotificacionRepository, NotificacionRepository>();
            builder.Services.AddScoped<IConocimientoRepository, ConocimientoRepository>();
            builder.Services.AddScoped<IRolRepository, RolRepository>();

            // ---------------------------------------------------------
            // 🎯 Registrar Servicios de Aplicación (Business Logic)
            // ---------------------------------------------------------
            builder.Services.AddScoped<IAsignacionService, AsignacionService>();
            builder.Services.AddScoped<IEscalamientoService, EscalamientoService>();
            builder.Services.AddScoped<INotificacionService, NotificacionService>();
            builder.Services.AddScoped<IConocimientoService, ConocimientoService>();
            builder.Services.AddScoped<FISEI.Incidentes.Infrastructure.Services.EmailService>();
            builder.Services.AddScoped<SlaService>();
            
            // ---------------------------------------------------------
            // 📡 SignalR para notificaciones en tiempo real
            // ---------------------------------------------------------
            builder.Services.AddSignalR();
            
            // ---------------------------------------------------------
            // 🔐 Autenticación para Blazor Server
            // ---------------------------------------------------------
            builder.Services.AddScoped<FISEI.Incidentes.Infrastructure.Services.TokenProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddTransient<FISEI.Incidentes.Infrastructure.Services.AuthHeaderHandler>();
            
            // ---------------------------------------------------------
            // 2️⃣ Agregar servicios para Blazor y controladores API
            // ---------------------------------------------------------
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
            
            // Register HttpClient for Blazor components with AuthHeaderHandler
            builder.Services.AddScoped(sp =>
            {
                var handler = sp.GetRequiredService<FISEI.Incidentes.Infrastructure.Services.AuthHeaderHandler>();
                handler.InnerHandler = new HttpClientHandler();
                
                var httpClient = new HttpClient(handler)
                {
                    BaseAddress = new Uri(builder.Configuration["AppBaseUrl"] ?? "http://localhost:5023/")
                };
                return httpClient;
            });

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                });

            // ---------------------------------------------------------
            // 3️⃣ Agregar Swagger para probar el API
            // ---------------------------------------------------------
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo  // ✅ Ahora funciona con Microsoft.OpenApi.Models
                {
                    Title = "FISEI Incidentes API",
                    Version = "v1",
                    Description = "API para la gestión de incidentes en la FISEI"
                });
            });

            // Registrar DomainSeeder para poblar roles/usuarios de dominio
            builder.Services.AddScoped<DomainSeeder>();

            var app = builder.Build();

            // ---------------------------------------------------------
            // 4️⃣ Configurar el pipeline HTTP
            // ---------------------------------------------------------
            
            // Habilitar archivos estáticos (CSS, JS, imágenes)
            app.UseStaticFiles();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FISEI Incidentes API v1");
                });
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseAntiforgery();

            app.MapControllers(); // ✅ Para habilitar los endpoints de la API
            app.MapHub<NotificationHub>("/notificationHub"); // ✅ SignalR Hub

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            // Aplicar migraciones y seeding de la base de datos
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await db.Database.MigrateAsync();
                var domainSeeder = scope.ServiceProvider.GetRequiredService<DomainSeeder>();
                await domainSeeder.SeedAsync();
            }

            app.Run();
        }
    }
}
