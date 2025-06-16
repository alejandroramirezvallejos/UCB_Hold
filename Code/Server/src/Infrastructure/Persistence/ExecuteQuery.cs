using Npgsql;
using NpgsqlTypes;
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

    public DataTable EjecutarSp(string nombreSp, Dictionary<string, object?> parametros)
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
    }    public void EjecutarSpNR(string nombreSp, Dictionary<string, object?> parametros)
    {
        try
        {
            NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            // EjecutarSpNR siempre maneja declaraciones CALL, que en PostgreSQL son Insert, Update o Delete 
            NpgsqlCommand cmd = new NpgsqlCommand(nombreSp, conn)
            {
                CommandType = CommandType.Text
            };
            AgregarParametros(cmd, parametros);

            cmd.ExecuteNonQuery();
            conn.Close();
        }
        catch (Exception ex)
        {
            
            var parametrosStr = parametros != null 
                ? string.Join(", ", parametros.Select(p => $"{p.Key}={p.Value}"))
                : "null";
            throw new Exception($"Error ejecutando comando: {nombreSp}\nParámetros: {parametrosStr}\nError original: {ex.Message}", ex);
        }
    }

    public DataTable EjecutarFuncion(string consultaSql, Dictionary<string, object?> parametros)
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

    public List<Dictionary<string, object?>> EjecutarFuncionDic(string consultaSql, Dictionary<string, object?> parametros)
    {
        DataTable dt = EjecutarFuncion(consultaSql, parametros);
        List<Dictionary<string, object?>> lista = new List<Dictionary<string, object?>>();

        foreach (DataRow fila in dt.Rows)
        {
            Dictionary<string, object?> dic = new Dictionary<string, object?>();
            foreach (DataColumn columna in dt.Columns)
            {
                dic[columna.ColumnName] = fila[columna] != DBNull.Value ? fila[columna] : null;
            }

            lista.Add(dic);
        }

        return lista;
    }
    private void AgregarParametros(NpgsqlCommand cmd, Dictionary<string, object?> parametros)
    {
        if (parametros == null)
        {
            return;
        }

        foreach (KeyValuePair<string, object?> param in parametros)
        {
            // Manejar diferentes tipos de arrays y listas para PostgreSQL
            if (param.Value != null && EsArrayOLista(param.Value))
            {
                var npgsqlParam = CrearParametroArray(param.Key, param.Value);
                cmd.Parameters.Add(npgsqlParam);
            }
            else
            {
                cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
            }
        }
    }

    private bool EsArrayOLista(object value)
    {
        var type = value.GetType();
        
        if (type.IsArray)
            return true;
            
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            return true;
            
        if (value is not string && type.GetInterfaces()
            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
            return true;
            
        return false;
    }
    private NpgsqlParameter CrearParametroArray(string nombreParametro, object arrayValue)
    {
        var processedValue = ConvertirAArray(arrayValue);

        return processedValue switch
        {
            int[] intArray => new NpgsqlParameter(nombreParametro, NpgsqlDbType.Array | NpgsqlDbType.Integer)
            {
                Value = intArray
            },
            long[] longArray => new NpgsqlParameter(nombreParametro, NpgsqlDbType.Array | NpgsqlDbType.Bigint)
            {
                Value = longArray
            },
            string[] stringArray => new NpgsqlParameter(nombreParametro, NpgsqlDbType.Array | NpgsqlDbType.Text)
            {
                Value = stringArray
            },
            double[] doubleArray => new NpgsqlParameter(nombreParametro, NpgsqlDbType.Array | NpgsqlDbType.Double)
            {
                Value = doubleArray
            },
            decimal[] decimalArray => new NpgsqlParameter(nombreParametro, NpgsqlDbType.Array | NpgsqlDbType.Numeric)
            {
                Value = decimalArray
            },
            bool[] boolArray => new NpgsqlParameter(nombreParametro, NpgsqlDbType.Array | NpgsqlDbType.Boolean)
            {
                Value = boolArray
            },
            DateTime[] dateTimeArray => new NpgsqlParameter(nombreParametro, NpgsqlDbType.Array | NpgsqlDbType.Timestamp)
            {
                Value = dateTimeArray
            },
            Guid[] guidArray => new NpgsqlParameter(nombreParametro, NpgsqlDbType.Array | NpgsqlDbType.Uuid)
            {
                Value = guidArray
            },
            byte[][] byteArrayArray => new NpgsqlParameter(nombreParametro, NpgsqlDbType.Array | NpgsqlDbType.Bytea)
            {
                Value = byteArrayArray
            },
            short[] shortArray => new NpgsqlParameter(nombreParametro, NpgsqlDbType.Array | NpgsqlDbType.Smallint)
            {
                Value = shortArray
            },
            float[] floatArray => new NpgsqlParameter(nombreParametro, NpgsqlDbType.Array | NpgsqlDbType.Real)
            {
                Value = floatArray
            },
            _ => new NpgsqlParameter(nombreParametro, NpgsqlDbType.Array | NpgsqlDbType.Unknown)
            {
                Value = processedValue
            }
        };
    }

    private object ConvertirAArray(object value)
    {
        var type = value.GetType();
        
        // Si ya es array, retornarlo tal como está
        if (type.IsArray)
            return value;
            
        // Si es List<T>, convertir a array
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
        {
            var elementType = type.GetGenericArguments()[0];
            var toArrayMethod = typeof(Enumerable).GetMethod("ToArray")?.MakeGenericMethod(elementType);
            return toArrayMethod?.Invoke(null, new[] { value }) ?? value;
        }
        
        // Si es IEnumerable<T>, intentar convertir a array
        var enumerableInterface = type.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            
        if (enumerableInterface != null)
        {
            var elementType = enumerableInterface.GetGenericArguments()[0];
            var toArrayMethod = typeof(Enumerable).GetMethod("ToArray")?.MakeGenericMethod(elementType);
            return toArrayMethod?.Invoke(null, new[] { value }) ?? value;
        }
        
        return value;
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
