using BibliotecaApi_P4_LuisEnmanuel.Models;
using Dapper;
using Microsoft.Data.Sqlite;

namespace BibliotecaApi_P4_LuisEnmanuel.Services;

public class DevolucionService(IConfiguration configuration)
{
    private readonly string _connectionString =
        configuration.GetConnectionString("DefaultConnection")!;
    public async Task<IEnumerable<Devolucion>> GetAllAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        const string sql = """
            SELECT IdDevolucion, IdPrestamo, FechaDevolucion, Observaciones
            FROM Devoluciones
            """;
        return await connection.QueryAsync<Devolucion>(sql);
    }
    public async Task<Devolucion?> GetByIdAsync(long id)
    {
        using var connection = new SqliteConnection(_connectionString);
        const string sql = """
            SELECT IdDevolucion, IdPrestamo, FechaDevolucion, Observaciones
            FROM Devoluciones
            WHERE IdDevolucion = @Id
            """;
        return await connection.QueryFirstOrDefaultAsync<Devolucion>(sql, new { Id = id });
    }


    public async Task<bool> ExisteDevolucionAsync(long idPrestamo)
    {
        using var connection = new SqliteConnection(_connectionString);

        const string sql = """
            SELECT COUNT(1)
            FROM Devoluciones
            WHERE IdPrestamo = @IdPrestamo
            """;

        var cantidad = await connection.ExecuteScalarAsync<long>(sql, new { IdPrestamo = idPrestamo });

        return cantidad > 0;
    }

    public async Task<Devolucion> CreateAsync(Devolucion devolucion)
    {
        using var connection = new SqliteConnection(_connectionString);

        const string sql = """
            INSERT INTO Devoluciones (IdPrestamo, FechaDevolucion, Observaciones)
            VALUES (@IdPrestamo, @FechaDevolucion, @Observaciones);
            SELECT last_insert_rowid();
            """;

        var id = await connection.ExecuteScalarAsync<long>(sql, devolucion);

        return devolucion with { IdDevolucion = id };
    }

}