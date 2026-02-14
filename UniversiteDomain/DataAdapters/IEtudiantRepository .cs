using UniversiteDomain.Entities;

namespace UniversiteDomain.DataAdapters;

public interface IEtudiantRepository : IRepository<Etudiant>
{
    Task<Etudiant?> FindEtudiantCompletAsync(long idEtudiant);
}

