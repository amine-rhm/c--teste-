using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

public class ParcoursRepository(UniversiteDbContext context) 
    : Repository<Parcours>(context), IParcoursRepository
{
    public async Task<Parcours> AddEtudiantAsync(Parcours parcours, Etudiant etudiant)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(etudiant);
        parcours.Inscrits.Add(etudiant);
        etudiant.ParcoursSuivi = parcours;
        await Context.SaveChangesAsync();
        return parcours;
    }

    public async Task<Parcours> AddEtudiantAsync(long idParcours, long idEtudiant)
    {
        Parcours? p = await Context.Parcours.FindAsync(idParcours);
        Etudiant? e = await Context.Etudiants.FindAsync(idEtudiant);
        ArgumentNullException.ThrowIfNull(p);
        ArgumentNullException.ThrowIfNull(e);
        return await AddEtudiantAsync(p, e);
    }

    public async Task<Parcours> AddEtudiantAsync(Parcours? parcours, List<Etudiant> etudiants)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(etudiants);
        foreach (var etudiant in etudiants)
        {
            parcours.Inscrits.Add(etudiant);
            etudiant.ParcoursSuivi = parcours;
        }
        await Context.SaveChangesAsync();
        return parcours;
    }

    public async Task<Parcours> AddEtudiantAsync(long idParcours, long[] idEtudiants)
    {
        Parcours? p = await Context.Parcours.FindAsync(idParcours);
        ArgumentNullException.ThrowIfNull(p);
        foreach (var idEtudiant in idEtudiants)
        {
            Etudiant? e = await Context.Etudiants.FindAsync(idEtudiant);
            ArgumentNullException.ThrowIfNull(e);
            p.Inscrits.Add(e);
            e.ParcoursSuivi = p;
        }
        await Context.SaveChangesAsync();
        return p;
    }

    public async Task<Parcours> AddUeAsync(long idParcours, long idUe)
    {
        Parcours? p = await Context.Parcours.FindAsync(idParcours);
        Ue? ue = await Context.Ues.FindAsync(idUe);
        ArgumentNullException.ThrowIfNull(p);
        ArgumentNullException.ThrowIfNull(ue);
        p.UesEnseignees ??= new List<Ue>();
        ue.EnseigneeDans ??= new List<Parcours>();
        p.UesEnseignees.Add(ue);
        ue.EnseigneeDans.Add(p);
        await Context.SaveChangesAsync();
        return p;
    }

    public async Task<Parcours> AddUeAsync(long idParcours, long[] idUes)
    {
        Parcours? p = await Context.Parcours.FindAsync(idParcours);
        ArgumentNullException.ThrowIfNull(p);
        p.UesEnseignees ??= new List<Ue>();
        foreach (var idUe in idUes)
        {
            Ue? ue = await Context.Ues.FindAsync(idUe);
            ArgumentNullException.ThrowIfNull(ue);
            ue.EnseigneeDans ??= new List<Parcours>();
            p.UesEnseignees.Add(ue);
            ue.EnseigneeDans.Add(p);
        }
        await Context.SaveChangesAsync();
        return p;
    }
}