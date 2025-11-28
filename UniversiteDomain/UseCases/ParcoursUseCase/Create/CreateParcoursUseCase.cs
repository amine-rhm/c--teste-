using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursExceptions;

namespace UniversiteDomain.UseCases.ParcoursUseCase.Create;

public class CreateParcoursUseCase
{
    private readonly IRepositoryFactory _repositoryFactory;

    public CreateParcoursUseCase(IRepositoryFactory repositoryFactory)
    {
        _repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
    }

    public async Task<Parcours> ExecuteAsync(string nomParcours, int anneeFormation)
    {
        var parcours = new Parcours { NomParcours = nomParcours, AnneeFormation = anneeFormation };
        return await ExecuteAsync(parcours);
    }

    public async Task<Parcours> ExecuteAsync(Parcours parcours)
    {
        await CheckBusinessRules(parcours);
        
        var parcoursRepo = _repositoryFactory.ParcoursRepository();
        Parcours pa = await parcoursRepo.CreateAsync(parcours);
        await parcoursRepo.SaveChangesAsync();
        return pa;
    }

    private async Task CheckBusinessRules(Parcours parcours)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(parcours.NomParcours);

        if (parcours.AnneeFormation is not (1 or 2))
            throw new InvalideAnneeFormationException("L'année de formation doit être 1 ou 2.");

        var parcoursRepo = _repositoryFactory.ParcoursRepository();
        List<Parcours> existe = await parcoursRepo.FindByConditionAsync(
            p => p.NomParcours.Equals(parcours.NomParcours)
        );

        if (existe.Count > 0)
            throw new DuplicateNomParcoursException($"{parcours.NomParcours} - ce parcours existe déjà");
    }
}

internal class InvalideAnneeFormationException : Exception
{
    public InvalideAnneeFormationException(string message) : base(message) { }
}

internal class DuplicateNomParcoursException : Exception
{
    public DuplicateNomParcoursException(string message) : base(message) { }
}