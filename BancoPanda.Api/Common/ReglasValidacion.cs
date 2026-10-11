namespace BancoPanda.Api.Common;

public static class ReglasValidacion
{
    public const string SoloLetras = @"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ ]+$";
    public const string Cedula = @"^\d{10}$";
    public const string Telefono = @"^09\d{8}$";
    public const string LetrasYNumeros = @"^(?=.*[A-Za-z])(?=.*\d).+$";
}
