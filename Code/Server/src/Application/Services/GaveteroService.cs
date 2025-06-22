using System.Data;
using IMT_Reservas.Server.Shared.Common;
public class GaveteroService : IGaveteroService
{
    private readonly IGaveteroRepository _gaveteroRepository;
    public GaveteroService(IGaveteroRepository gaveteroRepository) => _gaveteroRepository = gaveteroRepository;

    public void CrearGavetero(CrearGaveteroComando comando)
    {
        ValidarEntradaCreacion(comando);
        _gaveteroRepository.Crear(comando);
    }

    public void ActualizarGavetero(ActualizarGaveteroComando comando)
    {
        ValidarEntradaActualizacion(comando);
        _gaveteroRepository.Actualizar(comando);
    }

    public void EliminarGavetero(EliminarGaveteroComando comando)
    {
        ValidarEntradaEliminacion(comando);
        _gaveteroRepository.Eliminar(comando.Id);
    }

    public List<GaveteroDto>? ObtenerTodosGaveteros()
    {
        DataTable resultado = _gaveteroRepository.ObtenerTodos();
        var lista = new List<GaveteroDto>(resultado.Rows.Count);
        foreach (DataRow fila in resultado.Rows)
            lista.Add(MapearFilaADto(fila));
        return lista;
    }

    private void ValidarEntradaCreacion(CrearGaveteroComando comando)
    {
        if (comando == null) throw new ArgumentNullException();
        if (string.IsNullOrWhiteSpace(comando.Nombre)) throw new ErrorNombreRequerido();
        if (string.IsNullOrWhiteSpace(comando.NombreMueble)) throw new ErrorNombreMuebleRequerido();
        if (comando.Longitud.HasValue && comando.Longitud <= 0) throw new ErrorValorNegativo("longitud");
        if (comando.Profundidad.HasValue && comando.Profundidad <= 0) throw new ErrorValorNegativo("profundidad");
        if (comando.Altura.HasValue && comando.Altura <= 0) throw new ErrorValorNegativo("altura");
    }
    private void ValidarEntradaActualizacion(ActualizarGaveteroComando comando)
    {
        if (comando == null) throw new ArgumentNullException();
        if (comando.Id <= 0) throw new ErrorIdInvalido();
        if (!string.IsNullOrWhiteSpace(comando.Nombre) && comando.Nombre.Length > 255) throw new ErrorLongitudInvalida("nombre gavetero", 255);
        if (!string.IsNullOrWhiteSpace(comando.NombreMueble) && comando.NombreMueble.Length > 255) throw new ErrorLongitudInvalida("nombre mueble", 255);
        if (comando.Longitud.HasValue && comando.Longitud <= 0) throw new ErrorValorNegativo("longitud");
        if (comando.Profundidad.HasValue && comando.Profundidad <= 0) throw new ErrorValorNegativo("profundidad");
        if (comando.Altura.HasValue && comando.Altura <= 0) throw new ErrorValorNegativo("altura");
    }
    private void ValidarEntradaEliminacion(EliminarGaveteroComando comando)
    {
        if (comando == null) throw new ArgumentNullException();
        if (comando.Id <= 0) throw new ErrorIdInvalido();
    }
    private static GaveteroDto MapearFilaADto(DataRow fila) => new GaveteroDto
    {
        Id = Convert.ToInt32(fila["id_gavetero"]),
        Nombre = fila["nombre_gavetero"] == DBNull.Value ? null : Convert.ToString(fila["nombre_gavetero"]),
        Tipo = fila["tipo_gavetero"] == DBNull.Value ? null : Convert.ToString(fila["tipo_gavetero"]),
        NombreMueble = fila["nombre_mueble"] == DBNull.Value ? null : Convert.ToString(fila["nombre_mueble"]),
        Longitud = fila["longitud_gavetero"] == DBNull.Value ? null : Convert.ToDouble(fila["longitud_gavetero"]),
        Profundidad = fila["profundidad_gavetero"] == DBNull.Value ? null : Convert.ToDouble(fila["profundidad_gavetero"]),
        Altura = fila["altura_gavetero"] == DBNull.Value ? null : Convert.ToDouble(fila["altura_gavetero"])
    };
}