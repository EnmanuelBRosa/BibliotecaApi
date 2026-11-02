using Dapper;
using Microsoft.Data.Sqlite;

namespace BibliotecaApi_P4_LuisEnmanuel.Services;

public class DatabaseInitializer(IConfiguration configuration)
{
    private readonly string _connectionString =
        configuration.GetConnectionString("DefaultConnection")!;

    public async Task InitializeAsync()
    {
        using var connection = new SqliteConnection(_connectionString);

        const string sql = """
             CREATE TABLE IF NOT EXISTS Estudiantes (
                           IdEstudiante INTEGER PRIMARY KEY AUTOINCREMENT,
                           Nombre TEXT NOT NULL,
                           Apellido TEXT NOT NULL,
                           Matricula TEXT NOT NULL UNIQUE,
                           Celular TEXT NOT NULL
                       );

                       CREATE TABLE IF NOT EXISTS Libros (
                           IdLibro INTEGER PRIMARY KEY AUTOINCREMENT,
                           Titulo TEXT NOT NULL,
                           Autor TEXT NOT NULL,
                           ISBN TEXT NOT NULL,
                           CantidadTotal INTEGER NOT NULL CHECK (CantidadTotal >= 0)
                       );

                       CREATE TABLE IF NOT EXISTS Prestamos (
                           IdPrestamo INTEGER PRIMARY KEY AUTOINCREMENT,
                           IdEstudiante INTEGER NOT NULL,
                           IdLibro INTEGER NOT NULL,
                           FechaPrestamo TEXT NOT NULL,
                           FechaVencimiento TEXT NOT NULL,
                           FOREIGN KEY (IdEstudiante) REFERENCES Estudiantes (IdEstudiante),
                           FOREIGN KEY (IdLibro) REFERENCES Libros (IdLibro)
                       );

                       CREATE TABLE IF NOT EXISTS Devoluciones (
                           IdDevolucion INTEGER PRIMARY KEY AUTOINCREMENT,
                           IdPrestamo INTEGER NOT NULL UNIQUE,
                           FechaDevolucion TEXT NOT NULL,
                           Observaciones TEXT,
                           FOREIGN KEY (IdPrestamo) REFERENCES Prestamos (IdPrestamo)
                       );
            """;

        await connection.ExecuteAsync(sql);
    }
}

