using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.NoteException;

namespace UniversiteDomain.UseCases.NoteUseCases;

public class AjouterNoteUseCase(IEtudiantRepository etudiantRepository, IUeRepository ueRepository)
{
    
    public async Task<Note> ExecuteAsync(long etudiantId, long ueId, long valeurNote)
    {
        var etudiant = await etudiantRepository.FindAsync(etudiantId);
        if (etudiant == null)
            throw new ArgumentNullException(nameof(etudiant), "Étudiant introuvable.");

        var ue = await ueRepository.FindAsync(ueId);
        if (ue == null)
            throw new ArgumentNullException(nameof(ue), "Unité d'enseignement introuvable.");

        await CheckBusinessRules(etudiant, ue, valeurNote);

        var note = new Note
        {
            Etudiant = etudiant,
            Ue = ue,
            Valeur = valeurNote
        };

        etudiant.Notes.Add(note);
        await etudiantRepository.SaveChangesAsync();

        return note;
    }

    private async Task CheckBusinessRules(Etudiant etudiant, Ue ue, double valeurNote)
    {
        // Vérifier que l'UE appartient au parcours de l'étudiant
        if (!etudiant.ParcoursSuivi.UesEnseignees.Any(u => u.Id == ue.Id))
            throw new UeNotInParcoursException($"L'UE {ue.Intitule} n'appartient pas au parcours de l'étudiant {etudiant.Nom}.");

        // Vérifier que l'étudiant n'a pas déjà une note pour cette UE
        if (etudiant.Notes.Any(n => n.Ue.Id == ue.Id))
            throw new DuplicateNoteForUeException($"Une note pour l'UE {ue.Intitule} existe déjà pour l'étudiant {etudiant.Nom}.");

        // Vérifier que la note est dans l'intervalle [0, 20]
        if (valeurNote < 0 || valeurNote > 20)
            throw new InvalidNoteException($"La note {valeurNote} est invalide. Elle doit être comprise entre 0 et 20.");
    }
}
