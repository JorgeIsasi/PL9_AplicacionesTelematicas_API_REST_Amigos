using Amigos.DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Amigos.Services;


namespace Amigos
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Leer la ruta desde la configuración
            var firebaseCredentialPath = builder.Configuration["Firebase:CredentialPath"];
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(firebaseCredentialPath)
            });


            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Inyecta el servicio FCM
            builder.Services.AddScoped<Amigos.Services.FcmService>();
            builder.Services.AddScoped<Amigos.Services.NotificationService>();


            //Para inyeccion de dependencias
            builder.Services.AddSingleton<IInc, IncImpl>();  // Mantiene el valor
                                                             //builder.Services.AddScoped<IInc, IncImpl>();  // Se reinicia por solicitud HTTP
                                                             //builder.Services.AddTransient<IInc, IncImpl>(); // Se reinicia en cada llamada

            // Para añadir la base de datos como dependencia
            builder.Services.AddDbContext<AmigoDBContext>(options =>
                         options.UseSqlite("Data Source=Amigos.db"));


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapControllerRoute(
                name: "Prueba",
                 pattern: "{controller}/{action}/{valor}/{veces}");

            app.Run();
        }
    }
}