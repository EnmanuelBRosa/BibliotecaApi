using BibliotecaApi_P4_LuisEnmanuel.Models;
using Dapper;
using Microsoft.Data.Sqlite;

namespace BibliotecaApi_P4_LuisEnmanuel.Services;

public class PrestamoService(IConfiguration configuration)
{
    private readonly string _connectionString =
        configuration.GetConnectionString("DefaultConnection")!;

    public async Task<IEnumerable<Prestamo>> GetAllAsync()
    {
        using var connection = new SqliteConnection(_connectionString);

        const string sql = """
            SELECT IdPrestamo, IdEstudiante, IdLibro, FechaPrestamo, FechaVencimiento
            FROM Prestamos
            """;

        return await connection.QueryAsync<Prestamo>(sql);
    }

    public async Task<Prestamo?> GetByIdAsync(long id)
    {
        using var connection = new SqliteConnection(_connectionString);

        const string sql = """
            SELECT IdPrestamo, IdEstudiante, IdLibro, FechaPrestamo, FechaVencimiento
            FROM Prestamos
            WHERE IdPrestamo = @Id
            """;

        return await connection.QueryFirstOrDefaultAsync<Prestamo>(sql, new { Id = id });
    }

    public async Task<Prestamo> CreateAsync(Prestamo prestamo)
    {
        using var connection = new SqliteConnection(_connectionString);

        const string sql = """
            INSERT INTO Prestamos (IdEstudiante, IdLibro, FechaPrestamo, FechaVencimiento)
            VALUES (@IdEstudiante, @IdLibro, @FechaPrestamo, @FechaVencimiento);
            SELECT last_insert_rowid();
            """;

        var id = await connection.ExecuteScalarAsync<long>(sql, prestamo);

        return prestamo with { IdPrestamo = id };
    }
}