namespace BancoPanda.Api.Features.Beneficiaries;

public record BeneficiarioResponse(
    Guid BeneficiarioId,
    Guid BancoId,
    string Banco,
    string NumeroCuenta,
    string Cedula,
    string? Nombre);
