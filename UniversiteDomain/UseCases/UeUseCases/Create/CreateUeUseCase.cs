using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions;

namespace UniversiteDomain.UseCases.UeUseCase;

public class CreateUeUseCase
{
    private readonly IUeRepository ueRepository;

    public CreateUeUseCase(IUeRepository ueRepository)
    {
        this.ueRepository = ueRepository;
    }

    public async Task<Ue> ExecuteAsync(Ue ue)
    {
        ArgumentNullException.ThrowIfNull(ue);
        ArgumentNullException.ThrowIfNull(ue.Numero);
        ArgumentNullException.ThrowIfNull(ue.Intitule);

        // Règle métier : Intitulé > 3 caractères
        if (ue.Intitule.Length < 4)
            throw new InvalidUeIntituleException($"Intitulé '{ue.Intitule}' trop court.");

        // Règle métier : Numéro unique
        var existe = await ueRepository.FindByConditionAsync(u => u.Numero.Equals(ue.Numero));
        if (existe.Any())
            throw new DuplicateUeNumeroException($"Numéro '{ue.Numero}' déjà utilisé.");

        // Création
        Ue ueCree = await ueRepository.CreateAsync(ue);
        await ueRepository.SaveChangesAsync();

        return ueCree;
    }
}

// Exceptions personnalisées
public class DuplicateUeNumeroException : Exception
{
    public DuplicateUeNumeroException(string message) : base(message) { }
}

public class InvalidUeIntituleException : Exception
{
    public InvalidUeIntituleException(string message) : base(message) { }
}