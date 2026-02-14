namespace UniversiteDomain.Dtos;

public class NoteCsvDto
{
    public string NumEtud { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string NumeroUe { get; set; } = string.Empty;
    public string IntituleUe { get; set; } = string.Empty;
    public string? Note { get; set; }  // String car peut être vide dans le CSV
}