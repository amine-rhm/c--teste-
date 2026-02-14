using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Exceptions.NoteExceptions;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.NoteUseCase.Add;

public class CreateNoteUseCase
{
    private readonly IRepositoryFactory _repositoryFactory;

    public CreateNoteUseCase(IRepositoryFactory repositoryFactory)
    {
        _repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
    }

    public async Task<Note> ExecuteAsync(long idEtudiant, long idUe, float valeur)
    {
        var note = new Note { EtudiantId = idEtudiant, UeId = idUe, Valeur = valeur };
        return await ExecuteAsync(note);
    }

    public async Task<Note> ExecuteAsync(Note note)
    {
        await CheckBusinessRules(note);
        
        var noteRepo = _repositoryFactory.NoteRepository();
        Note created = await noteRepo.CreateAsync(note);
        await noteRepo.SaveChangesAsync();
        return created;
    }

    private async Task CheckBusinessRules(Note note)
    {
        ArgumentNullException.ThrowIfNull(note);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(note.EtudiantId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(note.UeId);

        // Règle 1 : Note entre 0 et 20
        if (note.Valeur < 0 || note.Valeur > 20)
            throw new InvalidNoteValeurException($"La note doit être comprise entre 0 et 20. Valeur : {note.Valeur}");

        var etudiantRepo = _repositoryFactory.EtudiantRepository();
        var ueRepo = _repositoryFactory.UeRepository();
        var noteRepo = _repositoryFactory.NoteRepository();

        // Vérifier que l'étudiant existe
        var etudiants = await etudiantRepo.FindByConditionAsync(e => e.Id == note.EtudiantId);
        if (etudiants.Count == 0)
            throw new EtudiantNotFoundException($"Etudiant {note.EtudiantId} introuvable");
        var etudiant = etudiants[0];

        // Vérifier que l'UE existe
        var ues = await ueRepo.FindByConditionAsync(u => u.Id == note.UeId);
        if (ues.Count == 0)
            throw new UeNotFoundException($"UE {note.UeId} introuvable");

        // Règle 2 : Un étudiant n'a qu'une note au maximum par UE
        var notesExistantes = await noteRepo.FindByConditionAsync(n => n.EtudiantId == note.EtudiantId && n.UeId == note.UeId);
        if (notesExistantes.Count > 0)
            throw new DuplicateNoteException($"L'étudiant {note.EtudiantId} a déjà une note dans l'UE {note.UeId}");

        // Règle 3 : L'étudiant ne peut avoir une note que dans une UE de son parcours
        if (etudiant.ParcoursSuivi == null)
            throw new UeNotInParcoursException($"L'étudiant {note.EtudiantId} n'est inscrit dans aucun parcours");

        var parcours = etudiant.ParcoursSuivi;
        parcours.UesEnseignees ??= new List<Ue>();
        
        bool ueInParcours = parcours.UesEnseignees.Any(u => u.Id == note.UeId);
        if (!ueInParcours)
            throw new UeNotInParcoursException($"L'UE {note.UeId} n'est pas dans le parcours de l'étudiant {note.EtudiantId}");
    }
}