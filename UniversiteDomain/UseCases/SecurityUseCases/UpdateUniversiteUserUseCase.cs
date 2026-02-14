using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.SecurityUseCases;

public class UpdateUniversiteUserUseCase(IRepositoryFactory factory)
{
    public async Task ExecuteAsync(Etudiant etudiant)
    {
        CheckBusinessRules(etudiant);
        
        // Pour l'instant, juste une vérification
        // La mise à jour du user peut être gérée différemment
        // TODO: Implémenter la mise à jour du user si nécessaire
        await Task.CompletedTask;
    }

    private void CheckBusinessRules(Etudiant etudiant)
    {
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(etudiant);
    }
    
    public bool IsAuthorized(string role)
    {
        // Seuls Scolarité et Responsables peuvent modifier
        return role.Equals(Roles.Scolarite) || role.Equals(Roles.Responsable);
    }
}