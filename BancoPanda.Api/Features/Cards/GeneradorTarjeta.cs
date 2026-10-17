using System.Security.Cryptography;
using BancoPanda.Api.Entities;

namespace BancoPanda.Api.Features.Cards;

public static class GeneradorTarjeta
{
    // 16 dígitos: prefijo de la marca (Visa 4, Mastercard 51-55) + dígitos al azar + dígito verificador de Luhn.
    public static string Generar(MarcaTarjeta marca)
    {
        var digitos = new int[16];
        for (var i = 0; i < 15; i++) digitos[i] = RandomNumberGenerator.GetInt32(0, 10);
        digitos[0] = marca == MarcaTarjeta.Visa ? 4 : 5;
        if (marca == MarcaTarjeta.Mastercard) digitos[1] = RandomNumberGenerator.GetInt32(1, 6);
        digitos[15] = DigitoLuhn(digitos);
        return string.Concat(digitos);
    }

    private static int DigitoLuhn(int[] digitos)
    {
        var suma = 0;
        for (var i = 0; i < 15; i++)
        {
            var d = digitos[i];
            if (i % 2 == 0)
            {
                d *= 2;
                if (d > 9) d -= 9;
            }
            suma += d;
        }
        return (10 - suma % 10) % 10;
    }
}
