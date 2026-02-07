using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SubastaAutos.Infraestructure.Models;

namespace SubastaAutos.Infraestructure.Data;

public partial class LibreriaContext : DbContext
{
    public LibreriaContext(DbContextOptions<LibreriaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Auto> Auto { get; set; }

    public virtual DbSet<AutoImagen> AutoImagen { get; set; }

    public virtual DbSet<Categoria> Categoria { get; set; }

    public virtual DbSet<CondicionAuto> CondicionAuto { get; set; }

    public virtual DbSet<EstadoAuto> EstadoAuto { get; set; }

    public virtual DbSet<EstadoPago> EstadoPago { get; set; }

    public virtual DbSet<EstadoSubasta> EstadoSubasta { get; set; }

    public virtual DbSet<Pago> Pago { get; set; }

    public virtual DbSet<Puja> Puja { get; set; }

    public virtual DbSet<ResultadoSubasta> ResultadoSubasta { get; set; }

    public virtual DbSet<RolUsuario> RolUsuario { get; set; }

    public virtual DbSet<Subasta> Subasta { get; set; }

    public virtual DbSet<Usuario> Usuario { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Auto>(entity =>
        {
            entity.HasKey(e => e.IdAuto).HasName("PK__Auto__2C8AA82C6B96CF35");

            entity.HasIndex(e => e.Vin, "UQ__Auto__C5DF234C485BF78F").IsUnique();

            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Marca)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Modelo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Vin)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("VIN");

            entity.HasOne(d => d.IdCondicionAutoNavigation).WithMany(p => p.Auto)
                .HasForeignKey(d => d.IdCondicionAuto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Auto_Condicion");

            entity.HasOne(d => d.IdEstadoAutoNavigation).WithMany(p => p.Auto)
                .HasForeignKey(d => d.IdEstadoAuto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Auto_Estado");

            entity.HasOne(d => d.IdVendedorNavigation).WithMany(p => p.Auto)
                .HasForeignKey(d => d.IdVendedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Auto_Vendedor");

            entity.HasMany(d => d.IdCategoria).WithMany(p => p.IdAuto)
                .UsingEntity<Dictionary<string, object>>(
                    "AutoCategoria",
                    r => r.HasOne<Categoria>().WithMany()
                        .HasForeignKey("IdCategoria")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_AC_Categoria"),
                    l => l.HasOne<Auto>().WithMany()
                        .HasForeignKey("IdAuto")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_AC_Auto"),
                    j =>
                    {
                        j.HasKey("IdAuto", "IdCategoria").HasName("PK__AutoCate__36B6AA8D38EE87B9");
                    });
        });

        modelBuilder.Entity<AutoImagen>(entity =>
        {
            entity.HasKey(e => e.IdImagen).HasName("PK__AutoImag__B42D8F2A59685E7A");

            entity.Property(e => e.EsPrincipal).HasDefaultValue(false);
            entity.Property(e => e.UrlImagen)
                .HasMaxLength(300)
                .IsUnicode(false);

            entity.HasOne(d => d.IdAutoNavigation).WithMany(p => p.AutoImagen)
                .HasForeignKey(d => d.IdAuto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Imagen_Auto");
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.IdCategoria).HasName("PK__Categori__A3C02A107601B1A9");

            entity.HasIndex(e => e.Nombre, "UQ__Categori__75E3EFCF5703D4FA").IsUnique();

            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CondicionAuto>(entity =>
        {
            entity.HasKey(e => e.IdCondicionAuto).HasName("PK__Condicio__95F21F923E92337D");

            entity.HasIndex(e => e.Nombre, "UQ__Condicio__75E3EFCFC16EA6E9").IsUnique();

            entity.Property(e => e.IdCondicionAuto).ValueGeneratedNever();
            entity.Property(e => e.Nombre)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<EstadoAuto>(entity =>
        {
            entity.HasKey(e => e.IdEstadoAuto).HasName("PK__EstadoAu__DBA193E5D045E8B9");

            entity.HasIndex(e => e.Nombre, "UQ__EstadoAu__75E3EFCFF8797A77").IsUnique();

            entity.Property(e => e.IdEstadoAuto).ValueGeneratedNever();
            entity.Property(e => e.Nombre)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<EstadoPago>(entity =>
        {
            entity.HasKey(e => e.IdEstadoPago).HasName("PK__EstadoPa__540F03E9EF3DD3D9");

            entity.HasIndex(e => e.Nombre, "UQ__EstadoPa__75E3EFCF6FB54231").IsUnique();

            entity.Property(e => e.IdEstadoPago).ValueGeneratedNever();
            entity.Property(e => e.Nombre)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<EstadoSubasta>(entity =>
        {
            entity.HasKey(e => e.IdEstadoSubasta).HasName("PK__EstadoSu__4F0A9413992C5838");

            entity.HasIndex(e => e.Nombre, "UQ__EstadoSu__75E3EFCF1D776306").IsUnique();

            entity.Property(e => e.IdEstadoSubasta).ValueGeneratedNever();
            entity.Property(e => e.Nombre)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => e.IdPago).HasName("PK__Pago__FC851A3A821AEAEA");

            entity.HasIndex(e => e.IdSubasta, "UQ__Pago__AA418F72151EF1B8").IsUnique();

            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Monto).HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.IdEstadoPagoNavigation).WithMany(p => p.Pago)
                .HasForeignKey(d => d.IdEstadoPago)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pago_Estado");

            entity.HasOne(d => d.IdSubastaNavigation).WithOne(p => p.Pago)
                .HasForeignKey<Pago>(d => d.IdSubasta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pago_Subasta");
        });

        modelBuilder.Entity<Puja>(entity =>
        {
            entity.HasKey(e => e.IdPuja).HasName("PK__Puja__F986B2D4BA7D8345");

            entity.Property(e => e.FechaHora).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Monto).HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.IdSubastaNavigation).WithMany(p => p.Puja)
                .HasForeignKey(d => d.IdSubasta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Puja_Subasta");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Puja)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Puja_Usuario");
        });

        modelBuilder.Entity<ResultadoSubasta>(entity =>
        {
            entity.HasKey(e => e.IdSubasta).HasName("PK__Resultad__AA418F73A3B43E7A");

            entity.Property(e => e.IdSubasta).ValueGeneratedNever();
            entity.Property(e => e.MontoFinal).HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.IdSubastaNavigation).WithOne(p => p.ResultadoSubasta)
                .HasForeignKey<ResultadoSubasta>(d => d.IdSubasta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Resultado_Subasta");

            entity.HasOne(d => d.IdUsuarioGanadorNavigation).WithMany(p => p.ResultadoSubasta)
                .HasForeignKey(d => d.IdUsuarioGanador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Resultado_Ganador");
        });

        modelBuilder.Entity<RolUsuario>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__RolUsuar__2A49584C92F9B950");

            entity.HasIndex(e => e.Nombre, "UQ__RolUsuar__75E3EFCF7DA15FB2").IsUnique();

            entity.Property(e => e.IdRol).ValueGeneratedNever();
            entity.Property(e => e.Nombre)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Subasta>(entity =>
        {
            entity.HasKey(e => e.IdSubasta).HasName("PK__Subasta__AA418F738793D57C");

            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.IncrementoMinimo).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.PrecioBase).HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.IdAutoNavigation).WithMany(p => p.Subasta)
                .HasForeignKey(d => d.IdAuto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Subasta_Auto");

            entity.HasOne(d => d.IdEstadoSubastaNavigation).WithMany(p => p.Subasta)
                .HasForeignKey(d => d.IdEstadoSubasta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Subasta_Estado");

            entity.HasOne(d => d.IdVendedorNavigation).WithMany(p => p.Subasta)
                .HasForeignKey(d => d.IdVendedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Subasta_Vendedor");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuario__5B65BF970C075087");

            entity.HasIndex(e => e.Correo, "UQ__Usuario__60695A191DDFF5AE").IsUnique();

            entity.Property(e => e.Correo)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.NombreCompleto)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuario)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuario_Rol");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
