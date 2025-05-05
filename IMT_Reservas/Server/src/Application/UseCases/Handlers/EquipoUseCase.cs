using System;
using System.Collections.Generic;
using System.Data;

public class EquipoUseCase : IObtenerEquipoConsulta, ICrearEquipoComando, 
                             IActualizarEquipoComando, IEliminarEquipoComando
{
    private readonly IExecuteQuery _ejecutarConsulta;

    public EquipoUseCase(IExecuteQuery ejecutarConsulta)
    {
        _ejecutarConsulta = ejecutarConsulta;
    }

    public EquipoDto Handle(CrearEquipoComando comando)
    {
        const string sql = @"
            INSERT INTO public.equipos
              (id_grupo_equipo, codigo_imt, codigo_ucb, descripcion, estado_equipo,
               numero_serial, ubicacion, costo_referencia, tiempo_max_prestamo,
               procedencia, id_gavetero, estado_eliminado, fecha_ingreso_equipo)
            VALUES
              (@grupoEquipoId, @codigoImt, @codigoUcb, @descripcion, @estadoEquipo,
               @numeroSerial, @ubicacion, @costoReferencia, @tiempoMaximoPrestamo,
               @procedencia, @gaveteroId, false, NOW())
            RETURNING *;
        ";

        DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>
        {
            ["grupoEquipoId"]        = comando.GrupoEquipoId,
            ["codigoImt"]            = comando.CodigoImt,
            ["codigoUcb"]            = comando.CodigoUcb,
            ["descripcion"]          = comando.Descripcion,
            ["estadoEquipo"]         = comando.EstadoEquipo,
            ["numeroSerial"]         = comando.NumeroSerial,
            ["ubicacion"]            = comando.Ubicacion,
            ["costoReferencia"]      = comando.CostoReferencia,
            ["tiempoMaximoPrestamo"] = comando.TiempoMaximoPrestamo,
            ["procedencia"]          = comando.Procedencia,
            ["gaveteroId"]           = comando.GaveteroId
        });

        return MapearFilaADto(dt.Rows[0]);
    }
    public EquipoDto? Handle(ActualizarEquipoComando comando)
    {
        const string sql = @"
            UPDATE public.equipos
            SET
                id_grupo_equipo      = @grupoEquipoId,
                codigo_imt           = @codigoImt,
                codigo_ucb           = @codigoUcb,
                descripcion          = @descripcion,
                estado_equipo        = @estadoEquipo,
                numero_serial        = @numeroSerial,
                ubicacion            = @ubicacion,
                costo_referencia     = @costoReferencia,
                tiempo_max_prestamo  = @tiempoMaximoPrestamo,
                procedencia          = @procedencia,
                id_gavetero          = @gaveteroId
            WHERE id_equipo = @id;
            SELECT * FROM public.equipos WHERE id_equipo = @id;
        ";

        DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>
        {
            ["id"]                   = comando.Id,
            ["grupoEquipoId"]        = comando.GrupoEquipoId,
            ["codigoImt"]            = comando.CodigoImt,
            ["codigoUcb"]            = comando.CodigoUcb,
            ["descripcion"]          = comando.Descripcion,
            ["estadoEquipo"]         = comando.EstadoEquipo,
            ["numeroSerial"]         = comando.NumeroSerial,
            ["ubicacion"]            = comando.Ubicacion,
            ["costoReferencia"]      = comando.CostoReferencia,
            ["tiempoMaximoPrestamo"] = comando.TiempoMaximoPrestamo,
            ["procedencia"]          = comando.Procedencia,
            ["gaveteroId"]           = comando.GaveteroId
        });

        if (dt.Rows.Count == 0)
            return null;

        return MapearFilaADto(dt.Rows[0]);
    }

    public bool Handle(EliminarEquipoComando comando)
    {
        const string sql = @"
            UPDATE public.equipos
            SET estado_eliminado = true
            WHERE id_equipo = @id;
        ";

        _ejecutarConsulta.EjecutarSpNR(sql, new Dictionary<string, object?>
        {
            ["id"] = comando.Id
        });

        return true;
    }

    public EquipoDto? Handle(ObtenerEquipoConsulta consulta)
    {
        const string sql = @"
            SELECT
                id_equipo,
                id_grupo_equipo,
                codigo_imt,
                codigo_ucb,
                descripcion,
                estado_equipo,
                numero_serial,
                ubicacion,
                costo_referencia,
                tiempo_max_prestamo,
                procedencia,
                id_gavetero,
                estado_eliminado,
                fecha_ingreso_equipo
            FROM public.equipos
            WHERE id_equipo = @id_equipo_input;
        ";

        DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object>
        {
            ["id_equipo_input"] = consulta.Id
        });

        if (dt.Rows.Count == 0)
            return null;

        return MapearFilaADto(dt.Rows[0]);
    }

    private static EquipoDto MapearFilaADto(DataRow fila)
    {
        return new EquipoDto
        {
            Id                   = Convert.ToInt32(fila["id_equipo"]),
            GrupoEquipoId        = Convert.ToInt32(fila["id_grupo_equipo"]),
            CodigoImt            = fila["codigo_imt"]?.ToString() ?? string.Empty,
            CodigoUcb            = fila["codigo_ucb"] as string,
            Descripcion          = fila["descripcion"] as string,
            EstadoEquipo         = fila["estado_equipo"]?.ToString() ?? string.Empty,
            NumeroSerial         = fila["numero_serial"] as string,
            Ubicacion            = fila["ubicacion"] as string,
            CostoReferencia      = fila["costo_referencia"] == DBNull.Value 
                                   ? null 
                                   : Convert.ToDouble(fila["costo_referencia"]),
            TiempoMaximoPrestamo = fila["tiempo_max_prestamo"] == DBNull.Value 
                                   ? null 
                                   : Convert.ToInt32(fila["tiempo_max_prestamo"]),
            Procedencia          = fila["procedencia"] as string,
            GaveteroId           = fila["id_gavetero"] == DBNull.Value 
                                   ? null 
                                   : Convert.ToInt32(fila["id_gavetero"]),
            EstadoDisponibilidad = fila["estado_equipo"]?.ToString() ?? string.Empty,
            EstaEliminado        = Convert.ToBoolean(fila["estado_eliminado"]),
            FechaDeIngreso       = DateOnly.FromDateTime(Convert.ToDateTime(fila["fecha_ingreso_equipo"]))
        };
    }
}
