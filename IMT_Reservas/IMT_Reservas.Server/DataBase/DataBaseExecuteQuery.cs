using Npgsql;
using System.Data;

public class DataBaseExecuteQuery
{
    private readonly string _connectionString;

    public DataBaseExecuteQuery(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<DataTable> EjecutarStoredProcedureConRetorno(string nombreSp, Dictionary<string, object> parametros)
    {
        ///Para selects
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand(nombreSp, conn)
        {
            CommandType = CommandType.StoredProcedure 
        };

        if (parametros != null)
        {
            foreach (var param in parametros)
            {
                cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);//util para poner null si esta vacio
            }
        }

        using var reader = await cmd.ExecuteReaderAsync();
        var dt = new DataTable();
        dt.Load(reader);
        return dt;
    }

    
    public async Task EjecutarStoredProcedureSinRetorno(string nombreSp, Dictionary<string, object> parametros)
    {
        /// Para INSERT, UPDATE, DELETE
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand(nombreSp, conn)
        {
            CommandType = CommandType.StoredProcedure 
        };

        foreach (var param in parametros)
        {
            cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
        }

        await cmd.ExecuteNonQueryAsync();
    }
    public async Task<DataTable> EjecutarFuncionConRetorno(string consultaSql, Dictionary<string, object> parametros)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand(consultaSql, conn)
        {
            CommandType = CommandType.Text
        };

        if (parametros != null)
        {
            foreach (var param in parametros)
            {
                cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
            }
        }

        using var reader = await cmd.ExecuteReaderAsync();
        var dt = new DataTable();
        dt.Load(reader);
        return dt;
    }

}