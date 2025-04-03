using System.Reflection;
using BackendAppProducto.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace BackendAppProducto
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configuraci�n de servicios
            ConfigureServices(builder);

            var app = builder.Build();

            // Configuraci�n del pipeline HTTP
            ConfigurePipeline(app);

            app.Run();
        }

        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            // Configuraci�n de PostgreSQL
            var connectionString = builder.Configuration.GetConnectionString("PostgreSQLConnection");
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            // Configuraci�n de CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // Configuraci�n de controladores
            builder.Services.AddControllers();

            // Configuraci�n de Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Product API",
                    Description = "API para realizar CRUD de Productos",
                    Contact = new OpenApiContact
                    {
                        Name = "Pedro Ramirez Gonzalez GitHub",
                        Url = new Uri("https://github.com/P3droRamirez")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    }
                });

                // Configuraci�n de documentaci�n XML
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);

                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            });
        }

        private static void ConfigurePipeline(WebApplication app)
        {
            // Configuraci�n del entorno de desarrollo
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product API v1");
                });
            }

            // Middlewares
            app.UseHttpsRedirection();
            app.UseCors("AllowAll"); // Aseg�rate que est� despu�s de UseHttpsRedirection y antes de UseAuthorization
            app.UseAuthorization();
            app.MapControllers();

            // Aplicar migraciones autom�ticamente (opcional)
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.Migrate();
            }
        }
    }
}