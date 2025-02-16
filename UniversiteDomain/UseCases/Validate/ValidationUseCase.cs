using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.NoteUseCases.Validate
{
    public class ValidationUseCase(IRepositoryFactory repositoryFactory)
    {
        public async Task<List<string>> ValidateAsync(IEnumerable<NoteCsvDto> records, long ueId)
        {
            var errors = new List<string>();

            // Vérifier si l'UE existe
            var ue = await repositoryFactory.UeRepository().FindAsync(ueId);
            if (ue == null)
            {
                errors.Add($"L'UE avec l'ID {ueId} n'existe pas.");
                return errors;
            }

            foreach (var record in records)
            {
                

                // Vérifier si l'étudiant existe dans la base
                var etudiants = await repositoryFactory.EtudiantRepository().FindByConditionAsync(e=>e.Nom == record.Nom);
                Etudiant etudiant = etudiants.FirstOrDefault();
                if (etudiant == null)
                {
                    errors.Add($"L'étudiant avec le numéro {record.Nom} n'existe pas.");
                    continue;
                }

                // Validation de la note
                float? noteValue = record.Note;

                if (noteValue < 0 || noteValue > 20)
                {
                    errors.Add($"Note hors plage (0-20) pour l'étudiant {record.Nom}.");
                }
            }

            return errors;
        }
    }
}