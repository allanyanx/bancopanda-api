using System.ComponentModel.DataAnnotations;
using BancoPanda.Api.Common;

namespace BancoPanda.Api.Features.Transfers;

public class TransferenciaRequest
{
    // Lo genera la app una vez por intento: si llega repetido, se devuelve el mismo comprobante.
    [Required(ErrorMessage = "Falta el identificador de la solicitud")]
    public Guid? SolicitudId { get; set; }

    [Required(ErrorMessage = "Elige la cuenta de origen")]
    public Guid? CuentaOrigenId { get; set; }

    [Required(ErrorMessage = "Elige el banco")]
    public Guid? BancoId { get; set; }

    [Required(ErrorMessage = "Ingresa el número de cuenta")]
    [RegularExpression(@"^\d{6,20}$", ErrorMessage = "El número de cuenta debe tener entre 6 y 20 dígitos")]
    public string NumeroCuenta { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingresa la cédula del beneficiario")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "La cédula debe tener 10 dígitos")]
    public string Cedula { get; set; } = string.Empty;

    [StringLength(120, ErrorMessage = "El nombre no puede superar los 120 caracteres")]
    [RegularExpression(ReglasValidacion.SoloLetras, ErrorMessage = "El nombre solo puede tener letras")]
    public string? Nombre { get; set; }

    [Required(ErrorMessage = "Ingresa el monto")]
    public decimal? Monto { get; set; }
}

public record ComprobanteResponse(
    Guid TransferenciaId,
    string Referencia,
    DateTime Fecha,
    decimal Monto,
    string Moneda,
    string CuentaOrigen,
    string Banco,
    string CuentaDestino,
    string? Beneficiario,
    decimal SaldoDisponible);
