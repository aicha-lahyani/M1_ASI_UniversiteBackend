using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

public class ParcoursRepository(UniversiteDbContext context) : Repository<Parcours>(context), IParcoursRepository
{
     public async Task<Parcours> AddEtudiantAsync(Parcours parcours, Etudiant etudiant)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(etudiant);

        ArgumentNullException.ThrowIfNull(Context.Parcours);
        ArgumentNullException.ThrowIfNull(Context.Etudiants);

        // Vérifie que le parcours est chargé
        var existingParcours = await Context.Parcours.FindAsync(parcours.Id);
        if (existingParcours == null)
            throw new ArgumentException("Le parcours spécifié n'existe pas.");

        // Ajoute l'étudiant au parcours
        existingParcours.Inscrits.Add(etudiant);
        await Context.SaveChangesAsync();
        return existingParcours;
    }

    public async Task<Parcours> AddEtudiantAsync(long idParcours, long idEtudiant)
    {
        ArgumentNullException.ThrowIfNull(Context.Parcours);
        ArgumentNullException.ThrowIfNull(Context.Etudiants);

        var parcours = await Context.Parcours.FindAsync(idParcours);
        if (parcours == null)
            throw new ArgumentException("Le parcours spécifié n'existe pas.");

        var etudiant = await Context.Etudiants.FindAsync(idEtudiant);
        if (etudiant == null)
            throw new ArgumentException("L'étudiant spécifié n'existe pas.");

        parcours.Inscrits.Add(etudiant);
        await Context.SaveChangesAsync();
        return parcours;
    }

    public async Task<Parcours> AddEtudiantAsync(Parcours? parcours, List<Etudiant> etudiants)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(etudiants);

        ArgumentNullException.ThrowIfNull(Context.Parcours);
        ArgumentNullException.ThrowIfNull(Context.Etudiants);

        var existingParcours = await Context.Parcours.FindAsync(parcours.Id);
        if (existingParcours == null)
            throw new ArgumentException("Le parcours spécifié n'existe pas.");

        foreach (var etudiant in etudiants)
        {
            existingParcours.Inscrits.Add(etudiant);
        }

        await Context.SaveChangesAsync();
        return existingParcours;
    }

    public async Task<Parcours> AddEtudiantAsync(long idParcours, long[] idEtudiants)
    {
        ArgumentNullException.ThrowIfNull(Context.Parcours);
        ArgumentNullException.ThrowIfNull(Context.Etudiants);

        var parcours = await Context.Parcours.FindAsync(idParcours);
        if (parcours == null)
            throw new ArgumentException("Le parcours spécifié n'existe pas.");

        foreach (var idEtudiant in idEtudiants)
        {
            var etudiant = await Context.Etudiants.FindAsync(idEtudiant);
            if (etudiant != null)
            {
                parcours.Inscrits.Add(etudiant);
            }
        }

        await Context.SaveChangesAsync();
        return parcours;
    }

    public async Task<Parcours> AddUeAsync(long idParcours, long[] idUe)
    {
        ArgumentNullException.ThrowIfNull(Context.Parcours);
        ArgumentNullException.ThrowIfNull(Context.Ues);

        var parcours = await Context.Parcours.FindAsync(idParcours);
        if (parcours == null)
            throw new ArgumentException("Le parcours spécifié n'existe pas.");

        foreach (var id in idUe)
        {
            var ue = await Context.Ues.FindAsync(id);
            if (ue != null && !parcours.UesEnseignees.Contains(ue))
            {
                parcours.UesEnseignees.Add(ue);
            }
        }

        await Context.SaveChangesAsync();
        return parcours;
    }

    public async Task<Parcours> AddUeAsync(long idParcours, long idUe)
    {
        ArgumentNullException.ThrowIfNull(Context.Parcours);
        ArgumentNullException.ThrowIfNull(Context.Ues);

        var parcours = await Context.Parcours.FindAsync(idParcours);
        if (parcours == null)
            throw new ArgumentException("Le parcours spécifié n'existe pas.");

        var ue = await Context.Ues.FindAsync(idUe);
        if (ue == null)
            throw new ArgumentException("L'UE spécifiée n'existe pas.");

        if (!parcours.UesEnseignees.Contains(ue))
        {
            parcours.UesEnseignees.Add(ue);
        }

        await Context.SaveChangesAsync();
        return parcours;
    }
   
}