using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.UeUseCases.Get;


public class GetUeByIdUseCase(IRepositoryFactory repository)
{
    
    public async Task<Ue> ExecuteAsync(long UeId)
    {
        // Ensure the input is a valid student number (non-zero)
        if (UeId == 0)
        {
            throw new ArgumentException("Numéro d'étudiant is required", nameof(UeId));
        }

        // Search for the Etudiant by their NumEtud
        var Ue = await repository.UeRepository().FindAsync(UeId);
        Console.WriteLine(Ue);

        // If no Etudiant is found, throw an exception
            
        
        

        return Ue; // Return true to indicate successful deletion
    }


    // Authorization check to verify if the user has the right role to perform the delete operation
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}