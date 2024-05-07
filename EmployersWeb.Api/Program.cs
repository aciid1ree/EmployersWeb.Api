using EmployeesStore.App.Validators;
using EmployersStore.Core.Abstractions;
using EmployersWeb.Api;
using EmployersWeb.Api.Repositories;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<ValidationResult>();
builder.Services.AddControllers();
builder.Services.AddTransient<DapperDbContext>();
builder.Services.AddTransient<IEmployeesRepository, EmployeesRepository>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
