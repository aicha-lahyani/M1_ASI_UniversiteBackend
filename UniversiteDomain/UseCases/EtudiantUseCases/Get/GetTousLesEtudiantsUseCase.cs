
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace Université_Domain.UseCases.EtudiantUseCases.Get;

public class GetTousLesEtudiantsUseCase(IRepositoryFactory factory)
{
    public async Task<List<Etudiant?>> ExecuteAsync()
    {
        await CheckBusinessRules();
        List<Etudiant> etudiant = await factory.EtudiantRepository().FindAllAsync();
        if (etudiant.Count == 0)
        { 
            throw new InvalidOperationException("Failed to retrieve students; result is null.");
            
        }
        return etudiant;
    }
    private async Task CheckBusinessRules()
    {
        ArgumentNullException.ThrowIfNull(factory);
        IEtudiantRepository etudiantRepository=factory.EtudiantRepository();
        ArgumentNullException.ThrowIfNull(etudiantRepository);
    }
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}