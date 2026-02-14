using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Get;

public class GetTousLesEtudiantsUseCase(IRepositoryFactory factory)
{
    public async Task<List<Etudiant>> ExecuteAsync()
    {
        CheckBusinessRules();
        return await factory.EtudiantRepository().FindAllAsync();
    }

    private void CheckBusinessRules()
    {
        ArgumentNullException.ThrowIfNull(factory);
    }
    
    public bool IsAuthorized(string role)
    {
        // Seuls Scolarité et Responsables peuvent voir tous les étudiants
        return role.Equals(Roles.Scolarite) || role.Equals(Roles.Responsable);
    }
}