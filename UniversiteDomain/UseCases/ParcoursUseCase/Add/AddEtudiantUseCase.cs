using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Exceptions.ParcoursExceptions;

namespace UniversiteDomain.UseCases.ParcoursUseCase.Add;

public class AddEtudiantDansParcoursUseCase
{
    private readonly IRepositoryFactory repositoryFactory;

    public AddEtudiantDansParcoursUseCase(IRepositoryFactory repositoryFactory)
    {
        this.repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
    }

    // Ajouter un étudiant à un parcours
    public async Task<Parcours> ExecuteAsync(Parcours parcours, Etudiant etudiant)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(etudiant);
        return await ExecuteAsync(parcours.Id, etudiant.Id);
    }

    public async Task<Parcours> ExecuteAsync(long idParcours, long idEtudiant)
    {
        await CheckBusinessRules(idParcours, idEtudiant);
        return await repositoryFactory.ParcoursRepository().AddEtudiantAsync(idParcours, idEtudiant);
    }

    // Ajouter plusieurs étudiants
    public async Task<Parcours> ExecuteAsync(Parcours parcours, List<Etudiant> etudiants)
    {
        long[] idEtudiants = etudiants.Select(x => x.Id).ToArray();
        return await ExecuteAsync(parcours.Id, idEtudiants);
    }

    public async Task<Parcours> ExecuteAsync(long idParcours, long[] idEtudiants)
    {
        foreach (var id in idEtudiants)
            await CheckBusinessRules(idParcours, id);

        return await repositoryFactory.ParcoursRepository().AddEtudiantAsync(idParcours, idEtudiants);
    }

    private async Task CheckBusinessRules(long idParcours, long idEtudiant)
    {
        if (idParcours <= 0) throw new ArgumentOutOfRangeException(nameof(idParcours));
        if (idEtudiant <= 0) throw new ArgumentOutOfRangeException(nameof(idEtudiant));

        var etudiantRepo = repositoryFactory.EtudiantRepository() 
            ?? throw new ArgumentNullException("EtudiantRepository null");
        var parcoursRepo = repositoryFactory.ParcoursRepository() 
            ?? throw new ArgumentNullException("ParcoursRepository null");

        // Vérifier que l'étudiant existe
        List<Etudiant> etudiants = await etudiantRepo.FindByConditionAsync(e => e.Id == idEtudiant);
        if (etudiants.Count == 0) throw new EtudiantNotFoundException(idEtudiant.ToString());

        // Vérifier que le parcours existe
        List<Parcours> parcoursList = await parcoursRepo.FindByConditionAsync(p => p.Id == idParcours);
        if (parcoursList.Count == 0) throw new ParcoursNotFoundException(idParcours.ToString());

        // Vérifier que l'étudiant n'est pas déjà inscrit
        var inscrit = await etudiantRepo.FindByConditionAsync(
            e => e.Id == idEtudiant && e.Parcours != null && e.Parcours.Id == idParcours
        );
        if (inscrit.Count > 0)
            throw new DuplicateInscriptionException(
                $"{idEtudiant} est déjà inscrit dans le parcours {idParcours}"
            );
    }
}
