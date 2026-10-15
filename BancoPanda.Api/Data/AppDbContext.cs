using BancoPanda.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace BancoPanda.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Cuenta> Cuentas => Set<Cuenta>();
    public DbSet<Tarjeta> Tarjetas => Set<Tarjeta>();
    public DbSet<Movimiento> Movimientos => Set<Movimiento>();

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
            e.Property(t => t.UltimosDigitos).HasMaxLength(4).IsFixedLength().IsRequired();
            e.Property(t => t.Marca).HasConversion<string>().HasMaxLength(20);
            e.Property(t => t.Tipo).HasConversion<string>().HasMaxLength(20);

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
    }
}