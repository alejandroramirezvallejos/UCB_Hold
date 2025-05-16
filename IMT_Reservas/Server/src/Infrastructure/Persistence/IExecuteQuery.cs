using System.Data;

public interface IExecuteQuery
{
    DataTable EjecutarSp(string nombreSp, Dictionary<string, object> parametros);
    void EjecutarSpNR(string nombreSp, Dictionary<string, object> parametros);
    DataTable EjecutarFuncion(string consultaSql, Dictionary<string, object> parametros);
    List<Dictionary<string, object>> EjecutarFuncionDic(string consultaSql, Dictionary<string, object> parametros);
    public bool ProbarConexion();
}
