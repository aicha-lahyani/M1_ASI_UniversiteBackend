using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Exceptions.ParcoursExceptions;
using UniversiteDomain.Util;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Update;

public class UpdateEtudiantUseCase(IRepositoryFactory factory)
{
    public async Task<Etudiant> ExecuteAsync(Etudiant etudiant)
    {
        await CheckBusinessRules(etudiant);
        
        // Récupérer l'étudiant existant
        var existingEtudiant = await factory.EtudiantRepository().FindAsync(etudiant.Id);
        await factory.EtudiantRepository().UpdateAsync(existingEtudiant);
        await factory.SaveChangesAsync();
        return existingEtudiant;
    }

    private async Task CheckBusinessRules(Etudiant etudiant)
    {
        ArgumentNullException.ThrowIfNull(etudiant);
        ArgumentNullException.ThrowIfNull(etudiant.NumEtud);
        ArgumentNullException.ThrowIfNull(etudiant.Email);
        ArgumentNullException.ThrowIfNull(factory);

        // Vérifier si l'étudiant existe
        var existingEtudiant = await factory.EtudiantRepository().FindAsync(etudiant.Id);
        if (existingEtudiant == null)
            throw new EtudiantNotFoundException($"Étudiant avec ID {etudiant.Id} non trouvé");

        // Vérifier si le nouveau numéro étudiant n'est pas déjà utilisé (sauf par l'étudiant lui-même)
        var duplicateNumEtud = await factory.EtudiantRepository()
            .FindByConditionAsync(e => e.NumEtud.Equals(etudiant.NumEtud) && e.Id != etudiant.Id);
        if (duplicateNumEtud.Any())
            throw new DuplicateNumEtudException($"{etudiant.NumEtud} - ce numéro d'étudiant est déjà affecté à un étudiant");

        // Vérifier le format de l'email
        if (!CheckEmail.IsValidEmail(etudiant.Email))
            throw new InvalidEmailException($"{etudiant.Email} - Email mal formé");

        // Vérifier si le nouvel email n'est pas déjà utilisé (sauf par l'étudiant lui-même)
        var duplicateEmail = await factory.EtudiantRepository()
            .FindByConditionAsync(e => e.Email.Equals(etudiant.Email) && e.Id != etudiant.Id);
        if (duplicateEmail.Any())
            throw new DuplicateEmailException($"{etudiant.Email} est déjà affecté à un étudiant");

        // Vérifier la longueur du nom
        if (etudiant.Nom.Length < 3)
            throw new InvalidNomEtudiantException($"{etudiant.Nom} incorrect - Le nom d'un étudiant doit contenir plus de 3 caractères");
    }

    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}