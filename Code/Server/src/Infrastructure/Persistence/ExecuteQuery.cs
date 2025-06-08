using Npgsql;
using System.Data;

public class ExecuteQuery : IExecuteQuery
{
    private readonly string _connectionString;

    public ExecuteQuery(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
                            ?? throw new InvalidOperationException(
                                "La cadena de conexión 'DefaultConnection' no está configurada.");
    }

    public DataTable EjecutarSp(string nombreSp, Dictionary<string, object> parametros)
    {
        NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        NpgsqlCommand cmd = new NpgsqlCommand(nombreSp, conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        AgregarParametros(cmd, parametros);

        NpgsqlDataReader reader = cmd.ExecuteReader();
        DataTable dt = new DataTable();
        dt.Load(reader);

        reader.Close();
        conn.Close();
        return dt;
    }

    public void EjecutarSpNR(string nombreSp, Dictionary<string, object> parametros)
    {
        NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        NpgsqlCommand cmd = new NpgsqlCommand(nombreSp, conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        AgregarParametros(cmd, parametros);

        cmd.ExecuteNonQuery();
        conn.Close();
    }

    public DataTable EjecutarFuncion(string consultaSql, Dictionary<string, object> parametros)
    {
        NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        NpgsqlCommand cmd = new NpgsqlCommand(consultaSql, conn)
        {
            CommandType = CommandType.Text
        };
        AgregarParametros(cmd, parametros);

        NpgsqlDataReader reader = cmd.ExecuteReader();
        DataTable dt = new DataTable();
        dt.Load(reader);

        reader.Close();
        conn.Close();
        return dt;
    }

    public List<Dictionary<string, object>> EjecutarFuncionDic(string consultaSql, Dictionary<string, object> parametros)
    {
        DataTable dt = EjecutarFuncion(consultaSql, parametros);
        List<Dictionary<string, object>> lista = new List<Dictionary<string, object>>();

        foreach (DataRow fila in dt.Rows)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            foreach (DataColumn columna in dt.Columns)
            {
                dic[columna.ColumnName] = fila[columna] != DBNull.Value ? fila[columna] : null;
            }

            lista.Add(dic);
        }

        return lista;
    }

    private void AgregarParametros(NpgsqlCommand cmd, Dictionary<string, object> parametros)
    {
        if (parametros == null)
        {
            return;
        }

        foreach (KeyValuePair<string, object> param in parametros)
        {
            cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
        }
    }

    public bool ProbarConexion()
    {
        try
        {
            NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            Console.WriteLine("Conexi�n exitosa");
            conn.Close();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error de conexi�n: {ex.Message}");
            return false;
        }
    }
}
