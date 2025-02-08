using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Delete;

public class DeleteEtudiantUseCase (IRepositoryFactory etudiantRepository)
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
        var etudiant = await etudiantRepository.EtudiantRepository().FindByConditionAsync(e => e.NumEtud.Equals(numEtud));
        Console.WriteLine($"Deleting student with NumEtud {numEtud} &&");

        // If no Etudiant is found, throw an exception
        /*if (!etudiant.Any())
        {
            Console.WriteLine($"Deleting student with NumEtud {numEtud}aaa");

            throw new EtudiantNotFoundException($"A student with NumEtud {numEtud} does not exist.");
        }*/
        Console.WriteLine($"Deleting student with NumEtud {numEtud}");
        // If the Etudiant exists, delete them
        //var etudiantToDelete = etudiant.First(); // We expect only one result, as NumEtud should be unique
        await etudiantRepository.EtudiantRepository().DeleteAsync(numEtud);

        // Commit the transaction (save changes to the database)
        await etudiantRepository.SaveChangesAsync();
        
        Console.WriteLine($"Deleting student with NumEtud ana hna f deleteetudusecase sff rah daz");


        return true; // Return true to indicate successful deletion
    }


    // Authorization check to verify if the user has the right role to perform the delete operation
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}