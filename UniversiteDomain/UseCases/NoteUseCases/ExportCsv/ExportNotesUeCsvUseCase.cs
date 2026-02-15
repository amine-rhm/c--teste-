using System.Globalization;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.NoteUseCases.ExportCsv;

public class ExportNotesUeCsvUseCase(IRepositoryFactory factory)
{
    public async Task<List<NoteCsvDto>> ExecuteAsync(long ueId)
    {
        CheckBusinessRules();
        
        // Récupère l'UE
        var ue = await factory.UeRepository().FindAsync(ueId);
        if (ue == null)
            throw new ArgumentException($"UE avec l'id {ueId} non trouvée");
        
        // Récupère tous les étudiants inscrits dans les parcours qui ont cette UE
        var parcours = await factory.ParcoursRepository().FindParcoursAvecUeAsync(ueId);
        var etudiants = new List<Etudiant>();
        
        foreach (var p in parcours)
        {
            var etudsInParcours = await factory.EtudiantRepository()
                .FindByConditionAsync(e => e.ParcoursSuivi != null && e.ParcoursSuivi.Id == p.Id);
            etudiants.AddRange(etudsInParcours);
        }
        
        // Récupère les notes existantes pour cette UE
        var notes = await factory.NoteRepository().FindByConditionAsync(n => n.UeId == ueId);
        
        // Crée les DTOs
        var result = new List<NoteCsvDto>();
        foreach (var etudiant in etudiants.Distinct())
        {
            var noteExistante = notes.FirstOrDefault(n => n.EtudiantId == etudiant.Id);
            
            result.Add(new NoteCsvDto
            {
                NumEtud = etudiant.NumEtud,
                Nom = etudiant.Nom,
                Prenom = etudiant.Prenom,
                NumeroUe = ue.Numero,
                IntituleUe = ue.Intitule,
                Note = noteExistante?.Valeur.ToString(CultureInfo.InvariantCulture) ?? ""
            });
        }
        
        return result;
    }
    
    private void CheckBusinessRules()
    {
        ArgumentNullException.ThrowIfNull(factory);
    }
    
    public bool IsAuthorized(string role)
    {
        // Seule la Scolarité peut exporter
        return role.Equals(Roles.Scolarite);
    }
}