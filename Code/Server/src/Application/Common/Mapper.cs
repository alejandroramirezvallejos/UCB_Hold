namespace IMT_Reservas.Server.Application.Common;
using System.Reflection;

public static class Mapper
{
    public static Dictionary<string, object?> ToParameters<TEntity>(TEntity entity) where TEntity : class
    {
        var parameters = new Dictionary<string, object?>();
        var properties = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            var value = property.GetValue(entity);
            var parameterName = ToSnakeCase(property.Name);
            parameters[parameterName] = value ?? DBNull.Value;
        }

        return parameters;
    }

    private static string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input)) 
            return input;

        var result = new System.Text.StringBuilder();
        
        for (int i = 0; i < input.Length; i++)
        {
            if (char.IsUpper(input[i]) && i > 0)
                result.Append('_');
            
            result.Append(char.ToLower(input[i]));
        }

        return result.ToString();
    }
}
