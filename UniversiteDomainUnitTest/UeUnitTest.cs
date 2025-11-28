using System.Linq.Expressions;
using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.UeExceptions;
using UniversiteDomain.UseCases.ParcoursUseCase.Add;
using UniversiteDomain.UseCases.UeUseCase;

namespace UniversiteDomainUnitTest;

public class UeUnitTest
{
    // ========== Tests CreateUeUseCase ==========

    [Test]
    public async Task CreateUe_Success()
    {
        string numero = "UE101";
        string intitule = "Mathematiques";
        Ue ueSansId = new Ue { Numero = numero, Intitule = intitule };
        Ue ueCree = new Ue { Id = 1, Numero = numero, Intitule = intitule };

        var mockRepo = new Mock<IUeRepository>();
        mockRepo.Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
                .ReturnsAsync(new List<Ue>());
        mockRepo.Setup(r => r.CreateAsync(It.IsAny<Ue>())).ReturnsAsync(ueCree);

        var useCase = new CreateUeUseCase(mockRepo.Object);

        var result = await useCase.ExecuteAsync(ueSansId);

        Assert.That(result.Id, Is.EqualTo(ueCree.Id));
        Assert.That(result.Numero, Is.EqualTo(ueCree.Numero));
        Assert.That(result.Intitule, Is.EqualTo(ueCree.Intitule));
    }

    [Test]
    public void CreateUe_DuplicateNumero_Throws()
    {
        string numero = "UE101";
        string intitule = "Physique";
        Ue ue = new Ue { Numero = numero, Intitule = intitule };

        var mockRepo = new Mock<IUeRepository>();
        mockRepo.Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
                .ReturnsAsync(new List<Ue> { new Ue { Numero = numero } });

        var useCase = new CreateUeUseCase(mockRepo.Object);

        Assert.ThrowsAsync<DuplicateUeNumeroException>(() => useCase.ExecuteAsync(ue));
    }

    [Test]
    public void CreateUe_IntituleTropCourt_Throws()
    {
        Ue ue = new Ue { Numero = "UE102", Intitule = "abc" };

        var mockRepo = new Mock<IUeRepository>();
        mockRepo.Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
                .ReturnsAsync(new List<Ue>());

        var useCase = new CreateUeUseCase(mockRepo.Object);

        Assert.ThrowsAsync<InvalidUeIntituleException>(() => useCase.ExecuteAsync(ue));
    }

    // ========== Tests AddUeDansParcoursUseCase ==========

    [Test]
    public async Task AddUeDansParcours_Success()
    {
        long idUe = 1;
        long idParcours = 2;

        Ue ue = new Ue { Id = idUe, Numero = "UE101", Intitule = "Mathematiques" };
        Parcours parcours = new Parcours 
        { 
            Id = idParcours, 
            NomParcours = "Parcours Info", 
            AnneeFormation = 1,
            UesEnseignees = new List<Ue>()
        };

        Parcours parcoursFinal = new Parcours 
        { 
            Id = idParcours, 
            NomParcours = "Parcours Info", 
            AnneeFormation = 1,
            UesEnseignees = new List<Ue> { ue }
        };

        var mockUe = new Mock<IUeRepository>();
        mockUe.Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
              .ReturnsAsync(new List<Ue> { ue });

        var mockParcours = new Mock<IParcoursRepository>();
        mockParcours.Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Parcours, bool>>>()))
                    .ReturnsAsync(new List<Parcours> { parcours });
        mockParcours.Setup(r => r.AddUeAsync(idParcours, idUe))
                    .ReturnsAsync(parcoursFinal);

        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.UeRepository()).Returns(mockUe.Object);
        mockFactory.Setup(f => f.ParcoursRepository()).Returns(mockParcours.Object);

        var useCase = new AddUeDansParcoursUseCase(mockFactory.Object);

        var result = await useCase.ExecuteAsync(idParcours, idUe);

        Assert.That(result.Id, Is.EqualTo(idParcours));
        if (result.UesEnseignees != null)
        {
            Assert.That(result.UesEnseignees.Count, Is.EqualTo(1));
            Assert.That(result.UesEnseignees[0].Id, Is.EqualTo(idUe));
        }
    }

    [Test]
    public void AddUeDansParcours_UeNotFound_Throws()
    {
        long idUe = 99;
        long idParcours = 1;

        var mockUe = new Mock<IUeRepository>();
        mockUe.Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
              .ReturnsAsync(new List<Ue>());

        var mockParcours = new Mock<IParcoursRepository>();

        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.UeRepository()).Returns(mockUe.Object);
        mockFactory.Setup(f => f.ParcoursRepository()).Returns(mockParcours.Object);

        var useCase = new AddUeDansParcoursUseCase(mockFactory.Object);

        Assert.ThrowsAsync<UeNotFoundException>(() => useCase.ExecuteAsync(idParcours, idUe));
    }

    [Test]
    public void AddUeDansParcours_Duplicate_Throws()
    {
        long idUe = 1;
        long idParcours = 2;

        Ue ue = new Ue { Id = idUe, Numero = "UE101", Intitule = "Mathematiques" };
        Parcours parcours = new Parcours 
        { 
            Id = idParcours, 
            NomParcours = "Parcours Info", 
            AnneeFormation = 1,
            UesEnseignees = new List<Ue> { ue }
        };

        var mockUe = new Mock<IUeRepository>();
        mockUe.Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
              .ReturnsAsync(new List<Ue> { ue });

        var mockParcours = new Mock<IParcoursRepository>();
        mockParcours.Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Parcours, bool>>>()))
                    .ReturnsAsync(new List<Parcours> { parcours });

        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.UeRepository()).Returns(mockUe.Object);
        mockFactory.Setup(f => f.ParcoursRepository()).Returns(mockParcours.Object);

        var useCase = new AddUeDansParcoursUseCase(mockFactory.Object);

        Assert.ThrowsAsync<DuplicateUeDansParcoursException>(() => useCase.ExecuteAsync(idParcours, idUe));
    }
}