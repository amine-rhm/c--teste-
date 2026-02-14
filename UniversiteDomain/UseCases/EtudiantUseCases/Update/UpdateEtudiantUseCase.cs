using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Update;

public class UpdateEtudiantUseCase(IRepositoryFactory factory)
{
    public async Task ExecuteAsync(Etudiant etudiant)
    {
        CheckBusinessRules(etudiant);
        await factory.EtudiantRepository().UpdateAsync(etudiant);
        await factory.SaveChangesAsync();
    }

    private void CheckBusinessRules(Etudiant etudiant)
    {
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(etudiant);
    }
    
    public bool IsAuthorized(string role)
    {
        // Seuls Scolarité et Responsables peuvent modifier un étudiant
        return role.Equals(Roles.Scolarite) || role.Equals(Roles.Responsable);
    }
}