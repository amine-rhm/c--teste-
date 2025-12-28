using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UniversiteDomain.Entities;

namespace UniversiteEFDataProvider.Data;

public class UniversiteDbContext : DbContext
{
    public static readonly ILoggerFactory ConsoleLogger = LoggerFactory.Create(builder => { builder.AddConsole(); });
    
    public UniversiteDbContext(DbContextOptions<UniversiteDbContext> options)
        : base(options)
    {
    }

    public UniversiteDbContext() : base()
    {
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLoggerFactory(ConsoleLogger)
            .EnableSensitiveDataLogging() 
            .EnableDetailedErrors();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // ========== ETUDIANT ==========
        modelBuilder.Entity<Etudiant>()
            .HasKey(e => e.Id);
        modelBuilder.Entity<Etudiant>()
            .HasOne(e => e.Parcours)
            .WithMany(p => p.Inscrits);
        modelBuilder.Entity<Etudiant>()
            .HasMany(e => e.Notes)
            .WithOne(n => n.Etudiant);
        
        // ========== PARCOURS ==========
        modelBuilder.Entity<Parcours>()
            .HasKey(p => p.Id);
        modelBuilder.Entity<Parcours>()
            .HasMany(p => p.Inscrits)
            .WithOne(e => e.Parcours);
        modelBuilder.Entity<Parcours>()
            .HasMany(p => p.UesEnseignees)
            .WithMany(ue => ue.EnseigneeDans);

        // ========== UE ==========
        modelBuilder.Entity<Ue>()
            .HasKey(ue => ue.Id);
        modelBuilder.Entity<Ue>()
            .HasMany(ue => ue.EnseigneeDans)
            .WithMany(p => p.UesEnseignees);
        modelBuilder.Entity<Ue>()
            .HasMany(ue => ue.Notes)
            .WithOne(n => n.Ue);
        
        // ========== NOTE ==========
        modelBuilder.Entity<Note>()
            .HasKey(n => new { n.EtudiantId, n.UeId });
        modelBuilder.Entity<Note>()
            .HasOne(n => n.Etudiant)
            .WithMany(e => e.Notes);
        modelBuilder.Entity<Note>()
            .HasOne(n => n.Ue)
            .WithMany(ue => ue.Notes);
    }
    
    public DbSet <Parcours>? Parcours { get; set; }
    public DbSet <Etudiant>? Etudiants { get; set; }
    public DbSet <Ue>? Ues { get; set; }
    public DbSet <Note>? Notes { get; set; }
    
}