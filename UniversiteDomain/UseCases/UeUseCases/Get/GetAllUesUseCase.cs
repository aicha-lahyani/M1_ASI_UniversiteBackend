using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.UeUseCases.Get;


public class GetAllUesUseCase(IRepositoryFactory repository)
{
    
    public async Task<List<Ue>> ExecuteAsync()
    {
        // Ensure the input is a valid student number (non-zero)
        

        // Search for the Etudiant by their NumEtud
        var Ue = await repository.UeRepository().FindAllAsync();

        // If no Etudiant is found, throw an exception
            
        
        

        return Ue; // Return true to indicate successful deletion
    }


    // Authorization check to verify if the user has the right role to perform the delete operation
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}