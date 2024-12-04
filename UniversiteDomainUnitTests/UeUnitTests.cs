namespace UniversiteDomainUnitTests;

using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.ParcoursUseCases.UeDansParcours;
using UniversiteDomain.UseCases.UeUseCases.Create;



    public class UeUnitTests
    {
        private Mock<IUeRepository> mockUeRepository;
        private Mock<IRepositoryFactory> mockFactory;

        [SetUp]
        public void Setup()
        {
            mockUeRepository = new Mock<IUeRepository>();
            mockFactory = new Mock<IRepositoryFactory>();
        }

        [Test]
        public async Task CreateUeUseCase()
        {
            long idUe = 1;
            string numeroUe = "UE 2";
            string intitule = "BD22";
            
            Ue ueAvant = new Ue { NumeroUe = numeroUe, Intitule = intitule };
            Ue ueFinal = new Ue { Id = idUe, NumeroUe = numeroUe, Intitule = intitule };

            mockUeRepository.Setup(repo => repo.FindByConditionAsync(p => p.NumeroUe == numeroUe))
                .ReturnsAsync(new List<Ue>());
            mockUeRepository.Setup(repo => repo.CreateAsync(ueAvant)).ReturnsAsync(ueFinal);
            mockFactory.Setup(facto => facto.UeRepository()).Returns(mockUeRepository.Object);

            var useCase = new CreateUeUseCase(mockFactory.Object);
            var ueTeste = await useCase.ExecuteAsync(ueAvant);

            Assert.That(ueTeste.Id, Is.EqualTo(ueFinal.Id));
            Assert.That(ueTeste.NumeroUe, Is.EqualTo(ueFinal.NumeroUe));
            Assert.That(ueTeste.Intitule, Is.EqualTo(ueFinal.Intitule));
        }
        [Test]
        
        public async Task AddUeDansParcoursUseCase()
{
    // Données de test
    long idUe = 1;
    long idParcours = 3;

    // Création d'un parcours et d'une UE
    Ue ue = new Ue { Id = idUe, NumeroUe = "UE101", Intitule = "Programmation Avancée" };
    Parcours parcours = new Parcours
    {
        Id = idParcours,
        NomParcours = "Parcours Informatique",
        AnneeFormation = 1,
        UesEnseignees = new List<Ue>() // Assurez que la liste est vide
    };
// Simulation des faux repositories
    var mockUeRepository = new Mock<IUeRepository>();
    var mockParcoursRepository = new Mock<IParcoursRepository>();

    // Simuler la recherche d'une UE existante
    mockUeRepository
        .Setup(repo => repo.FindByConditionAsync(u => u.Id.Equals(idUe)))
        .ReturnsAsync(new List<Ue> { ue });

    // Simuler la recherche d'un parcours existant
    mockParcoursRepository
        .Setup(repo => repo.FindByConditionAsync(p => p.Id.Equals(idParcours)))
        .ReturnsAsync(new List<Parcours> { parcours });

    // Simuler l'ajout d'une UE dans le parcours
    Parcours parcoursFinal = new Parcours
    {
        Id = idParcours,
        NomParcours = parcours.NomParcours,
        AnneeFormation = parcours.AnneeFormation,
        UesEnseignees = new List<Ue> { ue } // L'UE est ajoutée ici
    };

    mockParcoursRepository
        .Setup(repo => repo.AddUeAsync(idParcours, idUe))
        .ReturnsAsync(parcoursFinal);

    // Création d'une fausse factory contenant les faux repositories
    var mockFactory = new Mock<IRepositoryFactory>();
    mockFactory.Setup(f => f.UeRepository()).Returns(mockUeRepository.Object);
    mockFactory.Setup(f => f.ParcoursRepository()).Returns(mockParcoursRepository.Object);

    // Création du use case
    var useCase = new AddUeDansParcoursUseCase(mockFactory.Object);

    // Appel du use case
    var parcoursTest = await useCase.ExecuteAsync(idParcours, idUe);

    // Vérification des résultats
    Assert.That(parcoursTest.Id, Is.EqualTo(parcoursFinal.Id));
    Assert.That(parcoursTest.UesEnseignees, Is.Not.Null);
    Assert.That(parcoursTest.UesEnseignees.Count, Is.EqualTo(1));
    Assert.That(parcoursTest.UesEnseignees[0].Id, Is.EqualTo(idUe));
    Assert.That(parcoursTest.UesEnseignees[0].NumeroUe, Is.EqualTo(ue.NumeroUe));
}

    }
    
