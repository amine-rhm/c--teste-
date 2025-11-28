namespace UniversiteDomain.Entities
{
    public class Etudiant
    {
        public long Id { get; set; }
        public string NumEtud { get; set; } = string.Empty;
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        
        // Navigation vers le parcours
        public Parcours? Parcours { get; set; }
        // Un étudiant a plusieurs notes
        public List<Note> Notes { get; set; } = new();
        public override string ToString()
        {
            return $"ID {Id} : {NumEtud} - {Nom} {Prenom} inscrit en {Parcours?.NomParcours ?? "Aucun parcours"}";
        }
    }
}