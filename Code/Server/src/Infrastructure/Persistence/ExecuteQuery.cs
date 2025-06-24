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

    public virtual void EjecutarSpNR(string nombreSp, Dictionary<string, object?> parametros)
    {
        try
        {
            NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            NpgsqlCommand cmd = new NpgsqlCommand(nombreSp, conn)
            {
                CommandType = CommandType.Text
            };
            AgregarParametros(cmd, parametros);

            cmd.ExecuteNonQuery();
            conn.Close();
        }
        catch (NpgsqlException ex)
        {
            var parametrosStr = parametros != null 
                ? string.Join(", ", parametros.Select(p => $"{p.Key}={p.Value}"))
                : "null";
            throw new NpgsqlException($"Error ejecutando comando: {nombreSp}\nParámetros: {parametrosStr}\nError original: {ex.Message}", ex);        }
    }

    public virtual DataTable EjecutarFuncion(string consultaSql, Dictionary<string, object?> parametros)
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

    private void AgregarParametros(NpgsqlCommand cmd, Dictionary<string, object?> parametros)
    {
        if (parametros == null)
        {
            return;
        }

        foreach (KeyValuePair<string, object?> param in parametros)
        {
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
            
        return false;
    }    private NpgsqlParameter CrearParametroArray(string nombreParametro, object arrayValue)
    {
        return arrayValue switch
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
                Value = arrayValue
            }
        };
    }
}