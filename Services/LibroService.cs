using BibliotecaApi_P4_LuisEnmanuel.Models;
using Dapper;
using Microsoft.Data.Sqlite;

namespace BibliotecaApi_P4_LuisEnmanuel.Services;

public class LibroService(IConfiguration configuration)
{
    private readonly string _connectionString =
        configuration.GetConnectionString("DefaultConnection")!;

    public async Task<IEnumerable<Libro>> GetAllAsync()
    {
        using var connection = new SqliteConnection(_connectionString);

        const string sql = """
            SELECT IdLibro, Titulo, Autor, ISBN, CantidadTotal
            FROM Libros
            """;

        return await connection.QueryAsync<Libro>(sql);
    }

    public async Task<Libro?> GetByIdAsync(long id)
    {
        using var connection = new SqliteConnection(_connectionString);

        const string sql = """
            SELECT IdLibro, Titulo, Autor, ISBN, CantidadTotal
            FROM Libros
            WHERE IdLibro = @Id
            """;

        return await connection.QueryFirstOrDefaultAsync<Libro>(sql, new { Id = id });
    }

    public async Task<Libro> CreateAsync(Libro libro)
    {
        using var connection = new SqliteConnection(_connectionString);

        const string sql = """
            INSERT INTO Libros (Titulo, Autor, ISBN, CantidadTotal)
            VALUES (@Titulo, @Autor, @ISBN, @CantidadTotal);
            SELECT last_insert_rowid();
            """;

        var id = await connection.ExecuteScalarAsync<long>(sql, libro);

        return libro with { IdLibro = id };
    }

    public async Task<bool> UpdateAsync(Libro libro)
    {
        using var connection = new SqliteConnection(_connectionString);

        const string sql = """
            UPDATE Libros
            SET Titulo = @Titulo,
                Autor = @Autor,
                ISBN = @ISBN,
                CantidadTotal = @CantidadTotal
            WHERE IdLibro = @IdLibro
            """;

        var filasAfectadas = await connection.ExecuteAsync(sql, libro);

        return filasAfectadas > 0;
    }

    public async Task<bool> TienePrestamosAsync(long id)
    {
        using var connection = new SqliteConnection(_connectionString);

        const string sql = """
            SELECT COUNT(1)
            FROM Prestamos
            WHERE IdLibro = @Id
            """;

        var cantidad = await connection.ExecuteScalarAsync<long>(sql, new { Id = id });

        return cantidad > 0;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        using var connection = new SqliteConnection(_connectionString);

        const string sql = """
            DELETE FROM Libros
            WHERE IdLibro = @Id
            """;

        var filasAfectadas = await connection.ExecuteAsync(sql, new { Id = id });

        return filasAfectadas > 0;
    }

    public async Task<long> GetPrestamosActivosAsync(long id)
    {
        using var connection = new SqliteConnection(_connectionString);

        const string sql = """
            SELECT COUNT(1)
            FROM Prestamos p
            WHERE p.IdLibro = @Id
              AND NOT EXISTS (
                  SELECT 1 FROM Devoluciones d WHERE d.IdPrestamo = p.IdPrestamo)
            """;

        return await connection.ExecuteScalarAsync<long>(sql, new { Id = id });
    }

    public async Task<long> GetDisponiblesAsync(long id)
    {
        var libro = await GetByIdAsync(id);

        if (libro is null)
        {
            return 0;
        }

        var activos = await GetPrestamosActivosAsync(id);

        return libro.CantidadTotal - activos;
    }
}