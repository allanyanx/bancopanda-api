namespace BancoPanda.Api.Entities;

public enum TipoCuenta
{
    Ahorros,
    Corriente
}

public enum TipoMovimiento
{
    Ingreso,
    Egreso
}

public enum CategoriaMovimiento
{
    Sueldo,
    Transferencia,
    Comida,
    Transporte,
    Compras,
    Servicios,
    EfectivoMovil,
    Otros
}

public enum EstadoEfectivoMovil
{
    Pendiente,
    Cobrado,
    Expirado
}

public enum MarcaTarjeta
{
    Visa,
    Mastercard
}

public enum TipoTarjeta
{
    Debito,
    Credito
}