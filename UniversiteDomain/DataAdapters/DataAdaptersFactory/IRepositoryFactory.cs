using UniversiteDomain.Entities;

namespace UniversiteDomain.DataAdapters.DataAdaptersFactory;

public interface IRepositoryFactory
{
    IParcoursRepository ParcoursRepository();
    IEtudiantRepository EtudiantRepository();
    IUeRepository UeRepository();  
    INoteRepository NoteRepository(); 
    Task EnsureDeletedAsync();
    Task EnsureCreatedAsync();
    Task SaveChangesAsync();
    
    IUniversiteRoleRepository UniversiteRoleRepository { get; }
    IUniversiteUserRepository UniversiteUserRepository { get; }
    Task<IUniversiteUser?> AddUserAsync(string email, string userName, string password, string role, Etudiant? etudiant);
    Task<IUniversiteUser> FindByEmailAsync(string email);
    Task<bool> IsInRoleAsync(string email, string role);
}
