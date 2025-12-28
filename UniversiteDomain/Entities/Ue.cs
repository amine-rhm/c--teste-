namespace UniversiteDomain.Entities;

public class Ue
{
    public long Id { get; set; }
    public string Numero { get; set; } = String.Empty;
    public string Intitule { get; set; } = String.Empty;
    // ManyToMany : une Ue est enseignée dnas plusieurs parcours
    public List<Parcours>? EnseigneeDans { get; set; } = new();
    public List<Note>? Notes { get; set; } = new();
    
    public override string ToString()
    {
        return "ID "+Id +" : "+Numero+" - "+Intitule;
    }
}