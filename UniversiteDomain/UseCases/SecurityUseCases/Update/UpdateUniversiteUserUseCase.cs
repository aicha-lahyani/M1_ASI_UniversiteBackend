
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace Université_Domain.UseCases.SecurityUseCases.Update;

public class UpdateUniversiteUserUseCase(IRepositoryFactory factory)
{
    
    public async Task<Etudiant?> ExecuteAsync(Etudiant et)
    {
        // Ensure the input is not null
        if (et == null)
        {
            throw new ArgumentNullException(nameof(et), "Etudiant cannot be null");
        }

        // Check business rules
        await CheckBusinessRules();

        try
        {
            // Update the entity in the repository
            await factory.EtudiantRepository().UpdateAsync(et);

            // Commit the changes
            await factory.SaveChangesAsync();

            // Return the updated entity
            return et;
        }
        catch (Exception e)
        {
            // Log the exception (this could also be a proper logging framework)
            Console.WriteLine($"Error occurred while updating Etudiant: {e.Message}");
        
            // Rethrow the exception to ensure the caller is aware of the failure
            throw;
        }
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