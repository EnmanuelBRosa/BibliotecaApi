namespace BibliotecaApi_P4_LuisEnmanuel.Models;

public record Prestamo(long IdPrestamo, long IdEstudiante, long IdLibro, string FechaPrestamo, string FechaVencimiento)
{
    public Prestamo() : this(0, 0, 0, string.Empty, string.Empty) { }
}