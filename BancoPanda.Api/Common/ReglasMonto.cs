namespace BancoPanda.Api.Common;

public static class ReglasMonto
{
    public const decimal Maximo = 5000.00m;

    public static string? Validar(decimal monto)
    {
        if (monto <= 0) return "El monto debe ser mayor a cero";
        if (decimal.Round(monto, 2) != monto) return "El monto solo puede tener hasta 2 decimales";
        if (monto > Maximo) return $"El monto máximo por operación es ${Maximo:0}";
        return null;
    }

    // Ecuador está en UTC-5 todo el año (sin horario de verano).
    public static DateTime HoraEcuador(DateTime utc) => utc.AddHours(-5);
}
