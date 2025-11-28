using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.UeUseCase;

namespace UniversiteDomainUnitTests;

public class UeUnitTest
{
    [Test]
    public async Task CreateUe_Success()
    {
        // Données de test
        string numero = "UE101";
        string intitule = "Mathematiques";
        Ue ueSansId = new Ue { Numero = numero, Intitule = intitule };
        Ue ueCree = new Ue { Id = 1, Numero = numero, Intitule = intitule };

        // Mock du repository
        var mockRepo = new Mock<IUeRepository>();
        mockRepo.Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
                .ReturnsAsync(new List<Ue>()); // Pas de doublon
        mockRepo.Setup(r => r.CreateAsync(It.IsAny<Ue>())).ReturnsAsync(ueCree);

        var useCase = new CreateUeUseCase(mockRepo.Object);

        // Exécution
        var result = await useCase.ExecuteAsync(ueSansId);

        // Vérifications
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
}