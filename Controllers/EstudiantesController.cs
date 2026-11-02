using BibliotecaApi_P4_LuisEnmanuel.Models;
using BibliotecaApi_P4_LuisEnmanuel.Services;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaApi_P4_LuisEnmanuel.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstudiantesController(EstudianteService estudianteService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Estudiante>>> GetAll()
    {
        return Ok(await estudianteService.GetAllAsync());
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<Estudiante>> GetById(long id)
    {
        var estudiante = await estudianteService.GetByIdAsync(id);

        if (estudiante is null)
        {
            return NotFound(new { mensaje = $"No existe el estudiante con id {id}." });
        }

        return Ok(estudiante);
    }

    [HttpPost]
    public async Task<ActionResult<Estudiante>> Create(Estudiante estudiante)
    {
        var error = Validar(estudiante);

        if (error is not null)
        {
            return BadRequest(new { mensaje = error });
        }

        if (await estudianteService.ExisteMatriculaAsync(estudiante.Matricula))
        {
            return Conflict(new { mensaje = $"Ya existe un estudiante con la matrícula {estudiante.Matricula}." });
        }

        var creado = await estudianteService.CreateAsync(estudiante);

        return CreatedAtAction(nameof(GetById), new { id = creado.IdEstudiante }, creado);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<Estudiante>> Update(long id, Estudiante estudiante)
    {
        var error = Validar(estudiante);

        if (error is not null)
        {
            return BadRequest(new { mensaje = error });
        }

        if (await estudianteService.GetByIdAsync(id) is null)
        {
            return NotFound(new { mensaje = $"No existe el estudiante con id {id}." });
        }

        if (await estudianteService.ExisteMatriculaAsync(estudiante.Matricula, id))
        {
            return Conflict(new { mensaje = $"Ya existe otro estudiante con la matrícula {estudiante.Matricula}." });
        }

        var actualizado = estudiante with { IdEstudiante = id };

        await estudianteService.UpdateAsync(actualizado);

        return Ok(actualizado);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        if (await estudianteService.GetByIdAsync(id) is null)
        {
            return NotFound(new { mensaje = $"No existe el estudiante con id {id}." });
        }

        if (await estudianteService.TienePrestamosAsync(id))
        {
            return Conflict(new { mensaje = "No se puede eliminar: el estudiante tiene préstamos registrados." });
        }

        await estudianteService.DeleteAsync(id);

        return NoContent();
    }

    private static string? Validar(Estudiante estudiante)
    {
        if (string.IsNullOrWhiteSpace(estudiante.Nombre))
        {
            return "El nombre es obligatorio.";
        }

        if (string.IsNullOrWhiteSpace(estudiante.Apellido))
        {
            return "El apellido es obligatorio.";
        }

        if (string.IsNullOrWhiteSpace(estudiante.Matricula))
        {
            return "La matrícula es obligatoria.";
        }

        if (string.IsNullOrWhiteSpace(estudiante.Celular))
        {
            return "El celular es obligatorio.";
        }

        return null;
    }
}