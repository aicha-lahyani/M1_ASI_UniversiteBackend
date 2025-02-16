using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.UeUseCases.Delete;
public class DeleteUeUseCase(IRepositoryFactory repository)
{
    public async Task<bool> ExecuteAsync(long id)
    {
        // Ensure the input is a valid student number (non-zero)
        

        // Search for the Etudiant by their NumEtud

        // If no Etudiant is found, throw an exception

        await repository.UeRepository().DeleteAsync(id);

        await repository.UeRepository().SaveChangesAsync();


        return true;
        // Return true to indicate successful deletion
    }


    // Authorization check to verify if the user has the right role to perform the delete operation
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}