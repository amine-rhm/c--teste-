using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.SecurityUseCases;

public class IsInRoleUseCase(IRepositoryFactory factory)
{
    public async Task<bool> ExecuteAsync(string email, string role)
    {
        await CheckBusinessRules(email);
        return await factory.UniversiteUserRepository.IsInRoleAsync(email, role);
    }

    private Task CheckBusinessRules(string email)
    {
        ArgumentNullException.ThrowIfNull(email);
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(factory.UniversiteUserRepository);
        return Task.CompletedTask;
    }
  
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite) || role.Equals(Roles.Etudiant);
    }
}