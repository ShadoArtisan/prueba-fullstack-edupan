using Microsoft.EntityFrameworkCore;

namespace LinqEquivalente;

public class ReporteAccesosDbContext : DbContext
{
    public ReporteAccesosDbContext(DbContextOptions<ReporteAccesosDbContext> options) : base(options)
    {
    }

    public DbSet<Entidad> Entidades => Set<Entidad>();
    public DbSet<LogAcceso> LogAccesos => Set<LogAcceso>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Entidad>().ToTable("Entidades");
        modelBuilder.Entity<LogAcceso>().ToTable("LogAccesos");
    }
}
