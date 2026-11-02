using BibliotecaApi_P4_LuisEnmanuel.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddScoped<DatabaseInitializer>();
builder.Services.AddScoped<EstudianteService>();
builder.Services.AddScoped<LibroService>();
builder.Services.AddScoped<PrestamoService>();
builder.Services.AddScoped<DevolucionService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    await initializer.InitializeAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
