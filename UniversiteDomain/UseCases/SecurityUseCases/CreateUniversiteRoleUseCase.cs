using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.SecurityUseCases;

public class CreateUniversiteRoleUseCase(IRepositoryFactory factory)
{
    public async Task ExecuteAsync(string role)
    {
        CheckBusinessRules(role); 
        await factory.UniversiteRoleRepository.AddRoleAsync(role);
        await factory.SaveChangesAsync();
    }

    private void CheckBusinessRules(string role) 
    {
        ArgumentNullException.ThrowIfNull(role);
        ArgumentNullException.ThrowIfNull(factory);
    }
    
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}