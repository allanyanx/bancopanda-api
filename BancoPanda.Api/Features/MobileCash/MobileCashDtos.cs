using System.ComponentModel.DataAnnotations;
using BancoPanda.Api.Common;
using BancoPanda.Api.Entities;

namespace BancoPanda.Api.Features.MobileCash;

public class EfectivoMovilRequest
{
    // Lo genera la app una vez por intento, para no descontar dos veces si se reintenta.
    [Required(ErrorMessage = "Falta el identificador de la solicitud")]
    public Guid? SolicitudId { get; set; }

    [Required(ErrorMessage = "Elige la cuenta")]
    public Guid? CuentaId { get; set; }

    // Opcional: si no llega, se usa el celular registrado del usuario.
    [RegularExpression(ReglasValidacion.Celular, ErrorMessage = ReglasValidacion.MensajeCelular)]
    public string? TelefonoDestino { get; set; }

    [Required(ErrorMessage = "Ingresa el monto")]
    public decimal? Monto { get; set; }
}

public record EfectivoMovilResponse(
    Guid EfectivoMovilId,
    string PrimerCodigo,
    string SegundoCodigo,
    decimal Monto,
    string Moneda,
    string TelefonoDestino,
    DateTime FechaCreacion,
    DateTime FechaExpiracion,
    EstadoEfectivoMovil Estado,
    string NumeroCuenta,
    decimal SaldoDisponible);
