
using System.Globalization;
using System.Text;
using CsvHelper;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.NoteException;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.NoteUseCases.Export;

public class ExportNotesCsvUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<string> ExecuteAsync(long ueId)
    {
        var ue = await repositoryFactory.UeRepository().FindAsync(ueId);
        if (ue == null) throw new CsvProcessingException("UE non trouvée.");

        var etudiants = await repositoryFactory.EtudiantRepository().GetEtudiantsByUeIdAsync(ueId);
        if (etudiants == null || !etudiants.Any()) throw new CsvProcessingException("Aucun étudiant trouvé pour cette UE.");

        using var writer = new StringWriter();
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        csv.WriteField("Nom");
        csv.WriteField("Prenom");
        csv.WriteField("Note");
        csv.NextRecord();

        foreach (var etudiant in etudiants)
        {
            csv.WriteField(etudiant.Nom);
            csv.WriteField(etudiant.Prenom);
            csv.WriteField("");  // Colonne vide pour la saisie des notes
            csv.NextRecord();
        }

        return writer.ToString();
    }

    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Scolarite);
    }
}