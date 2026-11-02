using BibliotecaApi_P4_LuisEnmanuel.Models;
using BibliotecaApi_P4_LuisEnmanuel.Services;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaApi_P4_LuisEnmanuel.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LibrosController(LibroService libroService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Libro>>> GetAll()
    {
        return Ok(await libroService.GetAllAsync());
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<Libro>> GetById(long id)
    {
        var libro = await libroService.GetByIdAsync(id);

        if (libro is null)
        {
            return NotFound(new { mensaje = $"No existe el libro con id {id}." });
        }

        return Ok(libro);
    }

    [HttpGet("{id:long}/disponibles")]
    public async Task<IActionResult> GetDisponibles(long id)
    {
        var libro = await libroService.GetByIdAsync(id);

        if (libro is null)
        {
            return NotFound(new { mensaje = $"No existe el libro con id {id}." });
        }

        var disponibles = await libroService.GetDisponiblesAsync(id);

        return Ok(new { idLibro = id, cantidadTotal = libro.CantidadTotal, disponibles });
    }

    [HttpPost]
    public async Task<ActionResult<Libro>> Create(Libro libro)
    {
        var error = Validar(libro);

        if (error is not null)
        {
            return BadRequest(new { mensaje = error });
        }

        var creado = await libroService.CreateAsync(libro);

        return CreatedAtAction(nameof(GetById), new { id = creado.IdLibro }, creado);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<Libro>> Update(long id, Libro libro)
    {
        var error = Validar(libro);

        if (error is not null)
        {
            return BadRequest(new { mensaje = error });
        }

        if (await libroService.GetByIdAsync(id) is null)
        {
            return NotFound(new { mensaje = $"No existe el libro con id {id}." });
        }

        var activos = await libroService.GetPrestamosActivosAsync(id);

        if (libro.CantidadTotal < activos)
        {
            return Conflict(new
            {
                mensaje = $"No se puede dejar la cantidad en {libro.CantidadTotal}: hay {activos} préstamos activos."
            });
        }

        var actualizado = libro with { IdLibro = id };

        await libroService.UpdateAsync(actualizado);

        return Ok(actualizado);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        if (await libroService.GetByIdAsync(id) is null)
        {
            return NotFound(new { mensaje = $"No existe el libro con id {id}." });
        }

        if (await libroService.TienePrestamosAsync(id))
        {
            return Conflict(new { mensaje = "No se puede eliminar: el libro tiene préstamos registrados." });
        }

        await libroService.DeleteAsync(id);

        return NoContent();
    }

    private static string? Validar(Libro libro)
    {
        if (string.IsNullOrWhiteSpace(libro.Titulo))
        {
            return "El título es obligatorio.";
        }

        if (string.IsNullOrWhiteSpace(libro.Autor))
        {
            return "El autor es obligatorio.";
        }

        if (string.IsNullOrWhiteSpace(libro.ISBN)) 
        {
            return "El ISBN es obligatorio.";
        }

        if (libro.CantidadTotal < 0)
        {
            return "La cantidad total no puede ser negativa.";
        }

        return null;
    }
}