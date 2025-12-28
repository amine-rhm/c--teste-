using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

public class UeRepository(UniversiteDbContext context) 
    : Repository<Ue>(context), IUeRepository
{
    // Ajouter une UE à un parcours
    public async Task<Ue> AddToParcoursAsync(long idUe, long idParcours)
    {
        Ue? ue = await Context.Ues.FindAsync(idUe);
        Parcours? p = await Context.Parcours.FindAsync(idParcours);
        ArgumentNullException.ThrowIfNull(ue);
        ArgumentNullException.ThrowIfNull(p);
        
        ue.EnseigneeDans ??= new List<Parcours>();
        p.UesEnseignees ??= new List<Ue>();
        
        ue.EnseigneeDans.Add(p);
        p.UesEnseignees.Add(ue);
        
        await Context.SaveChangesAsync();
        return ue;
    }

    public async Task<Ue> AddToParcoursAsync(Ue ue, Parcours parcours)
    {
        ArgumentNullException.ThrowIfNull(ue);
        ArgumentNullException.ThrowIfNull(parcours);
        return await AddToParcoursAsync(ue.Id, parcours.Id);
    }
}