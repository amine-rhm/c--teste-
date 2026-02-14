using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Get;

public class GetEtudiantByIdUseCase(IRepositoryFactory factory)
{
    public async Task<Etudiant?> ExecuteAsync(long id)
    {
        CheckBusinessRules();
        return await factory.EtudiantRepository().FindAsync(id);
    }

    private void CheckBusinessRules()
    {
        ArgumentNullException.ThrowIfNull(factory);
    }
    
    public bool IsAuthorized(string role, IUniversiteUser? user, long idEtudiant)
    {
        // Scolarité et Responsables peuvent voir n'importe quel étudiant
        if (role.Equals(Roles.Scolarite) || role.Equals(Roles.Responsable))
            return true;
        
        // Un étudiant ne peut voir que ses propres infos
        return user?.Etudiant != null && 
               role.Equals(Roles.Etudiant) && 
               user.Etudiant.Id == idEtudiant;
    }
}