using UniversiteDomain.Entities;

namespace UniversiteDomain.Dtos;

public class UeDto
{
    public long Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public string Intitule { get; set; } = string.Empty;

    // Constructeur par défaut - SANS throw !
    public UeDto()
    {
    }

    // Constructeur avec paramètres (si tu en as besoin)
    public UeDto(string intitule, string numero)
    {
        Intitule = intitule;
        Numero = numero;
    }

    public UeDto ToDto(Ue ue)
    {
        this.Id = ue.Id;
        this.Numero = ue.Numero;
        this.Intitule = ue.Intitule;
        return this;
    }
    
    public Ue ToEntity()
    {
        return new Ue { Id = this.Id, Numero = this.Numero, Intitule = this.Intitule };
    }
}