using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;
using UniversiteEFDataProvider.Repositories;

namespace UniversiteEFDataProvider.RepositoryFactories;

public class RepositoryFactory (
    UniversiteDbContext context, 
    IUniversiteRoleRepository universiteRoleRepository, 
    IUniversiteUserRepository universiteUserRepository) : IRepositoryFactory
{
    private IParcoursRepository? _parcours;
    private IEtudiantRepository? _etudiants;
    private IUeRepository? _ues;
    private INoteRepository? _notes;
    private readonly IUniversiteRoleRepository _universiteRoleRepository = universiteRoleRepository;
    private readonly IUniversiteUserRepository _universiteUserRepository = universiteUserRepository;

    public IParcoursRepository ParcoursRepository()
    {
        if (_parcours == null)
        {
            _parcours = new ParcoursRepository(context ?? throw new InvalidOperationException());
        }
        return _parcours;
    }

    public IEtudiantRepository EtudiantRepository()
    {
        if (_etudiants == null)
        {
            _etudiants = new EtudiantRepository(context ?? throw new InvalidOperationException());
        }
        return _etudiants;
    }

    public IUeRepository UeRepository()
    {
        if (_ues == null)
        {
            _ues = new UeRepository(context ?? throw new InvalidOperationException());
        }
        return _ues;
    }

    public INoteRepository NoteRepository()
    {
        if (_notes == null)
        {
            _notes = new NoteRepository(context ?? throw new InvalidOperationException());
        }
        return _notes;
    }

    // UTILISE les instances passées dans le constructeur
    public IUniversiteRoleRepository UniversiteRoleRepository()
    {
        return _universiteRoleRepository;
    }

    public IUniversiteUserRepository UniversiteUserRepository()
    {
        return _universiteUserRepository;
    }
       
    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();  
    }

    IUniversiteRoleRepository IRepositoryFactory.UniversiteRoleRepository => _universiteRoleRepository;

    IUniversiteUserRepository IRepositoryFactory.UniversiteUserRepository => _universiteUserRepository;
    
    public Task<IUniversiteUser?> AddUserAsync(string email, string userName, string password, string role, Etudiant? etudiant)
    {
        throw new NotImplementedException();
    }

    public Task<IUniversiteUser> FindByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsInRoleAsync(string email, string role)
    {
        throw new NotImplementedException();
    }

    public async Task EnsureCreatedAsync()
    {
        await context.Database.EnsureCreatedAsync();  
    }

    public async Task EnsureDeletedAsync()
    {
        await context.Database.EnsureDeletedAsync();  
    }
}