using CRET.DataAccessLayer;
using CRET.Mapper;
using CRET.Repository.Implementation;
using CRET.Repository.Interface;
using CRET.Services.Implementation;
using CRET.Services.Interface;
using CRET.Web.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;
using NLog.Web;

namespace Cret.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            IConfiguration Configuration = builder.Configuration;
            // Add services to the container.
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddMicrosoftIdentityWebApi(Configuration);
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Host.UseNLog(); // Add this line to use NLog
            builder.Services.AddAutoMapper(typeof(MappingProfile));


            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                   options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Transient);

            //Regiset Service
            builder.Services.AddScoped<IDatabaseService, DatabaseService>();
            builder.Services.AddTransient<IOrganizationRepository, OrganizationRepository>();
            builder.Services.AddTransient<IUserRepository, UserRepository>();
            builder.Services.AddTransient<ISettingRepository, SettingRepository>();
            builder.Services.AddTransient<ICertificateRepository, CertificateRepository>();

            builder.Services.AddTransient<IEmailService, EmailService>();
            builder.Services.AddTransient<IQRCodeService, QRCodeService>();
            builder.Services.AddTransient<ICertificateService, CertificateService>();
            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseCors(policyBuilder =>
            {
                policyBuilder.AllowAnyOrigin();
                policyBuilder.AllowAnyHeader();
                policyBuilder.AllowAnyMethod();
            });
            app.UseSwagger();
            app.UseSwaggerUI();


            var databaseFilePath = Configuration.GetConnectionString("DefaultConnection").Replace("Data Source=", "");

            // Extract the folder path from the database file path
            var folderPath = Path.GetDirectoryName(databaseFilePath);

            // Check if the folder exists, if not, create it
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            //if (app.Environment.IsDevelopment())
            //{
            //    builder.Configuration.AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);
            //}
            //else if (app.Environment.IsStaging())
            //{
            //    builder.Configuration
            //        .AddJsonFile("appsettings.Staging.json", optional: false, reloadOnChange: true);
            //}
            //else if (app.Environment.IsProduction())
            //{
            //    builder.Configuration
            //        .AddJsonFile("appsettings.Prod.json", optional: false, reloadOnChange: true);
            //}

            app.UseHttpsRedirection();
            using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                if (context != null)
                {
                    context.Database.Migrate();
                }
            }
            IdentityModelEventSource.ShowPII = false;
            app.UseAuthentication();
            app.UseAuthorization();



            //Custom Authoriation Middleware
            app.UseAuthorizationMiddleware();

            app.MapControllers();

            app.MapFallbackToFile("/index.html");

            app.Run();
        }
    }
}
