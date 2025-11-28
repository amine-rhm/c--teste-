using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursExceptions;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.ParcoursUseCases.UeDansParcours;

public class AddUeDansParcoursUseCase
{
    private readonly IRepositoryFactory repositoryFactory;

    public AddUeDansParcoursUseCase(IRepositoryFactory repositoryFactory)
    {
        this.repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
    }

    // Ajouter une UE à un parcours
    public async Task<Parcours> ExecuteAsync(Parcours parcours, Ue ue)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(ue);
        return await ExecuteAsync(parcours.Id, ue.Id);
    }

    public async Task<Parcours> ExecuteAsync(long idParcours, long idUe)
    {
        await CheckBusinessRules(idParcours, idUe);
        return await repositoryFactory.ParcoursRepository().AddUeAsync(idParcours, idUe);
    }

    // Ajouter plusieurs UEs à un parcours
    public async Task<Parcours> ExecuteAsync(Parcours parcours, List<Ue> ues)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(ues);

        long[] idUes = ues.Select(u => u.Id).ToArray();
        return await ExecuteAsync(idParcours: parcours.Id, idUes: idUes);
    }

    public async Task<Parcours> ExecuteAsync(long idParcours, long[] idUes)
    {
        ArgumentNullException.ThrowIfNull(idUes);
        
        if (idUes.Length == 0)
            throw new ArgumentException("La liste des UEs ne peut pas être vide", nameof(idUes));

        Parcours parcoursFinal = null!;
        foreach (var idUe in idUes)
        {
            await CheckBusinessRules(idParcours, idUe);
            parcoursFinal = await repositoryFactory.ParcoursRepository().AddUeAsync(idParcours, idUe);
        }
        return parcoursFinal;
    }

    private async Task CheckBusinessRules(long idParcours, long idUe)
    {
        if (idParcours <= 0) throw new ArgumentOutOfRangeException(nameof(idParcours));
        if (idUe <= 0) throw new ArgumentOutOfRangeException(nameof(idUe));

        var ueRepo = repositoryFactory.UeRepository() 
                     ?? throw new InvalidOperationException("IUeRepository est null");
        var parcoursRepo = repositoryFactory.ParcoursRepository() 
                           ?? throw new InvalidOperationException("IParcoursRepository est null");

        // Vérifier que l'UE existe
        List<Ue> ueList = await ueRepo.FindByConditionAsync(u => u.Id == idUe);
        if (ueList.Count == 0)
            throw new UeNotFoundException($"UE {idUe} introuvable");

        // Vérifier que le parcours existe
        List<Parcours> parcoursList = await parcoursRepo.FindByConditionAsync(p => p.Id == idParcours);
        Parcours parcours = parcoursList.FirstOrDefault()
                            ?? throw new ParcoursNotFoundException($"Parcours {idParcours} introuvable");

        // Initialisation si null (normalement pas nécessaire si bien configuré)
        parcours.UesEnseignees ??= new List<Ue>();

        // Vérifier que l'UE n'est pas déjà présente
        if (parcours.UesEnseignees.Any(u => u.Id == idUe))
            throw new DuplicateUeDansParcoursException($"UE {idUe} est déjà présente dans le parcours {idParcours}");
    }
}