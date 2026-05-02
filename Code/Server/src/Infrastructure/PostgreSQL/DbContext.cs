using Microsoft.EntityFrameworkCore;
using IMT_Reservas.Server.Core.Entities;

namespace IMT_Reservas.Server.Infrastructure.PostgreSQL;

public class ApplicationDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Carrera> Carreras { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<GrupoEquipo> GruposEquipos { get; set; }
    public DbSet<Mueble> Muebles { get; set; }
    public DbSet<Gavetero> Gaveteros { get; set; }
    public DbSet<Equipo> Equipos { get; set; }
    public DbSet<Accesorio> Accesorios { get; set; }
    public DbSet<Componente> Componentes { get; set; }
    public DbSet<EmpresaMantenimiento> EmpresasMantenimiento { get; set; }
    public DbSet<Mantenimiento> Mantenimientos { get; set; }
    public DbSet<DetalleMantenimiento> DetallesMantenimientos { get; set; }
    public DbSet<Prestamo> Prestamos { get; set; }
    public DbSet<DetallePrestamo> DetallesPrestamos { get; set; }
    public DbSet<Contrato> Contratos { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Carrera>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => new { e.Nombre, e.EstadoEliminado });
            entity.HasQueryFilter(e => !e.EstadoEliminado);
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => new { e.Nombre, e.EstadoEliminado });
            entity.HasQueryFilter(e => !e.EstadoEliminado);
        });

        modelBuilder.Entity<EmpresaMantenimiento>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Direccion).HasMaxLength(512);
            entity.Property(e => e.Telefono).HasMaxLength(64);
            entity.Property(e => e.NombreResponsable).HasMaxLength(64);
            entity.Property(e => e.ApellidoResponsable).HasMaxLength(64);
            entity.HasIndex(e => new { e.Nombre, e.EstadoEliminado });
            entity.HasQueryFilter(e => !e.EstadoEliminado);
        });

        modelBuilder.Entity<GrupoEquipo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(256);
            entity.Property(e => e.Modelo).IsRequired().HasMaxLength(512);
            entity.Property(e => e.Marca).IsRequired().HasMaxLength(256);
            entity.Property(e => e.Descripcion).IsRequired();
            entity.Property(e => e.UrlImagen).IsRequired();
            entity.Property(e => e.CostoPromedio).HasPrecision(10, 2).HasDefaultValue(0);
            entity.Property(e => e.Cantidad).HasDefaultValue(0);
            entity.HasOne<Categoria>().WithMany().HasForeignKey(e => e.IdCategoria).IsRequired();
            entity.HasIndex(e => new { e.IdCategoria, e.Nombre, e.Modelo, e.Marca, e.EstadoEliminado });
            entity.HasIndex(e => new { e.Nombre, e.Modelo, e.Marca }).IsUnique();
            entity.HasQueryFilter(e => !e.EstadoEliminado);
        });

        modelBuilder.Entity<Mueble>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Tipo).HasMaxLength(255);
            entity.Property(e => e.Ubicacion).HasMaxLength(255);
            entity.Property(e => e.NumeroGaveteros).HasDefaultValue(0);
            entity.HasIndex(e => new { e.Nombre, e.EstadoEliminado });
            entity.HasIndex(e => e.Nombre).IsUnique();
            entity.HasQueryFilter(e => !e.EstadoEliminado);
        });

        modelBuilder.Entity<Gavetero>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Tipo).HasMaxLength(255);
            entity.HasOne<Mueble>().WithMany().HasForeignKey(e => e.IdMueble).IsRequired();
            entity.HasIndex(e => new { e.Nombre, e.IdMueble, e.EstadoEliminado });
            entity.HasIndex(e => e.Nombre).IsUnique();
            entity.HasQueryFilter(e => !e.EstadoEliminado);
        });

        modelBuilder.Entity<Equipo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CodigoImt).IsRequired();
            entity.Property(e => e.Modelo).HasMaxLength(255);
            entity.Property(e => e.Marca).HasMaxLength(255);
            entity.Property(e => e.CodigoUcb).HasMaxLength(256);
            entity.Property(e => e.NumeroSerial).HasMaxLength(255);
            entity.Property(e => e.Ubicacion).HasMaxLength(255);
            entity.Property(e => e.CostoReferencia).HasDefaultValue(0);
            entity.Property(e => e.TiempoMaximoPrestamo).HasDefaultValue(9999);
            entity.Property(e => e.FechaIngresoEquipo).HasDefaultValue(DateOnly.FromDateTime(DateTime.UtcNow));
            entity.Property(e => e.EstadoEquipo).HasDefaultValue("operativo");
            entity.HasOne<GrupoEquipo>().WithMany().HasForeignKey(e => e.IdGrupoEquipo).IsRequired();
            entity.HasOne<Gavetero>().WithMany().HasForeignKey(e => e.IdGavetero);
            entity.HasIndex(e => new { e.IdGrupoEquipo, e.CodigoImt, e.EstadoEliminado });
            entity.HasIndex(e => e.CodigoImt).IsUnique();
            entity.HasQueryFilter(e => !e.EstadoEliminado);
        });

        modelBuilder.Entity<Accesorio>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Modelo).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Tipo).HasMaxLength(255);
            entity.HasOne<Equipo>().WithMany().HasForeignKey(e => e.IdEquipo).IsRequired();
            entity.HasIndex(e => new { e.Nombre, e.IdEquipo, e.EstadoEliminado });
            entity.HasQueryFilter(e => !e.EstadoEliminado);
        });

        modelBuilder.Entity<Componente>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Modelo).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Tipo).HasMaxLength(255);
            entity.HasOne<Equipo>().WithMany().HasForeignKey(e => e.IdEquipo).IsRequired();
            entity.HasIndex(e => new { e.Nombre, e.IdEquipo, e.EstadoEliminado });
            entity.HasQueryFilter(e => !e.EstadoEliminado);
        });

        modelBuilder.Entity<Mantenimiento>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FechaMantenimiento).IsRequired();
            entity.Property(e => e.FechaFinalMantenimiento).IsRequired();
            entity.HasOne<EmpresaMantenimiento>().WithMany().HasForeignKey(e => e.IdEmpresa).IsRequired();
            entity.HasIndex(e => new { e.FechaMantenimiento, e.FechaFinalMantenimiento, e.IdEmpresa, e.EstadoEliminado });
            entity.HasQueryFilter(e => !e.EstadoEliminado);
        });

        modelBuilder.Entity<DetalleMantenimiento>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TipoMantenimiento).HasMaxLength(256);
            entity.HasOne<Mantenimiento>().WithMany().HasForeignKey(e => e.IdMantenimiento).IsRequired();
            entity.HasOne<Equipo>().WithMany().HasForeignKey(e => e.IdEquipo).IsRequired();
            entity.HasIndex(e => new { e.IdMantenimiento, e.EstadoEliminado });
            entity.HasQueryFilter(e => !e.EstadoEliminado);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Carnet);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(64);
            entity.Property(e => e.ApellidoPaterno).IsRequired().HasMaxLength(64);
            entity.Property(e => e.ApellidoMaterno).IsRequired().HasMaxLength(64);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Contrasena).IsRequired();
            entity.Property(e => e.Telefono).IsRequired().HasMaxLength(32);
            entity.Property(e => e.TelefonoReferencia).HasMaxLength(32);
            entity.Property(e => e.NombreReferencia).HasMaxLength(32);
            entity.Property(e => e.EmailReferencia).HasMaxLength(255);
            entity.Property(e => e.Rol).HasDefaultValue("estudiante");
            entity.HasOne<Carrera>().WithMany().HasForeignKey(e => e.IdCarrera).IsRequired();
            entity.HasIndex(e => new { e.Nombre, e.EstadoEliminado });
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasQueryFilter(e => !e.EstadoEliminado);
        });

        modelBuilder.Entity<Prestamo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FechaSolicitud).HasDefaultValueSql("now() AT TIME ZONE 'America/La_Paz'");
            entity.Property(e => e.FechaPrestamoEsperada).IsRequired();
            entity.Property(e => e.FechaDevolucionEsperada).IsRequired();
            entity.Property(e => e.EstadoPrestamo).HasDefaultValue("pendiente");
            entity.Property(e => e.IdContrato);
            entity.HasOne<Usuario>().WithMany().HasForeignKey(e => e.Carnet).IsRequired();
            entity.HasIndex(e => e.IdContrato);
            entity.HasIndex(e => new { e.FechaPrestamoEsperada, e.FechaDevolucionEsperada, e.Carnet, e.EstadoEliminado });
            entity.HasQueryFilter(e => !e.EstadoEliminado);
        });

        modelBuilder.Entity<DetallePrestamo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne<Prestamo>().WithMany().HasForeignKey(e => e.IdPrestamo).IsRequired();
            entity.HasOne<Equipo>().WithMany().HasForeignKey(e => e.IdEquipo).IsRequired();
            entity.HasIndex(e => new { e.IdPrestamo, e.EstadoEliminado });
            entity.HasQueryFilter(e => !e.EstadoEliminado);
        });

        modelBuilder.Entity<Contrato>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PrestamoId).IsRequired();
            entity.HasQueryFilter(e => !e.EstadoEliminado);
        });
    }
}
