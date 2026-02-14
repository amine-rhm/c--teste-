using System.Linq.Expressions;
using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.EtudiantUseCases.Create;

namespace UniversiteDomainUnitTest;

public class EtudiantUnitTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task CreateEtudiantUseCase()
    {
        long id = 1;
        string numEtud = "et1";
        string nom = "Durant";
        string prenom = "Jean";
        string email = "jean.durant@etud.u-picardie.fr";

        // On crée l'étudiant qui doit être ajouté en base
        Etudiant etudiantSansId = new Etudiant { NumEtud = numEtud, Nom = nom, Prenom = prenom, Email = email };

        // On initialise un faux EtudiantRepository
        var mockEtudiantRepo = new Mock<IEtudiantRepository>();

        // Simulation de la fonction FindByCondition
        var reponseFindByCondition = new List<Etudiant>();
        mockEtudiantRepo.Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Etudiant, bool>>>()))
            .ReturnsAsync(reponseFindByCondition);

        // Simulation de la fonction Create
        Etudiant etudiantCree = new Etudiant { Id = id, NumEtud = numEtud, Nom = nom, Prenom = prenom, Email = email };
        mockEtudiantRepo.Setup(repo => repo.CreateAsync(etudiantSansId)).ReturnsAsync(etudiantCree);

        // On initialise un faux RepositoryFactory
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.EtudiantRepository()).Returns(mockEtudiantRepo.Object);
        mockFactory.Setup(f => f.SaveChangesAsync()).Returns(Task.CompletedTask);

        var fauxRepositoryFactory = mockFactory.Object;

        // Création du use case en injectant notre faux factory
        CreateEtudiantUseCase useCase = new CreateEtudiantUseCase(fauxRepositoryFactory);

        // Appel du use case
        var etudiantTeste = await useCase.ExecuteAsync(etudiantSansId);

        // Vérification du résultat
        Assert.That(etudiantTeste.Id, Is.EqualTo(etudiantCree.Id));
        Assert.That(etudiantTeste.NumEtud, Is.EqualTo(etudiantCree.NumEtud));
        Assert.That(etudiantTeste.Nom, Is.EqualTo(etudiantCree.Nom));
        Assert.That(etudiantTeste.Prenom, Is.EqualTo(etudiantCree.Prenom));
        Assert.That(etudiantTeste.Email, Is.EqualTo(etudiantCree.Email));
    }
}