namespace BibliotecaApi_P4_LuisEnmanuel.Models;

public record Devolucion (long IdDevolucion, long IdPrestamo, string FechaDevolucion , string? Observaciones);
