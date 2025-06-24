using System.Data;

public interface IExecuteQuery
{
    void EjecutarSpNR(string nombreSp, Dictionary<string, object?> parametros);
    DataTable EjecutarFuncion(string consultaSql, Dictionary<string, object?> parametros);
}
