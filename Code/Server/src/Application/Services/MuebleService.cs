using System.Data;
using IMT_Reservas.Server.Shared.Common;
public class MuebleService : IMuebleService
{
    private readonly IMuebleRepository _muebleRepository;
    public MuebleService(IMuebleRepository muebleRepository) => _muebleRepository = muebleRepository;

    public void CrearMueble(CrearMuebleComando comando)
    {
        ValidarEntradaCreacion(comando);
        _muebleRepository.Crear(comando);
    }

    public void ActualizarMueble(ActualizarMuebleComando comando)
    {
        ValidarEntradaActualizacion(comando);
        _muebleRepository.Actualizar(comando);
    }

    public void EliminarMueble(EliminarMuebleComando comando)
    {
        ValidarEntradaEliminacion(comando);
        _muebleRepository.Eliminar(comando.Id);
    }

    public List<MuebleDto>? ObtenerTodosMuebles()
    {
        var resultado = _muebleRepository.ObtenerTodos();
        var lista = new List<MuebleDto>(resultado.Rows.Count);
        foreach (DataRow fila in resultado.Rows)
            lista.Add(MapearFilaADto(fila));
        return lista;
    }

    private void ValidarEntradaCreacion(CrearMuebleComando comando)
    {
        if (comando == null) throw new ArgumentNullException();
        if (string.IsNullOrWhiteSpace(comando.Nombre)) throw new ErrorNombreRequerido();
        if (comando.Costo.HasValue && comando.Costo < 0) throw new ErrorValorNegativo("costo");
        if (comando.Longitud.HasValue && comando.Longitud <= 0) throw new ErrorValorNegativo("longitud");
        if (comando.Profundidad.HasValue && comando.Profundidad <= 0) throw new ErrorValorNegativo("profundidad");
        if (comando.Altura.HasValue && comando.Altura <= 0) throw new ErrorValorNegativo("altura");
    }
    private void ValidarEntradaActualizacion(ActualizarMuebleComando comando)
    {
        if (comando == null) throw new ArgumentNullException();
        if (comando.Id <= 0) throw new ErrorIdInvalido();
        if (!string.IsNullOrWhiteSpace(comando.Nombre) && comando.Nombre.Length > 255) throw new ErrorLongitudInvalida("nombre mueble", 255);
        if (comando.Costo.HasValue && comando.Costo < 0) throw new ErrorValorNegativo("costo");
        if (comando.Longitud.HasValue && comando.Longitud <= 0) throw new ErrorValorNegativo("longitud");
        if (comando.Profundidad.HasValue && comando.Profundidad <= 0) throw new ErrorValorNegativo("profundidad");
        if (comando.Altura.HasValue && comando.Altura <= 0) throw new ErrorValorNegativo("altura");
    }
    private void ValidarEntradaEliminacion(EliminarMuebleComando comando)
    {
        if (comando == null) throw new ArgumentNullException();
        if (comando.Id <= 0) throw new ErrorIdInvalido();
    }
    private static MuebleDto MapearFilaADto(DataRow fila) => new MuebleDto
    {
        Id = Convert.ToInt32(fila["id_mueble"]),
        Nombre = fila["nombre_mueble"] == DBNull.Value ? null : fila["nombre_mueble"].ToString(),
        NumeroGaveteros = fila["numero_gaveteros_mueble"] == DBNull.Value ? null : Convert.ToInt32(fila["numero_gaveteros_mueble"]),
        Ubicacion = fila["ubicacion_mueble"] == DBNull.Value ? null : fila["ubicacion_mueble"].ToString(),
        Tipo = fila["tipo_mueble"] == DBNull.Value ? null : fila["tipo_mueble"].ToString(),
        Costo = fila["costo_mueble"] == DBNull.Value ? null : Convert.ToDouble(fila["costo_mueble"]),
        Longitud = fila["longitud_mueble"] == DBNull.Value ? null : Convert.ToDouble(fila["longitud_mueble"]),
        Profundidad = fila["profundidad_mueble"] == DBNull.Value ? null : Convert.ToDouble(fila["profundidad_mueble"]),
        Altura = fila["altura_mueble"] == DBNull.Value ? null : Convert.ToDouble(fila["altura_mueble"])
    };
}