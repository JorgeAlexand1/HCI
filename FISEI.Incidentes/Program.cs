using FISEI.Incidentes.Core.Interfaces.IRepositories;
using FISEI.Incidentes.Core.Interfaces.IServices;
using FISEI.Incidentes.Infrastructure.Data;
using FISEI.Incidentes.Infrastructure.Data.Repositories;
using FISEI.Incidentes.Presentation.Components;
using Microsoft.EntityFrameworkCore;
using FISEI.Incidentes.Application.Services;        
using Microsoft.OpenApi.Models;  // ✅ CAMBIAR ESTA LÍNEA

namespace FISEI.Incidentes
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ---------------------------------------------------------
            // 1️⃣ Configurar la cadena de conexión a SQL Server
            // ---------------------------------------------------------
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            // ---------------------------------------------------------
            // 📦 Registrar Repositorios (Dependency Injection)
            // ---------------------------------------------------------
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IIncidenteRepository, IncidenteRepository>();
            builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            builder.Services.AddScoped<IAsignacionRepository, AsignacionRepository>();
            builder.Services.AddScoped<INotificacionRepository, NotificacionRepository>();
            builder.Services.AddScoped<IConocimientoRepository, ConocimientoRepository>();

            // ---------------------------------------------------------
            // 🎯 Registrar Servicios de Aplicación (Business Logic)
            // ---------------------------------------------------------
            builder.Services.AddScoped<IAsignacionService, AsignacionService>();
            builder.Services.AddScoped<IEscalamientoService, EscalamientoService>();
            builder.Services.AddScoped<INotificacionService, NotificacionService>();
            builder.Services.AddScoped<IConocimientoService, ConocimientoService>();
            // ---------------------------------------------------------
            // 2️⃣ Agregar servicios para Blazor y controladores API
            // ---------------------------------------------------------
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

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

            var app = builder.Build();

            // ---------------------------------------------------------
            // 4️⃣ Configurar el pipeline HTTP
            // ---------------------------------------------------------
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
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAntiforgery();

            app.MapControllers(); // ✅ Para habilitar los endpoints de la API

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
