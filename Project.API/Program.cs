using Microsoft.EntityFrameworkCore;
using Project.BLL.Interfaces;
using Project.BLL.Services;
using Project.DAL.Data;
using Project.DAL.Interfaces;
using Project.DAL.Repositories;
using static System.Runtime.InteropServices.JavaScript.JSType;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Connection"),
        b => b.MigrationsAssembly("Project.DAL")
    ));



builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<PatientRepository>();
builder.Services.AddScoped<DoctorRepository>();
builder.Services.AddScoped<SpecializationRepository>();
builder.Services.AddScoped<DoctorAvailabilityRepository>();
builder.Services.AddScoped<AppointmentRepository>();
builder.Services.AddScoped<NotificationRepository>();


// DAL
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();

// BLL
// BLL
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// DAL Interfaces
builder.Services.AddScoped<IUserRepository, UserRepository>();


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();






// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
