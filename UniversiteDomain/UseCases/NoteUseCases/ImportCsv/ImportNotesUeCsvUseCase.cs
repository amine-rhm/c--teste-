using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.NoteUseCases.ImportCsv;

public class ImportNotesUeCsvUseCase(IRepositoryFactory factory)
{
    public async Task ExecuteAsync(List<NoteCsvDto> notesCsv, long ueId)
    {
        CheckBusinessRules(notesCsv);
        
        // ÉTAPE 1 : Valider TOUTES les lignes AVANT d'enregistrer quoi que ce soit
        var erreurs = new List<string>();
        var notesAEnregistrer = new List<(long etudiantId, long ueId, float valeur)>();
        
        foreach (var ligne in notesCsv)
        {
            // Ignorer les lignes sans note
            if (string.IsNullOrWhiteSpace(ligne.Note))
                continue;
            
            // Valider le format de la note
            if (!float.TryParse(ligne.Note, System.Globalization.CultureInfo.InvariantCulture, out float valeurNote))
            {
                erreurs.Add($"Note invalide pour {ligne.NumEtud} : '{ligne.Note}' n'est pas un nombre");
                continue;
            }
            
            // Valider que la note est entre 0 et 20
            if (valeurNote < 0 || valeurNote > 20)
            {
                erreurs.Add($"Note invalide pour {ligne.NumEtud} : {valeurNote} doit être entre 0 et 20");
                continue;
            }
            
            // Vérifier que l'étudiant existe
            var etudiants = await factory.EtudiantRepository()
                .FindByConditionAsync(e => e.NumEtud == ligne.NumEtud);
            
            if (etudiants.Count == 0)
            {
                erreurs.Add($"Étudiant non trouvé : {ligne.NumEtud}");
                continue;
            }
            
            var etudiant = etudiants.First();
            notesAEnregistrer.Add((etudiant.Id, ueId, valeurNote));
        }
        
        // S'il y a des erreurs, on ARRÊTE TOUT
        if (erreurs.Count > 0)
        {
            throw new InvalidOperationException(
                "Erreurs dans le fichier CSV : " + string.Join("; ", erreurs));
        }
        
        // ÉTAPE 2 : Tout est OK, on enregistre TOUTES les notes
        foreach (var (etudiantId, ueIdParam, valeur) in notesAEnregistrer)
        {
            // Vérifier si une note existe déjà
            var notesExistantes = await factory.NoteRepository()
                .FindByConditionAsync(n => n.EtudiantId == etudiantId && n.UeId == ueIdParam);
            
            if (notesExistantes.Count > 0)
            {
                // Mise à jour
                var noteExistante = notesExistantes.First();
                noteExistante.Valeur = valeur;
                await factory.NoteRepository().UpdateAsync(noteExistante);
            }
            else
            {
                // Création
                var nouvelleNote = new Note
                {
                    EtudiantId = etudiantId,
                    UeId = ueIdParam,
                    Valeur = valeur
                };
                await factory.NoteRepository().CreateAsync(nouvelleNote);
            }
        }
        
        await factory.SaveChangesAsync();
    }
    
    private void CheckBusinessRules(List<NoteCsvDto> notesCsv)
    {
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(notesCsv);
    }
    
    public bool IsAuthorized(string role)
    {
        // Seule la Scolarité peut importer
        return role.Equals(Roles.Scolarite);
    }
}