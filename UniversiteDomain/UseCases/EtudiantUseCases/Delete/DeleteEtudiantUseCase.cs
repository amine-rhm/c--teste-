using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Delete;

public class DeleteEtudiantUseCase(IRepositoryFactory factory)
{
    public async Task ExecuteAsync(long id)
    {
        CheckBusinessRules();
        await factory.EtudiantRepository().DeleteAsync(id);
        await factory.SaveChangesAsync();
    }

    private void CheckBusinessRules()
    {
        ArgumentNullException.ThrowIfNull(factory);
    }
    
    public bool IsAuthorized(string role)
    {
        // Seuls Scolarité et Responsables peuvent supprimer un étudiant
        return role.Equals(Roles.Scolarite) || role.Equals(Roles.Responsable);
    }
}