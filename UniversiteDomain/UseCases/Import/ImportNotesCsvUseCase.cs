using System.Globalization;
using CsvHelper;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.NoteException;
using UniversiteDomain.UseCases.NoteUseCases.Validate;

namespace UniversiteDomain.UseCases.NoteUseCases.Import;

public class ImportNotesCsvUseCase(IRepositoryFactory repositoryFactory, ValidationUseCase validate)
{
    public async Task ExecuteAsync(Stream csvStream, long ueId)
    {
        using var reader = new StreamReader(csvStream);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var records = csv.GetRecords<NoteCsvDto>().ToList();

        // Validation des données
        var validationErrors = await validate.ValidateAsync(records, ueId);
        if (validationErrors.Any())
        {
            throw new CsvProcessingException(string.Join("; ", validationErrors));
        }

        // Enregistrement des notes
        foreach (var record in records)
        {
            

            // ✅ Récupérer l'étudiant avec `.FirstOrDefault()`
            var etudiantList = await repositoryFactory.EtudiantRepository()
                .FindByConditionAsync(e => e.Nom == record.Nom);

            var etudiant = etudiantList.FirstOrDefault();

            if (etudiant == null)
            {
                Console.WriteLine($"❌ L'étudiant avec le numéro {record.Nom} n'existe pas.");
                throw new CsvProcessingException($"L'étudiant avec le numéro {record.Nom} n'existe pas.");
            }

            // ✅ Vérifier si l'UE existe
            var ue = await repositoryFactory.UeRepository().FindAsync(ueId);
            if (ue == null)
            {
                Console.WriteLine($"❌ L'UE avec l'ID {ueId} n'existe pas.");
                throw new CsvProcessingException($"L'UE avec l'ID {ueId} n'existe pas.");
            }

            // ✅ Créer et sauvegarder la note
            var note = new Note
            {
                EtudiantId = etudiant.Id,
                UeId = ueId,
                Valeur = record.Note.Value
            };

            await repositoryFactory.NoteRepository().SaveOrUpdateAsync(note);
        }

        Console.WriteLine("✅ Importation des notes terminée avec succès !");
    }

    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Scolarite);
    }
}