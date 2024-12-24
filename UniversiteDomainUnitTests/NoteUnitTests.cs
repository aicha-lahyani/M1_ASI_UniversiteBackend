using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.NoteUseCases;

namespace UniversiteDomainUnitTests;

public class NoteUnitTests
{
    [Test]
    public async Task AjouterNote_ValidInput_Success()
    {
        // Arrange : préparation des données et des mocks
        long etudiantId = 1;
        long ueId = 1;
        long noteValeur = 15;

        var etudiant = new Etudiant
        {
            Id = etudiantId,
            Nom = "Dupont",
            Prenom = "Marie",
            ParcoursSuivi = new Parcours
            {
                Id = 1,
                NomParcours = "Informatique",
                UesEnseignees = new List<Ue>
                {
                    new Ue { Id = ueId, Intitule = "Programmation" }
                }
            },
            Notes = new List<Note>() // Aucun note initialement
        };

        var ue = etudiant.ParcoursSuivi.UesEnseignees.First();

        // Mock du repository étudiant
        var mockEtudiantRepo = new Mock<IEtudiantRepository>();
        mockEtudiantRepo.Setup(repo => repo.FindAsync(etudiantId)).ReturnsAsync(etudiant);
        mockEtudiantRepo.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Mock du repository UE
        var mockUeRepo = new Mock<IUeRepository>();
        mockUeRepo.Setup(repo => repo.FindAsync(ueId)).ReturnsAsync(ue);

        // Création du use case
        var useCase = new AjouterNoteUseCase(mockEtudiantRepo.Object, mockUeRepo.Object);

        // Act : appel du use case
        var note = await useCase.ExecuteAsync(etudiantId, ueId, noteValeur);

        // Assert : vérification des résultats
        Assert.NotNull(note); // La note ne doit pas être null
        Assert.AreEqual(noteValeur, note.Valeur); // La valeur de la note est correcte
        Assert.AreEqual(etudiant.Id, note.Etudiant.Id); // La note est liée à l'étudiant correct
        Assert.AreEqual(ue.Id, note.Ue.Id); // La note est liée à la bonne UE

        // Vérification que SaveChangesAsync a bien été appelé
        mockEtudiantRepo.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

}