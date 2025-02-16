using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.UeUseCases.Create;

public class CreateUeUseCase
{
    private readonly IRepositoryFactory repositoryFactory;

    public CreateUeUseCase(IRepositoryFactory repositoryFactory)
    {
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        this.repositoryFactory = repositoryFactory;
    }

    public async Task<Ue> ExecuteAsync(string numeroUe, string intitule)
    {
        var ue = new Ue { NumeroUe = numeroUe, Intitule = intitule };
        return await ExecuteAsync(ue);
    }

    public async Task<Ue> ExecuteAsync(Ue ue)
    {
        await CheckBusinessRules(ue);
        Ue createdUe = await repositoryFactory.UeRepository().CreateAsync(ue);
        await repositoryFactory.UeRepository().SaveChangesAsync();
        return createdUe;
    }

    private async Task CheckBusinessRules(Ue ue)
    {
        ArgumentNullException.ThrowIfNull(ue);
        ArgumentNullException.ThrowIfNull(ue.NumeroUe);
        ArgumentNullException.ThrowIfNull(ue.Intitule);

        var existe = await repositoryFactory.UeRepository().FindByConditionAsync(p => p.NumeroUe.Equals(ue.NumeroUe)) ?? new List<Ue>();
        if (existe.Any()) 
            throw new DuplicateUeException($"{ue.NumeroUe} - Cette UE existe déjà.");
    }
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}