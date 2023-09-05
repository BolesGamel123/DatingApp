

using Api.Data;
using Api.Helpers;
using Api.Interfaces;
using Api.Services;
using Microsoft.EntityFrameworkCore;

namespace Api.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,
            IConfiguration config)
        {
            services.AddDbContext<DataContext>(opt => 
            {

               opt.UseSqlite(config.GetConnectionString("DefaultConnection"));

            });
            services.AddCors(opt =>
            {
            opt.AddPolicy("CorsPolicy", Policy =>
            {
            Policy.AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithOrigins("http://localhost:4200")

                    ;
            });
            });

            services.AddScoped<ITokenService,TokenService>();
            services.AddScoped<IUserRepository,UserRepository>(); 
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
             services.AddScoped<IPhotoService,PhotoService>();
        
 
              return services;
        }
    }
}