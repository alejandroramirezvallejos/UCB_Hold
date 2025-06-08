using System.Data;
public class CarreraRepository : ICarreraRepository
{
        private readonly IExecuteQuery _ejecutarConsulta;

        public CarreraRepository(IExecuteQuery ejecutarConsulta)
        {
            _ejecutarConsulta = ejecutarConsulta;
        }

        public void Crear(CrearCarreraComando comando)
        {
            const string sql = @"
            CALL public.insertar_carrera(
	        @nombre
            )";

            var parametros = new Dictionary<string, object?>
            {
                ["nombre"] = comando.Nombre
            };
            try
            {
                _ejecutarConsulta.EjecutarSpNR(sql, parametros);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear la carrera", ex);
            }
        }

        public void Eliminar(int id)
        {
            const string sql = @"
                CALL public.eliminar_carrera(
	            @id
            )";
            try
            {
                _ejecutarConsulta.EjecutarSpNR(sql, new Dictionary<string, object?>
                {
                    ["id"] = id
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar la carrera", ex);
            }
        }

        public void Actualizar(ActualizarCarreraComando comando)
        {
        const string sql = @"
                CALL public.actualizar_carrera(
                    @id,
                    @nombre
                )";

            var parametros = new Dictionary<string, object?>
            {
                ["id"] = comando.Id,
                ["nombre"] = comando.Nombre ?? (object)DBNull.Value
            };
            try
            {
                _ejecutarConsulta.EjecutarSpNR(sql, parametros);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar la carrera", ex);
            }
        }
        public List<CarreraDto> ObtenerTodas()
        {
            const string sql = @"
                SELECT * from public.obtener_carreras()
            ";
        try
        {
            DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
            var lista = new List<CarreraDto>(dt.Rows.Count);
            foreach (DataRow fila in dt.Rows)
            {
                lista.Add(new CarreraDto
                {
                    Nombre = fila["nombre"] == DBNull.Value ? null : fila["nombre"].ToString()
                });
            }
            return lista;
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener las carreras", ex);
        }
    }
}