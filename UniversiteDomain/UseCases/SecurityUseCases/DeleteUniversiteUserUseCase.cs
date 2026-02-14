using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.SecurityUseCases;

public class DeleteUniversiteUserUseCase(IRepositoryFactory factory)
{
    public async Task ExecuteAsync(long idEtudiant)
    {
        CheckBusinessRules();
        
        // Récupère l'étudiant
        var etudiant = await factory.EtudiantRepository().FindAsync(idEtudiant);
        if (etudiant == null)
            throw new ArgumentException($"Étudiant avec l'id {idEtudiant} non trouvé");
        
        // Pour l'instant, on ne supprime que l'étudiant
        // La suppression du user est gérée par cascade ou manuellement
        // TODO: Implémenter la suppression du user si nécessaire
    }

    private void CheckBusinessRules()
    {
        ArgumentNullException.ThrowIfNull(factory);
    }
    
    public bool IsAuthorized(string role)
    {
        // Seuls Scolarité et Responsables peuvent supprimer
        return role.Equals(Roles.Scolarite) || role.Equals(Roles.Responsable);
    }
}