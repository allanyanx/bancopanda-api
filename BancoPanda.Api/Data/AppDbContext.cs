using BancoPanda.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace BancoPanda.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Cuenta> Cuentas => Set<Cuenta>();
    public DbSet<Tarjeta> Tarjetas => Set<Tarjeta>();
    public DbSet<Movimiento> Movimientos => Set<Movimiento>();
    public DbSet<Banco> Bancos => Set<Banco>();
    public DbSet<Beneficiario> Beneficiarios => Set<Beneficiario>();
    public DbSet<Transferencia> Transferencias => Set<Transferencia>();
    public DbSet<EfectivoMovil> EfectivosMoviles => Set<EfectivoMovil>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(e =>
        {
            e.Property(u => u.NombreUsuario).HasMaxLength(20).IsRequired();
            e.Property(u => u.Nombres).HasMaxLength(60).IsRequired();
            e.Property(u => u.Apellidos).HasMaxLength(60).IsRequired();
            e.Property(u => u.Cedula).HasMaxLength(10).IsRequired();
            e.Property(u => u.Correo).HasMaxLength(150).IsRequired();
            e.Property(u => u.Telefono).HasMaxLength(15);
            e.Property(u => u.ContrasenaHash).IsRequired();

            e.HasIndex(u => u.NombreUsuario).IsUnique();
            e.HasIndex(u => u.Correo).IsUnique();
            e.HasIndex(u => u.Cedula).IsUnique();
        });

        modelBuilder.Entity<Cuenta>(e =>
        {
            e.Property(c => c.NumeroCuenta).HasMaxLength(20).IsRequired();
            e.Property(c => c.Tipo).HasConversion<string>().HasMaxLength(20);
            e.Property(c => c.Saldo).HasPrecision(18, 2);
            e.Property(c => c.Moneda).HasMaxLength(3);

            e.HasIndex(c => c.NumeroCuenta).IsUnique();

            e.HasOne(c => c.Usuario)
             .WithMany(u => u.Cuentas)
             .HasForeignKey(c => c.UsuarioId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Tarjeta>(e =>
        {
            e.Property(t => t.NumeroTarjeta).HasMaxLength(16).IsFixedLength().IsRequired();
            e.Property(t => t.Marca).HasConversion<string>().HasMaxLength(20);
            e.Property(t => t.Tipo).HasConversion<string>().HasMaxLength(20);

            e.HasIndex(t => t.NumeroTarjeta).IsUnique();

            e.HasOne(t => t.Cuenta)
             .WithMany(c => c.Tarjetas)
             .HasForeignKey(t => t.CuentaId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Movimiento>(e =>
        {
            e.Property(m => m.Descripcion).HasMaxLength(150).IsRequired();
            e.Property(m => m.Monto).HasPrecision(18, 2);
            e.Property(m => m.Tipo).HasConversion<string>().HasMaxLength(10);
            e.Property(m => m.Categoria).HasConversion<string>().HasMaxLength(20);
            e.HasIndex(m => new { m.CuentaId, m.Fecha });
            e.HasOne(m => m.Cuenta)
             .WithMany(c => c.Movimientos)
             .HasForeignKey(m => m.CuentaId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Banco>(e =>
        {
            e.Property(b => b.Nombre).HasMaxLength(60).IsRequired();
            e.HasIndex(b => b.Nombre).IsUnique();

            e.HasData(
                new Banco { BancoId = Banco.BancoPandaId, Nombre = "Banco Panda" },
                new Banco { BancoId = new Guid("b0000000-0000-4000-8000-000000000002"), Nombre = "Banco Guayaquil" },
                new Banco { BancoId = new Guid("b0000000-0000-4000-8000-000000000003"), Nombre = "Banco Pichincha" },
                new Banco { BancoId = new Guid("b0000000-0000-4000-8000-000000000004"), Nombre = "Banco Bolivariano" },
                new Banco { BancoId = new Guid("b0000000-0000-4000-8000-000000000005"), Nombre = "Produbanco" },
                new Banco { BancoId = new Guid("b0000000-0000-4000-8000-000000000006"), Nombre = "Banco del Pacífico" },
                new Banco { BancoId = new Guid("b0000000-0000-4000-8000-000000000007"), Nombre = "Banco Amazonas" },
                new Banco { BancoId = new Guid("b0000000-0000-4000-8000-000000000008"), Nombre = "Banco Internacional" },
                new Banco { BancoId = new Guid("b0000000-0000-4000-8000-000000000009"), Nombre = "Banco del Austro" });
        });

        modelBuilder.Entity<Beneficiario>(e =>
        {
            e.Property(b => b.NumeroCuenta).HasMaxLength(20).IsRequired();
            e.Property(b => b.Cedula).HasMaxLength(10).IsRequired();
            e.Property(b => b.Nombre).HasMaxLength(120);

            e.HasIndex(b => new { b.UsuarioId, b.BancoId, b.NumeroCuenta }).IsUnique();

            e.HasOne(b => b.Usuario)
             .WithMany()
             .HasForeignKey(b => b.UsuarioId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(b => b.Banco)
             .WithMany()
             .HasForeignKey(b => b.BancoId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Transferencia>(e =>
        {
            e.Property(t => t.Referencia).HasMaxLength(20).IsRequired();
            e.Property(t => t.Monto).HasPrecision(18, 2);

            e.HasIndex(t => t.Referencia).IsUnique();
            e.HasIndex(t => t.SolicitudId).IsUnique();

            e.HasOne(t => t.CuentaOrigen)
             .WithMany(c => c.Transferencias)
             .HasForeignKey(t => t.CuentaOrigenId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(t => t.Beneficiario)
             .WithMany()
             .HasForeignKey(t => t.BeneficiarioId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(t => t.Movimiento)
             .WithOne()
             .HasForeignKey<Transferencia>(t => t.MovimientoId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EfectivoMovil>(e =>
        {
            e.Property(m => m.TelefonoDestino).HasMaxLength(15).IsRequired();
            e.Property(m => m.Monto).HasPrecision(18, 2);
            e.Property(m => m.CodigoHash).IsRequired();
            e.Property(m => m.Estado).HasConversion<string>().HasMaxLength(15);

            e.HasIndex(m => m.SolicitudId).IsUnique();

            e.HasOne(m => m.Cuenta)
             .WithMany(c => c.EfectivosMoviles)
             .HasForeignKey(m => m.CuentaId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(m => m.Movimiento)
             .WithOne()
             .HasForeignKey<EfectivoMovil>(m => m.MovimientoId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}