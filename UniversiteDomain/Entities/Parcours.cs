namespace UniversiteDomain.Entities
{
    public class Parcours
    {
        public long Id { get; set; }

        public string NomParcours { get; set; } = string.Empty;
        public int AnneeFormation { get; set; }

        // Relation One-to-Many : un parcours peut avoir plusieurs étudiants
        public List<Etudiant> Inscrits { get; set; } = new();
        public List<Ue>? UesEnseignees { get; set; } = new();
        // Une Ue a plusieurs notes
        public List<Note> Notes { get; set; } = new();
        public override string ToString()
        {
            return $"ID {Id} : {NomParcours} - Année {AnneeFormation}";
        }
    }
}