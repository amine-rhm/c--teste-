using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Entities;

namespace UniversiteEFDataProvider.Data;
 
public class UniversiteDbContext : IdentityDbContext<UniversiteUser>
{
    public static readonly ILoggerFactory ConsoleLogger = LoggerFactory.Create(builder => { builder.AddConsole(); });
    
    public UniversiteDbContext(DbContextOptions<UniversiteDbContext> options)
        : base(options)
    {
    }
 
    public UniversiteDbContext()
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
        
        // Configuration Etudiant
        modelBuilder.Entity<Etudiant>()
            .HasKey(e => e.Id);
        
        modelBuilder.Entity<Etudiant>()
            .HasOne(e => e.ParcoursSuivi)
            .WithMany(p => p.Inscrits)
            .OnDelete(DeleteBehavior.SetNull);  // Ne pas supprimer le parcours
        
        modelBuilder.Entity<Etudiant>()
            .HasMany(e => e.NotesObtenues)
            .WithOne(n => n.Etudiant)
            .OnDelete(DeleteBehavior.Cascade);  // Supprimer les notes en cascade
        
        // Configuration Parcours
        modelBuilder.Entity<Parcours>()
            .HasKey(p => p.Id);
        
        modelBuilder.Entity<Parcours>()
            .HasMany(p => p.UesEnseignees)
            .WithMany(ue => ue.EnseigneeDans);

        // Configuration Ue
        modelBuilder.Entity<Ue>()
            .HasKey(ue => ue.Id);
        
        modelBuilder.Entity<Ue>()
            .HasMany(ue => ue.Notes)
            .WithOne(n => n.Ue)
            .OnDelete(DeleteBehavior.Cascade);  // Supprimer les notes si UE supprimée
        
        // Configuration Note
        modelBuilder.Entity<Note>()
            .HasKey(n => new { n.EtudiantId, n.UeId });
        
        // Configuration UniversiteUser <-> Etudiant
        modelBuilder.Entity<UniversiteUser>()
            .HasOne<Etudiant>(user => user.Etudiant)
            .WithOne()
            .HasForeignKey<UniversiteUser>(user => user.EtudiantId)
            .OnDelete(DeleteBehavior.SetNull);  // Si étudiant supprimé, mettre EtudiantId à null
        
        // Auto-inclusion de l'étudiant dans le user
        modelBuilder.Entity<UniversiteUser>().Navigation<Etudiant>(user => user.Etudiant).AutoInclude();
        
        modelBuilder.Entity<UniversiteRole>();
    }
    
    public DbSet<Parcours>? Parcours { get; set; }
    public DbSet<Etudiant>? Etudiants { get; set; }
    public DbSet<Ue>? Ues { get; set; }
    public DbSet<Note>? Notes { get; set; }
    public DbSet<UniversiteUser>? UniversiteUsers { get; set; }
    public DbSet<UniversiteRole>? UniversiteRoles { get; set; }
}