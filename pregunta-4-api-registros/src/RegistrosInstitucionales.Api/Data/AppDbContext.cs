using Microsoft.EntityFrameworkCore;
using RegistrosInstitucionales.Api.Entities;

namespace RegistrosInstitucionales.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Entidad> Entidades => Set<Entidad>();
    public DbSet<Registro> Registros => Set<Registro>();
    public DbSet<LogAcceso> LogAccesos => Set<LogAcceso>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Entidad>(entity =>
        {
            entity.ToTable("Entidades");
            entity.HasKey(e => e.EntidadId);
            entity.Property(e => e.NombreEntidad).HasMaxLength(200).IsRequired();
            entity.Property(e => e.ApiKeyHash).HasMaxLength(64).IsFixedLength().IsRequired();
            entity.Property(e => e.FechaInicioConvenio).HasColumnType("date");
            entity.Property(e => e.FechaVencimiento).HasColumnType("date");
            entity.HasIndex(e => e.ApiKeyHash).IsUnique();
        });

        modelBuilder.Entity<Registro>(entity =>
        {
            entity.ToTable("Registros");
            entity.HasKey(r => r.RegistroId);
            entity.Property(r => r.Identificador).HasMaxLength(20).IsRequired();
            entity.Property(r => r.Nombre).HasMaxLength(200).IsRequired();
            entity.Property(r => r.Estado).HasMaxLength(50).IsRequired();
            entity.Property(r => r.NumeroRegistro).HasMaxLength(50).IsRequired();
            entity.Property(r => r.FechaEvento).HasColumnType("date");
            entity.Property(r => r.FechaInscripcion).HasColumnType("date");
            // Coincide con el índice IX_Registros_Identificador_Nombre del script de esquema (Pregunta 7).
            entity.HasIndex(r => new { r.Identificador, r.Nombre });
        });

        modelBuilder.Entity<LogAcceso>(entity =>
        {
            entity.ToTable("LogAccesos");
            entity.HasKey(l => l.LogId);
            entity.Property(l => l.FechaHora).HasColumnType("datetime2(3)");
            entity.Property(l => l.TipoConsulta).HasMaxLength(50).IsRequired();
            entity.Property(l => l.Resultado).HasMaxLength(30).IsRequired();
            entity.Property(l => l.Motivo).HasMaxLength(200);
            // Coincide con el índice IX_LogAccesos_EntidadId_FechaHora del script de esquema (Pregunta 7).
            entity.HasIndex(l => new { l.EntidadId, l.FechaHora });
        });
    }
}
