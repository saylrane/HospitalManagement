using HospitalManagement.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer(); builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetValue<string>("ConnectionStrings:Default") ?? "Server=(localdb)\\mssqllocaldb;Database=HospitalDb;Trusted_Connection=True;";

builder.Services.AddDbContext<ApplicationDBContext>(opts => opts.UseSqlServer(connectionString));
builder.Services.AddTransient<IDbConnection>(_ => new SqlConnection(connectionString));
//builder.Services.AddScoped<GenericReadRepository>(); 
//builder.Services.AddScoped<PatientReadRepository>(); 
//builder.Services.AddScoped<AppointmentReadRepository>(); 
//builder.Services.AddScoped<IAppointmentService, AppointmentService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
