using Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(opt => {

    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));

});
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("CorsPolicy", Policy =>
    {
        Policy.AllowAnyHeader()
              .AllowAnyMethod()
              .WithOrigins("http://localhost:4200")
       
             ;
});
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCors("CorsPolicy");
app.MapControllers();

app.Run();
