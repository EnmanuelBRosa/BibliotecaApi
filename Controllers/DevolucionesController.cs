using BibliotecaApi_P4_LuisEnmanuel.Models;
using BibliotecaApi_P4_LuisEnmanuel.Services;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaApi_P4_LuisEnmanuel.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DevolucionesController(
    DevolucionService devolucionService,
    PrestamoService prestamoService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Devolucion>>> GetAll()
    {
        return Ok(await devolucionService.GetAllAsync());
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<Devolucion>> GetById(long id)
    {
        var devolucion = await devolucionService.GetByIdAsync(id);

        if (devolucion is null)
        {
            return NotFound(new { mensaje = $"No existe la devolución con id {id}." });
        }

        return Ok(devolucion);
    }

    [HttpPost]
    public async Task<ActionResult<Devolucion>> Create(Devolucion devolucion)
    {
        if (!DateTime.TryParse(devolucion.FechaDevolucion, out var fechaDevolucion))

        {
            return BadRequest(new { mensaje = "La fecha de devolución es obligatoria." });
        }

        var prestamo = await prestamoService.GetByIdAsync(devolucion.IdPrestamo);

        if (prestamo is null)
        {
            return NotFound(new { mensaje = $"No existe el préstamo con id {devolucion.IdPrestamo}." });
        }

        if (await devolucionService.ExisteDevolucionAsync(devolucion.IdPrestamo))
        {
            return Conflict(new { mensaje = "Este préstamo ya fue devuelto." });
        }


        if (!DateTime.TryParse(prestamo.FechaPrestamo, out var fechaPrestamo) || fechaDevolucion < fechaPrestamo)
        {
            return BadRequest(new { mensaje = "La fecha de devolución no puede ser anterior a la fecha del préstamo." });
        }

        var creada = await devolucionService.CreateAsync(devolucion);

        return CreatedAtAction(nameof(GetById), new { id = creada.IdDevolucion }, creada);
    }
}