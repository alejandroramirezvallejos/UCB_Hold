using System.Data;
using Shared.Common;
public class EquipoService : IEquipoService
{
    private readonly EquipoRepository _equipoRepository;

    public EquipoService(EquipoRepository equipoRepository)
    {
        _equipoRepository = equipoRepository;
    }    public void CrearEquipo(CrearEquipoComando comando)
    {
        try
        {
            ValidarEntradaCreacion(comando);
            _equipoRepository.Crear(comando);
        }
        catch (ErrorNombreRequerido)
        {
            throw;
        }
        catch (ErrorValorNegativo)
        {
            throw;
        }
        catch (ErrorIdInvalido)
        {
            throw;
        }
        catch (Exception ex)
        {
            var parametros = new Dictionary<string, object?>
            {
                ["nombre"] = comando.NombreGrupoEquipo,
                ["modelo"] = comando.Modelo,
                ["marca"] = comando.Marca,
                ["codigoUcb"] = comando.CodigoUcb
            };
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "crear", "equipo", parametros);
        }
    }

    private void ValidarEntradaCreacion(CrearEquipoComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));

        if (string.IsNullOrWhiteSpace(comando.NombreGrupoEquipo))
            throw new ErrorNombreRequerido("nombre del grupo de equipo");

        if (string.IsNullOrWhiteSpace(comando.Modelo))
            throw new ErrorNombreRequerido("modelo");

        if (string.IsNullOrWhiteSpace(comando.Marca))
            throw new ErrorNombreRequerido("marca");

        if (comando.CostoReferencia.HasValue && comando.CostoReferencia < 0)
            throw new ErrorValorNegativo("costo de referencia", comando.CostoReferencia.Value);

        if (comando.TiempoMaximoPrestamo.HasValue && comando.TiempoMaximoPrestamo <= 0)
            throw new ErrorIdInvalido("tiempo máximo de préstamo");
    }    public void ActualizarEquipo(ActualizarEquipoComando comando)
    {
        try
        {
            ValidarEntradaActualizacion(comando);
            _equipoRepository.Actualizar(comando);
        }
        catch (ErrorIdInvalido)
        {
            throw;
        }
        catch (ErrorValorNegativo)
        {
            throw;
        }
        catch (Exception ex)
        {
            var parametros = new Dictionary<string, object?>
            {
                ["id"] = comando.Id,
                ["nombre"] = comando.NombreGrupoEquipo
            };
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "actualizar", "equipo", parametros);
        }
    }

    private void ValidarEntradaActualizacion(ActualizarEquipoComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));

        if (comando.Id <= 0)
            throw new ErrorIdInvalido("ID del equipo");

        if (comando.CostoReferencia.HasValue && comando.CostoReferencia < 0)
            throw new ErrorValorNegativo("costo de referencia", comando.CostoReferencia.Value);

        if (comando.TiempoMaximoPrestamo.HasValue && comando.TiempoMaximoPrestamo <= 0)
            throw new ErrorIdInvalido("tiempo máximo de préstamo");
    }    public void EliminarEquipo(EliminarEquipoComando comando)
    {
        try
        {
            ValidarEntradaEliminacion(comando);
            _equipoRepository.Eliminar(comando.Id);
        }
        catch (ErrorIdInvalido)
        {
            throw;
        }
        catch (Exception ex)
        {
            var parametros = new Dictionary<string, object?>
            {
                ["id"] = comando.Id
            };
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "eliminar", "equipo", parametros);
        }
    }

    private void ValidarEntradaEliminacion(EliminarEquipoComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));

        if (comando.Id <= 0)
            throw new ErrorIdInvalido("ID del equipo");
    }
    public List<EquipoDto>? ObtenerTodosEquipos()
    {
        try
        {
            DataTable resultado = _equipoRepository.ObtenerTodos();
            var lista = new List<EquipoDto>(resultado.Rows.Count);
            foreach (DataRow fila in resultado.Rows)
            {
                lista.Add(MapearFilaADto(fila));
            }
            return lista;
        }
        catch
        {
            throw;
        }
    }
    private static EquipoDto MapearFilaADto(DataRow fila)
    {
        return new EquipoDto
        {
            Id = Convert.ToInt32(fila["id_equipo"]),
            NombreGrupoEquipo = fila["nombre_grupo_equipo"] == DBNull.Value ? null : fila["nombre_grupo_equipo"].ToString(),
            CodigoImt = fila["codigo_imt_equipo"] == DBNull.Value ? null : Convert.ToInt32(fila["codigo_imt_equipo"]),
            CodigoUcb = fila["codigo_ucb_equipo"] == DBNull.Value ? null : fila["codigo_ucb_equipo"].ToString(),
            Descripcion = fila["descripcion_equipo"] == DBNull.Value ? null : fila["descripcion_equipo"].ToString(),
            NumeroSerial = fila["numero_serial_equipo"] == DBNull.Value ? null : fila["numero_serial_equipo"].ToString(),
            Ubicacion = fila["ubicacion_equipo"] == DBNull.Value ? null : fila["ubicacion_equipo"].ToString(),
            Procedencia = fila["procedencia_equipo"] == DBNull.Value ? null : fila["procedencia_equipo"].ToString(),
            TiempoMaximoPrestamo = fila["tiempo_max_prestamo_equipo"] == DBNull.Value ? null : Convert.ToInt32(fila["tiempo_max_prestamo_equipo"]),
            NombreGavetero = fila["nombre_gavetero_equipo"] == DBNull.Value ? null : fila["nombre_gavetero_equipo"].ToString(),
            EstadoEquipo = fila["estado_equipo_equipo"] == DBNull.Value ? null : fila["estado_equipo_equipo"].ToString(),
            CostoReferencia = fila["costo_referencia_equipo"] == DBNull.Value ? null : Convert.ToDouble(fila["costo_referencia_equipo"]),
        };
    }
}