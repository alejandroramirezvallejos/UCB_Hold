public static class Verificar
{
    public static T SiEsNulo<T>(T valor, string nombre)
    {
        if (valor == null)
            throw new ArgumentNullException($"{nombre} no puede ser nulo");
        return valor;
    }

    public static string SiEsVacio(string valor, string nombre)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException($"{nombre} no puede estar vacio");
        return valor.Trim();
    }

    public static int SiEsNatural(int valor, string nombre)
    {
        if (valor <= 0)
            throw new ArgumentException($"{nombre} debe ser un numero natural positivo: '{valor}'");
        return valor;
    }

    public static double SiEsPositivo(double valor, string nombre)
    {
        if (valor < 0)
            throw new ArgumentException($"{nombre} debe ser un numero positivo: '{valor}'");
        return valor;
    }

    public static TEnum SiEstaEnEnum<TEnum>(string valor, string nombre)
           where TEnum : struct, Enum
    {
        string texto = SiEsVacio(valor, nombre);

        if (!Enum.TryParse<TEnum>(texto, ignoreCase: true, out TEnum resultado))
            throw new ArgumentException($"{nombre} ingresado es invalido: '{valor}'");
        return resultado;
    }

    public static DateOnly SiNoEsFutura(DateOnly valor, string nombre)
    {
        DateOnly hoy = DateOnly.FromDateTime(DateTime.Now);
        if (valor > hoy)
            throw new ArgumentException($"{nombre} no puede ser una fecha futura: '{valor}'");
        return valor;
    }

    public static DateOnly SiNoEsPosteriorA(DateOnly valor, DateOnly valorPosterior, string nombre, string nombrePosterior)
    {
        if (valor > valorPosterior)
            throw new ArgumentException($"{nombre} no puede ser posterior a {nombrePosterior}: '{valor}, {valorPosterior}'");
        return valor;
    }

    public static DateOnly SiNoEsPasada(DateOnly valor, string nombre)
    {
        DateOnly hoy = DateOnly.FromDateTime(DateTime.Now);
        if (valor < hoy)
            throw new ArgumentException($"{nombre} no puede ser una fecha pasada: '{valor}'");
        return valor;
    }

    public static DateOnly SiNoEsAnteriorA(DateOnly valor, DateOnly valorAnterior, string nombre, string nombreAnterior)
    {
        if (valor < valorAnterior)
            throw new ArgumentException($"{nombre} no puede ser anterior a {nombreAnterior}: '{valor}, {valorAnterior}'");
        return valor;
    }

    public static DateTime SiNoEsFutura(DateTime valor, string nombre)
    {
        DateTime ahora = DateTime.Now;
        if (valor > ahora)
            throw new ArgumentException($"{nombre} no puede ser una fecha futura: '{valor}'");
        return valor;
    }

    public static DateTime SiNoEsPasada(DateTime valor, string nombre)
    {
        DateTime ahora = DateTime.Now;
        if (valor < ahora)
            throw new ArgumentException($"{nombre} no puede ser una fecha pasada: '{valor}'");
        return valor;
    }

    public static DateTime SiNoEsAnteriorA(DateTime valor, DateTime valorAnterior, string nombre, string nombreAnterior)
    {
        if (valor < valorAnterior)
            throw new ArgumentException($"{nombre} no puede ser anterior a {nombreAnterior}: '{valor}, {valorAnterior}'");
        return valor;
    }

    public static DateTime SiNoEsPosteriorA(DateTime valor, DateTime valorPosterior, string nombre, string nombrePosterior)
    {
        if (valor > valorPosterior)
            throw new ArgumentException($"{nombre} no puede ser posterior a {nombrePosterior}: '{valor}, {valorPosterior}'");
        return valor;
    }
}
