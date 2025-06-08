using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

[ApiController]
[Route("api/[controller]")]
public class EmpresaMantenimientoController : ControllerBase
{
    private readonly ICrearEmpresaMantenimientoComando _crear;
    private readonly IObtenerEmpresaMantenimientoConsulta _obtener;
    private readonly IActualizarEmpresaMantenimientoComando _actualizar;
    private readonly IEliminarEmpresaMantenimientoComando _eliminar;

    public EmpresaMantenimientoController(ICrearEmpresaMantenimientoComando crear, 
                                         IObtenerEmpresaMantenimientoConsulta obtener,
                                         IActualizarEmpresaMantenimientoComando actualizar, 
                                         IEliminarEmpresaMantenimientoComando eliminar)
    {
        _crear = crear;
        _obtener = obtener;
        _actualizar = actualizar;
        _eliminar = eliminar;
    }

    [HttpPost]
    public ActionResult Crear([FromBody] EmpresaMantenimientoRequestDto dto)
    {
        try
        {
            if (dto == null)
                return BadRequest("Los datos de la empresa de mantenimiento son requeridos");

            if (string.IsNullOrWhiteSpace(dto.NombreEmpresa))
                return BadRequest("El nombre de la empresa es requerido");

            var comando = new CrearEmpresaMantenimientoComando(
                dto.NombreEmpresa,
                dto.NombreResponsable,
                dto.ApellidoResponsable,
                dto.Telefono,
                dto.Direccion,
                dto.Nit
            );

            _crear.Handle(comando);
            return Created();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al crear empresa de mantenimiento: {ex.Message}");
        }
    }

    [HttpGet]
    public ActionResult<List<EmpresaMantenimientoDto>> ObtenerTodos()
    {
        try
        {
            var resultado = _obtener.Handle();
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al obtener empresas de mantenimiento: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public ActionResult Actualizar([Range(1, int.MaxValue)] int id, [FromBody] EmpresaMantenimientoRequestDto dto)
    {
        try
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0");

            if (dto == null)
                return BadRequest("Los datos de la empresa de mantenimiento son requeridos");

            var comando = new ActualizarEmpresaMantenimientoComando(
                id,
                dto.NombreEmpresa,
                dto.NombreResponsable,
                dto.ApellidoResponsable,
                dto.Telefono,
                dto.Direccion,
                dto.Nit
            );

            _actualizar.Handle(comando);
            return Ok("Empresa de mantenimiento actualizada exitosamente");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al actualizar empresa de mantenimiento: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public ActionResult Eliminar([Range(1, int.MaxValue)] int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0");

            var comando = new EliminarEmpresaMantenimientoComando(id);
            _eliminar.Handle(comando);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al eliminar empresa de mantenimiento: {ex.Message}");
        }
    }
}