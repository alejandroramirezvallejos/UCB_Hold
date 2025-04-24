using Npgsql;
using System.Data;
using Microsoft.Extensions.Configuration;

public class DataBaseExecuteQuery
{
    private readonly string _connectionString;

    public DataBaseExecuteQuery(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
                            ?? throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no está configurada.");
    }

    public async Task<DataTable> EjecutarStoredProcedureConRetorno(string nombreSp, Dictionary<string, object> parametros)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand(nombreSp, conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        AgregarParametros(cmd, parametros);

        using var reader = await cmd.ExecuteReaderAsync();
        var dt = new DataTable();
        dt.Load(reader);

        return dt;
    }

    public async Task EjecutarStoredProcedureSinRetorno(string nombreSp, Dictionary<string, object> parametros)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand(nombreSp, conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        AgregarParametros(cmd, parametros);

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

        AgregarParametros(cmd, parametros);

        using var reader = await cmd.ExecuteReaderAsync();
        var dt = new DataTable();
        dt.Load(reader);

        return dt;
    }

    private void AgregarParametros(NpgsqlCommand cmd, Dictionary<string, object>? parametros)
    {
        if (parametros == null) return;

        foreach (var param in parametros)
        {
            cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
        }
    }

    //INFO: Metodo temporal para pruebas, eliminar despues
    public async Task<bool> ProbarConexion()
    {
        try
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            Console.WriteLine("Conexión exitosa");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error de conexión: {ex.Message}");
            return false;
        }
    }
}

