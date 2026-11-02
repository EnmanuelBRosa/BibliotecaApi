using BibliotecaApi_P4_LuisEnmanuel.Models;
using Dapper;
using Microsoft.Data.Sqlite;

namespace BibliotecaApi_P4_LuisEnmanuel.Services;

public class EstudianteService(IConfiguration configuration)
{
    private readonly string _connectionString =
        configuration.GetConnectionString("DefaultConnection")!;

    public async Task<IEnumerable<Estudiante>> GetAllAsync()
    {
        using var connection = new SqliteConnection(_connectionString);

        const string sql = """
            SELECT IdEstudiante, Nombre, Apellido, Matricula, Celular
            FROM Estudiantes
            """;

        return await connection.QueryAsync<Estudiante>(sql);
    }

    public async Task<Estudiante?> GetByIdAsync(long id)
    {
        using var connection = new SqliteConnection(_connectionString);

        const string sql = """
            SELECT IdEstudiante, Nombre, Apellido, Matricula, Celular
            FROM Estudiantes
            WHERE IdEstudiante = @Id
            """;

        return await connection.QueryFirstOrDefaultAsync<Estudiante>(sql, new { Id = id });
    }

    public async Task<bool> ExisteMatriculaAsync(string matricula, long excluirId = 0)
    {
        using var connection = new SqliteConnection(_connectionString);

        const string sql = """
            SELECT COUNT(1)
            FROM Estudiantes
            WHERE Matricula = @Matricula AND IdEstudiante <> @ExcluirId
            """;

        var cantidad = await connection.ExecuteScalarAsync<long>(
            sql, new { Matricula = matricula, ExcluirId = excluirId });

        return cantidad > 0;
    }

    public async Task<Estudiante> CreateAsync(Estudiante estudiante)
    {
        using var connection = new SqliteConnection(_connectionString);

        const string sql = """
            INSERT INTO Estudiantes (Nombre, Apellido, Matricula, Celular)
            VALUES (@Nombre, @Apellido, @Matricula, @Celular);
            SELECT last_insert_rowid();
            """;

        var id = await connection.ExecuteScalarAsync<long>(sql, estudiante);

        return estudiante with { IdEstudiante = id };
    }

    public async Task<bool> UpdateAsync(Estudiante estudiante)
    {
        using var connection = new SqliteConnection(_connectionString);

        const string sql = """
            UPDATE Estudiantes
            SET Nombre = @Nombre,
                Apellido = @Apellido,
                Matricula = @Matricula,
                Celular = @Celular
            WHERE IdEstudiante = @IdEstudiante
            """;

        var filasAfectadas = await connection.ExecuteAsync(sql, estudiante);

        return filasAfectadas > 0;
    }

    public async Task<bool> TienePrestamosAsync(long id)
    {
        using var connection = new SqliteConnection(_connectionString);

        const string sql = """
            SELECT COUNT(1)
            FROM Prestamos
            WHERE IdEstudiante = @Id
            """;

        var cantidad = await connection.ExecuteScalarAsync<long>(sql, new { Id = id });

        return cantidad > 0;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        using var connection = new SqliteConnection(_connectionString);

        const string sql = """
            DELETE FROM Estudiantes
            WHERE IdEstudiante = @Id
            """;

        var filasAfectadas = await connection.ExecuteAsync(sql, new { Id = id });

        return filasAfectadas > 0;
    }
}