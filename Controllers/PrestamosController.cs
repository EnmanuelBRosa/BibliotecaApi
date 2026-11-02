using BibliotecaApi_P4_LuisEnmanuel.Models;
using BibliotecaApi_P4_LuisEnmanuel.Services;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaApi_P4_LuisEnmanuel.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrestamosController(
    PrestamoService prestamoService,
    EstudianteService estudianteService,
    LibroService libroService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Prestamo>>> GetAll()
    {
        return Ok(await prestamoService.GetAllAsync());
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<Prestamo>> GetById(long id)
    {
        var prestamo = await prestamoService.GetByIdAsync(id);

        if (prestamo is null)
        {
            return NotFound(new { mensaje = $"No existe el préstamo con id {id}." });
        }

        return Ok(prestamo);
    }

    [HttpPost]
    public async Task<ActionResult<Prestamo>> Create(Prestamo prestamo)
    {

        if (!DateTime.TryParse(prestamo.FechaPrestamo, out var fechaPrestamo))
        {
            return BadRequest(new { mensaje = "La fecha del préstamo es obligatoria y debe ser válida." });
        }

        if (!DateTime.TryParse(prestamo.FechaVencimiento, out var fechaVencimiento))
        {
            return BadRequest(new { mensaje = "La fecha de vencimiento es obligatoria y debe ser válida." });
        }

        if (fechaVencimiento < fechaPrestamo)
        {
            return BadRequest(new { mensaje = "La fecha de vencimiento no puede ser anterior a la fecha del préstamo." });
        }


        if (await estudianteService.GetByIdAsync(prestamo.IdEstudiante) is null)
        {
            return NotFound(new { mensaje = $"No existe el estudiante con id {prestamo.IdEstudiante}." });
        }

        if (await libroService.GetByIdAsync(prestamo.IdLibro) is null)
        {
            return NotFound(new { mensaje = $"No existe el libro con id {prestamo.IdLibro}." });
        }

        if (await libroService.GetDisponiblesAsync(prestamo.IdLibro) <= 0)
        {
            return Conflict(new { mensaje = "No hay unidades disponibles de este libro." });
        }

        var creado = await prestamoService.CreateAsync(prestamo);

        return CreatedAtAction(nameof(GetById), new { id = creado.IdPrestamo }, creado);
    }
}