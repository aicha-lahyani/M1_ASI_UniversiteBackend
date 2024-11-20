using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Exceptions.ParcoursExceptions;
using UniversiteDomain.Util;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Create;

public class CreateParcoursUseCase(IRepositoryFactory parcoursRepository)
{
    public async Task<Parcours> ExecuteAsync(long Id, string nomP, int anneeF)
    {
        var parcours = new Parcours { Id = Id, NomParcours = nomP, AnneeFormation = anneeF };
        return await ExecuteAsync(parcours);
    }

    public async Task<Parcours> ExecuteAsync(Parcours parcours)
    {
        await CheckBusinessRules(parcours);
        Parcours pr = await parcoursRepository.ParcoursRepository().CreateAsync(parcours);
        parcoursRepository.SaveChangesAsync().Wait();
        return pr;
    }

    private async Task CheckBusinessRules(Parcours parcours)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(parcours.NomParcours);

        List<Parcours> existe = await parcoursRepository.ParcoursRepository().FindByConditionAsync(p => p.NomParcours.Equals(parcours.NomParcours)) ?? new List<Parcours>();

        if (existe.Any())
            throw new DuplicateParcoursException(parcours.NomParcours + " Ce parcours existe déjà");

        if (parcours.AnneeFormation < 1 || parcours.AnneeFormation > 2)
            throw new FormationYearException("L'année de formation est incorrecte.");
    }
}