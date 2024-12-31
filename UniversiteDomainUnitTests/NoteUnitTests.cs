using Moq;
using NUnit.Framework;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.NoteException;

namespace UniversiteDomainUnitTests;

[TestFixture]
public class AjouterNoteUseCaseTests
{
    [Test]
    public async Task AjouterNote_ValidInput_Success()
    {
        // Arrange
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
            Notes = new List<Note>()
        };

        var ue = etudiant.ParcoursSuivi.UesEnseignees.First();

        var mockFactory = new Mock<IRepositoryFactory>();
        var mockEtudiantRepo = new Mock<IEtudiantRepository>();
        var mockUeRepo = new Mock<IUeRepository>();

        mockEtudiantRepo.Setup(repo => repo.FindAsync(etudiantId)).ReturnsAsync(etudiant);
        mockEtudiantRepo.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);
        mockUeRepo.Setup(repo => repo.FindAsync(ueId)).ReturnsAsync(ue);

        mockFactory.Setup(f => f.EtudiantRepository()).Returns(mockEtudiantRepo.Object);
        mockFactory.Setup(f => f.UeRepository()).Returns(mockUeRepo.Object);

        var useCase = new CreateNoteUseCase(mockFactory.Object);

        // Act
        var note = await useCase.ExecuteAsync(etudiantId, ueId, noteValeur);

        // Assert
        Assert.NotNull(note);
        Assert.AreEqual(noteValeur, note.Valeur);
        Assert.AreEqual(etudiant.Id, note.Etudiant.Id);
        Assert.AreEqual(ue.Id, note.Ue.Id);
        mockEtudiantRepo.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public void AjouterNote_StudentNotFound_ThrowsException()
    {
        // Arrange
        var mockFactory = new Mock<IRepositoryFactory>();
        var mockEtudiantRepo = new Mock<IEtudiantRepository>();
        var mockUeRepo = new Mock<IUeRepository>();

        mockEtudiantRepo.Setup(repo => repo.FindAsync(It.IsAny<long>())).ReturnsAsync((Etudiant)null);
        mockFactory.Setup(f => f.EtudiantRepository()).Returns(mockEtudiantRepo.Object);

        var useCase = new CreateNoteUseCase(mockFactory.Object);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await useCase.ExecuteAsync(1, 1, 15));
    }

    [Test]
    public void AjouterNote_UeNotFound_ThrowsException()
    {
        // Arrange
        long etudiantId = 1;

        var etudiant = new Etudiant
        {
            Id = etudiantId,
            ParcoursSuivi = new Parcours { UesEnseignees = new List<Ue>() }
        };

        var mockFactory = new Mock<IRepositoryFactory>();
        var mockEtudiantRepo = new Mock<IEtudiantRepository>();
        var mockUeRepo = new Mock<IUeRepository>();

        mockEtudiantRepo.Setup(repo => repo.FindAsync(etudiantId)).ReturnsAsync(etudiant);
        mockUeRepo.Setup(repo => repo.FindAsync(It.IsAny<long>())).ReturnsAsync((Ue)null);

        mockFactory.Setup(f => f.EtudiantRepository()).Returns(mockEtudiantRepo.Object);
        mockFactory.Setup(f => f.UeRepository()).Returns(mockUeRepo.Object);

        var useCase = new CreateNoteUseCase(mockFactory.Object);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await useCase.ExecuteAsync(etudiantId, 1, 15));
    }

    [Test]
    public void AjouterNote_UeNotInParcours_ThrowsException()
    {
        // Arrange
        long etudiantId = 1;
        long ueId = 2; // UE non associée au parcours

        var etudiant = new Etudiant
        {
            Id = etudiantId,
            ParcoursSuivi = new Parcours
            {
                UesEnseignees = new List<Ue>
                {
                    new Ue { Id = 1, Intitule = "Programmation" }
                }
            },
            Notes = new List<Note>()
        };

        var ue = new Ue { Id = ueId, Intitule = "Mathématiques" };

        var mockFactory = new Mock<IRepositoryFactory>();
        var mockEtudiantRepo = new Mock<IEtudiantRepository>();
        var mockUeRepo = new Mock<IUeRepository>();

        mockEtudiantRepo.Setup(repo => repo.FindAsync(etudiantId)).ReturnsAsync(etudiant);
        mockUeRepo.Setup(repo => repo.FindAsync(ueId)).ReturnsAsync(ue);

        mockFactory.Setup(f => f.EtudiantRepository()).Returns(mockEtudiantRepo.Object);
        mockFactory.Setup(f => f.UeRepository()).Returns(mockUeRepo.Object);

        var useCase = new CreateNoteUseCase(mockFactory.Object);

        // Act & Assert
        Assert.ThrowsAsync<UeNotInParcoursException>(async () =>
            await useCase.ExecuteAsync(etudiantId, ueId, 15));
    }

    [Test]
    public void AjouterNote_DuplicateNoteForUe_ThrowsException()
    {
        // Arrange
        long etudiantId = 1;
        long ueId = 1;

        var etudiant = new Etudiant
        {
            Id = etudiantId,
            ParcoursSuivi = new Parcours
            {
                UesEnseignees = new List<Ue>
                {
                    new Ue { Id = ueId, Intitule = "Programmation" }
                }
            },
            Notes = new List<Note>
            {
                new Note { Ue = new Ue { Id = ueId }, Valeur = 10 }
            }
        };

        var mockFactory = new Mock<IRepositoryFactory>();
        var mockEtudiantRepo = new Mock<IEtudiantRepository>();
        var mockUeRepo = new Mock<IUeRepository>();

        mockEtudiantRepo.Setup(repo => repo.FindAsync(etudiantId)).ReturnsAsync(etudiant);
        mockFactory.Setup(f => f.EtudiantRepository()).Returns(mockEtudiantRepo.Object);

        var useCase = new CreateNoteUseCase(mockFactory.Object);

        // Act & Assert
        Assert.ThrowsAsync<DuplicateNoteForUeException>(async () =>
            await useCase.ExecuteAsync(etudiantId, ueId, 15));
    }

    [Test]
    public void AjouterNote_InvalidNoteValue_ThrowsException()
    {
        // Arrange
        long etudiantId = 1;
        long ueId = 1;

        var etudiant = new Etudiant
        {
            Id = etudiantId,
            ParcoursSuivi = new Parcours
            {
                UesEnseignees = new List<Ue>
                {
                    new Ue { Id = ueId, Intitule = "Programmation" }
                }
            },
            Notes = new List<Note>()
        };

        var ue = etudiant.ParcoursSuivi.UesEnseignees.First();

        var mockFactory = new Mock<IRepositoryFactory>();
        var mockEtudiantRepo = new Mock<IEtudiantRepository>();
        var mockUeRepo = new Mock<IUeRepository>();

        mockEtudiantRepo.Setup(repo => repo.FindAsync(etudiantId)).ReturnsAsync(etudiant);
        mockUeRepo.Setup(repo => repo.FindAsync(ueId)).ReturnsAsync(ue);

        mockFactory.Setup(f => f.EtudiantRepository()).Returns(mockEtudiantRepo.Object);
        mockFactory.Setup(f => f.UeRepository()).Returns(mockUeRepo.Object);

        var useCase = new CreateNoteUseCase(mockFactory.Object);

        // Act & Assert
        Assert.ThrowsAsync<InvalidNoteException>(async () =>
            await useCase.ExecuteAsync(etudiantId, ueId, 25)); // Note hors intervalle
    }
}
