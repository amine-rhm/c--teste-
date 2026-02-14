using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Get;

public class GetEtudiantCompletUseCase(IRepositoryFactory factory)
{
    public async Task<Etudiant?> ExecuteAsync(long idEtudiant)
    {
        CheckBusinessRules();
        Etudiant? etudiant = await factory.EtudiantRepository().FindEtudiantCompletAsync(idEtudiant);
        return etudiant;
    }

    private void CheckBusinessRules() 
    {
        ArgumentNullException.ThrowIfNull(factory);
        IEtudiantRepository etudiantRepository = factory.EtudiantRepository();
        ArgumentNullException.ThrowIfNull(etudiantRepository);
    }

    public bool IsAuthorized(string role, IUniversiteUser? user, long idEtudiant)
    {
        // Scolarité et Responsables peuvent tout voir
        if (role.Equals(Roles.Scolarite) || role.Equals(Roles.Responsable))
            return true;
        
        // Si c'est un étudiant qui est connecté, il ne peut consulter que ses propres notes
        return user?.Etudiant != null && 
               role.Equals(Roles.Etudiant) && 
               user.Etudiant.Id == idEtudiant;
    }
}