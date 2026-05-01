using IMT_Reservas.Server.Application.Services.Implementations;
using IMT_Reservas.Server.Application.Commands;
using IMT_Reservas.Server.Application.DTOs.Response;
public class CategoriaController : Controller<CategoriaDto, CategoriaService, CrearCategoriaComando, ActualizarCategoriaComando, EliminarCategoriaComando>
{
    public CategoriaController(CategoriaService servicio) : base(servicio) { }
}
