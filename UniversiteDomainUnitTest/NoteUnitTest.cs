using System.Linq.Expressions;
using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.NoteExceptions;
using UniversiteDomain.UseCases.NoteUseCase.Add;

namespace UniversiteDomainUnitTest;

public class NoteUnitTest
{
    [Test]
    public async Task CreateNote_Success()
    {
        long idEtudiant = 1;
        long idUe = 1;
        float valeur = 15.5f;

        Ue ue = new Ue { Id = idUe, Numero = "UE101", Intitule = "Maths" };
        Parcours parcours = new Parcours 
        { 
            Id = 1, 
            NomParcours = "Info", 
            AnneeFormation = 1,
            UesEnseignees = new List<Ue> { ue }
        };
        Etudiant etudiant = new Etudiant 
        { 
            Id = idEtudiant, 
            NumEtud = "ET1", 
            Nom = "Dupont", 
            Prenom = "Jean",
            Email = "jean@test.fr",
            ParcoursSuivi = parcours
        };

        Note noteCreee = new Note { Id = 1, EtudiantId = idEtudiant, UeId = idUe, Valeur = valeur };

        // Mocks
        var mockEtudiant = new Mock<IEtudiantRepository>();
        mockEtudiant.Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Etudiant, bool>>>()))
                    .ReturnsAsync(new List<Etudiant> { etudiant });

        var mockUe = new Mock<IUeRepository>();
        mockUe.Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
              .ReturnsAsync(new List<Ue> { ue });

        var mockNote = new Mock<INoteRepository>();
        mockNote.Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Note, bool>>>()))
                .ReturnsAsync(new List<Note>());
        mockNote.Setup(r => r.CreateAsync(It.IsAny<Note>())).ReturnsAsync(noteCreee);

        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.EtudiantRepository()).Returns(mockEtudiant.Object);
        mockFactory.Setup(f => f.UeRepository()).Returns(mockUe.Object);
        mockFactory.Setup(f => f.NoteRepository()).Returns(mockNote.Object);

        var useCase = new CreateNoteUseCase(mockFactory.Object);

        var result = await useCase.ExecuteAsync(idEtudiant, idUe, valeur);

        Assert.That(result.Valeur, Is.EqualTo(valeur));
        Assert.That(result.EtudiantId, Is.EqualTo(idEtudiant));
        Assert.That(result.UeId, Is.EqualTo(idUe));
    }

    [Test]
    public void CreateNote_ValeurInvalide_Throws()
    {
        var mockFactory = new Mock<IRepositoryFactory>();
        var useCase = new CreateNoteUseCase(mockFactory.Object);

        Assert.ThrowsAsync<InvalidNoteValeurException>(() => useCase.ExecuteAsync(1, 1, 25f));
        Assert.ThrowsAsync<InvalidNoteValeurException>(() => useCase.ExecuteAsync(1, 1, -5f));
    }

    [Test]
    public void CreateNote_Duplicate_Throws()
    {
        long idEtudiant = 1;
        long idUe = 1;

        Ue ue = new Ue { Id = idUe, Numero = "UE101", Intitule = "Maths" };
        Parcours parcours = new Parcours 
        { 
            Id = 1, 
            UesEnseignees = new List<Ue> { ue }
        };
        Etudiant etudiant = new Etudiant { Id = idEtudiant, ParcoursSuivi = parcours };
        Note noteExistante = new Note { Id = 1, EtudiantId = idEtudiant, UeId = idUe, Valeur = 10 };

        var mockEtudiant = new Mock<IEtudiantRepository>();
        mockEtudiant.Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Etudiant, bool>>>()))
                    .ReturnsAsync(new List<Etudiant> { etudiant });

        var mockUe = new Mock<IUeRepository>();
        mockUe.Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
              .ReturnsAsync(new List<Ue> { ue });

        var mockNote = new Mock<INoteRepository>();
        mockNote.Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Note, bool>>>()))
                .ReturnsAsync(new List<Note> { noteExistante });

        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.EtudiantRepository()).Returns(mockEtudiant.Object);
        mockFactory.Setup(f => f.UeRepository()).Returns(mockUe.Object);
        mockFactory.Setup(f => f.NoteRepository()).Returns(mockNote.Object);

        var useCase = new CreateNoteUseCase(mockFactory.Object);

        Assert.ThrowsAsync<DuplicateNoteException>(() => useCase.ExecuteAsync(idEtudiant, idUe, 15f));
    }

    [Test]
    public void CreateNote_UeNotInParcours_Throws()
    {
        long idEtudiant = 1;
        long idUe = 99;

        Ue ueAutre = new Ue { Id = 1, Numero = "UE101", Intitule = "Maths" };
        Ue ueDemandee = new Ue { Id = idUe, Numero = "UE999", Intitule = "Autre" };
        Parcours parcours = new Parcours 
        { 
            Id = 1, 
            UesEnseignees = new List<Ue> { ueAutre }  // L'UE demandée n'est pas dans le parcours
        };
        Etudiant etudiant = new Etudiant { Id = idEtudiant, ParcoursSuivi = parcours };

        var mockEtudiant = new Mock<IEtudiantRepository>();
        mockEtudiant.Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Etudiant, bool>>>()))
                    .ReturnsAsync(new List<Etudiant> { etudiant });

        var mockUe = new Mock<IUeRepository>();
        mockUe.Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
              .ReturnsAsync(new List<Ue> { ueDemandee });

        var mockNote = new Mock<INoteRepository>();
        mockNote.Setup(r => r.FindByConditionAsync(It.IsAny<Expression<Func<Note, bool>>>()))
                .ReturnsAsync(new List<Note>());

        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.EtudiantRepository()).Returns(mockEtudiant.Object);
        mockFactory.Setup(f => f.UeRepository()).Returns(mockUe.Object);
        mockFactory.Setup(f => f.NoteRepository()).Returns(mockNote.Object);

        var useCase = new CreateNoteUseCase(mockFactory.Object);

        Assert.ThrowsAsync<UeNotInParcoursException>(() => useCase.ExecuteAsync(idEtudiant, idUe, 15f));
    }
}