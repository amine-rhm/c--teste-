using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

public class NoteRepository(UniversiteDbContext context) 
    : Repository<Note>(context), INoteRepository
{
    // Récupérer les notes d'un étudiant
    public async Task<List<Note>> GetNotesByEtudiantAsync(long idEtudiant)
    {
        return await Context.Notes
            .Where(n => n.EtudiantId == idEtudiant)
            .ToListAsync();
    }

    // Récupérer les notes d'une UE
    public async Task<List<Note>> GetNotesByUeAsync(long idUe)
    {
        return await Context.Notes
            .Where(n => n.UeId == idUe)
            .ToListAsync();
    }

    // Récupérer une note spécifique (étudiant + UE)
    public async Task<Note?> GetNoteAsync(long idEtudiant, long idUe)
    {
        return await Context.Notes
            .FirstOrDefaultAsync(n => n.EtudiantId == idEtudiant && n.UeId == idUe);
    }
}