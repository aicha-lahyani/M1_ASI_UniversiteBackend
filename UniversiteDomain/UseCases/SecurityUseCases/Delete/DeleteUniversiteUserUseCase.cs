

using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace Université_Domain.UseCases.SecurityUseCases.Delete;

public class DeleteUniversiteUserUseCase(IRepositoryFactory repository)
{
    // Executes the delete operation for a given Etudiant (based on NumEtud)
    public async Task<bool> ExecuteAsync(long numEtud)
    {
        // Ensure the input is a valid student number (non-zero)
        if (numEtud == 0)
        {
            throw new ArgumentException("Numéro d'étudiant is required", nameof(numEtud));
        }

        // Search for the Etudiant by their NumEtud
        var etudiant = await repository.UniversiteUserRepository().FindByConditionAsync(e => e.EtudiantId.Equals(numEtud));

        // If no Etudiant is found, throw an exception
        /*if (!etudiant.Any())
        {
            throw new EtudiantNotFoundException($"A student with NumEtud {numEtud} does not exist.");
        }*/

        // If the Etudiant exists, delete them
        //var etudiantToDelete = etudiant.First(); // We expect only one result, as NumEtud should be unique
        await repository.UniversiteUserRepository().DeleteAsync(numEtud);

        // Commit the transaction (save changes to the database)
        await repository.SaveChangesAsync();
        Console.WriteLine($"Deleting user with NumEtud ana hna rah daz");


        return true; // Return true to indicate successful deletion
    }


    // Authorization check to verify if the user has the right role to perform the delete operation
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}