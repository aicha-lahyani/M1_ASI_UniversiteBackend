using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

public class UeRepository(UniversiteDbContext context) : Repository<Ue>(context), IUeRepository
{
    // Associe une UE à un parcours
    public async Task AssocierUeAuParcoursAsync(long idUe, long idParcours)
    {
        ArgumentNullException.ThrowIfNull(Context.Ues);
        ArgumentNullException.ThrowIfNull(Context.Parcours);
        
        Ue ue = (await Context.Ues.FindAsync(idUe))!;
        Parcours parcours = (await Context.Parcours.FindAsync(idParcours))!;
        
        if (!parcours.UesEnseignees.Contains(ue))
        {
            parcours.UesEnseignees.Add(ue);
            await Context.SaveChangesAsync();
        }
    }

    // Récupère toutes les UE pour un parcours donné
    public async Task<List<Ue>> ObtenirUesParParcoursAsync(long idParcours)
    {
        ArgumentNullException.ThrowIfNull(Context.Parcours);

        Parcours parcours = (await Context.Parcours.FindAsync(idParcours))!;
        return parcours.UesEnseignees.ToList();
    }
}