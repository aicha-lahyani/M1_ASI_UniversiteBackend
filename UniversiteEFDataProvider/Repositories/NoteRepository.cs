using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

public class NoteRepository(UniversiteDbContext context) : Repository<Note>(context), INoteRepository
{
    // Ajoute ou met à jour une note pour un étudiant dans une UE
    public async Task AjouterOuMettreAJourNoteAsync(long idEtudiant, long idUe, long valeurNote)
    {
        ArgumentNullException.ThrowIfNull(Context.Notes);
        ArgumentNullException.ThrowIfNull(Context.Etudiants);
        ArgumentNullException.ThrowIfNull(Context.Ues);

        // Vérifie si une note existe déjà pour cet étudiant et cette UE
        var note = await Context.Notes.FindAsync(idEtudiant, idUe);
        if (note != null)
        {
            // Met à jour la note existante
            note.Valeur = valeurNote;
        }
        else
        {
            // Crée une nouvelle note
            note = new Note
            {
                EtudiantId= idEtudiant,
                UeId = idUe,
                Valeur = valeurNote
            };
            Context.Notes.Add(note);
        }
        
        await Context.SaveChangesAsync();
    }

    // Récupère toutes les notes d'un étudiant
    public async Task<List<Note>> ObtenirNotesParEtudiantAsync(long idEtudiant)
    {
        ArgumentNullException.ThrowIfNull(Context.Notes);

        return await Context.Notes
            .Where(n => n.EtudiantId == idEtudiant)
            .ToListAsync();
    }
}