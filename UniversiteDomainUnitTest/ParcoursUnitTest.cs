using System.Linq.Expressions;
using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.ParcoursUseCase.Create;
using UniversiteDomain.UseCases.ParcoursUseCase.EtudiantDansParcours;

namespace UniversiteDomainUnitTest;

public class ParcoursUnitTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task CreateParcoursUseCase()
    {
        long idParcours = 1;
        string nomParcours = "Parcours 1";
        int anneeFormation = 2;

        var parcoursAvant = new Parcours { NomParcours = nomParcours, AnneeFormation = anneeFormation };

        // Mock du repository
        var mockParcours = new Mock<IParcoursRepository>();

        // Simule FindByConditionAsync pour retourner une liste vide (aucun doublon)
        mockParcours.Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Parcours, bool>>>()))
            .ReturnsAsync(new List<Parcours>());

        // Simule CreateAsync pour renvoyer un parcours avec ID
        Parcours parcoursFinal = new Parcours
            { Id = idParcours, NomParcours = nomParcours, AnneeFormation = anneeFormation };
        mockParcours.Setup(repo => repo.CreateAsync(It.IsAny<Parcours>())).ReturnsAsync(parcoursFinal);

        // Factory
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.ParcoursRepository()).Returns(mockParcours.Object);

        // Use case
        CreateParcoursUseCase useCase = new CreateParcoursUseCase(mockFactory.Object);

        // Exécution
        Parcours parcoursTeste = await useCase.ExecuteAsync(parcoursAvant);

        // Vérifications
        Assert.That(parcoursTeste.Id, Is.EqualTo(parcoursFinal.Id));
        Assert.That(parcoursTeste.NomParcours, Is.EqualTo(parcoursFinal.NomParcours));
        Assert.That(parcoursTeste.AnneeFormation, Is.EqualTo(parcoursFinal.AnneeFormation));
    }

    [Test]
    public async Task AddEtudiantDansParcoursUseCase()
    {
        long idEtudiant = 1;
        long idParcours = 3;

        // Parcours et étudiant
        Parcours parcours = new Parcours
        {
            Id = idParcours,
            NomParcours = "Parcours 3",
            AnneeFormation = 1,
            Inscrits = new List<Etudiant>()
        };

        Etudiant etudiant = new Etudiant
        {
            Id = idEtudiant,
            NumEtud = "1",
            Nom = "nom1",
            Prenom = "prenom1",
            Email = "test@test.fr",
            ParcoursSuivi = null // Pas encore inscrit !
        };

        // Mock EtudiantRepository
        var mockEtudiant = new Mock<IEtudiantRepository>();

        // Premier appel : l'étudiant existe
        // Troisième appel : l'étudiant n'est PAS déjà inscrit (liste vide)
        mockEtudiant.SetupSequence(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Etudiant, bool>>>()))
            .ReturnsAsync(new List<Etudiant> { etudiant }) // 1er appel : étudiant existe
            .ReturnsAsync(new List<Etudiant>()); // 3ème appel : pas encore inscrit

        // Mock ParcoursRepository
        var mockParcours = new Mock<IParcoursRepository>();
        mockParcours.Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Parcours, bool>>>()))
            .ReturnsAsync(new List<Parcours> { parcours });

        // Parcours final avec l'étudiant ajouté
        Parcours parcoursFinal = new Parcours
        {
            Id = idParcours,
            NomParcours = "Parcours 3",
            AnneeFormation = 1,
            Inscrits = new List<Etudiant> { etudiant }
        };

        mockParcours.Setup(repo => repo.AddEtudiantAsync(idParcours, idEtudiant))
            .ReturnsAsync(parcoursFinal);

        // Use case
        AddEtudiantDansParcoursUseCase useCase = new AddEtudiantDansParcoursUseCase(
            mockEtudiant.Object,
            mockParcours.Object
        );

        // Exécution
        var parcoursTest = await useCase.ExecuteAsync(idParcours, idEtudiant);

        // Vérifications
        Assert.That(parcoursTest.Id, Is.EqualTo(parcoursFinal.Id));
        Assert.That(parcoursTest.Inscrits, Is.Not.Null);
        Assert.That(parcoursTest.Inscrits.Count, Is.EqualTo(1));
        Assert.That(parcoursTest.Inscrits[0].Id, Is.EqualTo(idEtudiant));
    }
}