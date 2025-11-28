namespace UniversiteDomain.Entities;

public class Note
{
    public long Id { get; set; }
    public float Valeur { get; set; }
    
    // Clés étrangères
    public long EtudiantId { get; set; }
    public long UeId { get; set; }
    
    // Navigation
    public Etudiant? Etudiant { get; set; }
    public Ue? Ue { get; set; }
    
    public override string ToString()
    {
        return $"Note {Id} : Etudiant {EtudiantId} - UE {UeId} = {Valeur}/20";
    }
}