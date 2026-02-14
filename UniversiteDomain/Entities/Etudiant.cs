namespace UniversiteDomain.Entities
{
    public class Etudiant
    {
        public long Id { get; set; }
        public string NumEtud { get; set; } = string.Empty;
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        
        // Navigation vers le parcours - UNE SEULE PROPRIÉTÉ
        public Parcours? ParcoursSuivi { get; set; }
        
        // Un étudiant a plusieurs notes - UNE SEULE PROPRIÉTÉ
        public List<Note>? NotesObtenues { get; set; } = new();

        public override string ToString()
        {
            return $"ID {Id} : {NumEtud} - {Nom} {Prenom} inscrit en {ParcoursSuivi?.NomParcours ?? "Aucun parcours"}";
        }
    }
}