using System.Globalization;
using System.Security.Claims;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.NoteUseCases.ExportCsv;
using UniversiteDomain.UseCases.NoteUseCases.ImportCsv;
using UniversiteDomain.UseCases.SecurityUseCases;

namespace UniversiteRestApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NoteController(IRepositoryFactory repositoryFactory) : ControllerBase
{
    // GET: api/Note/export-csv/4 - Télécharge le CSV des notes d'une UE
    [HttpGet("export-csv/{ueId}")]
    public async Task<IActionResult> ExportCsv(long ueId)
    {
        string role;
        string email;
        IUniversiteUser user;
        
        try
        {
            (role, email, user) = await CheckSecuAsync(HttpContext, repositoryFactory);
        }
        catch (Exception)
        {
            return Unauthorized();
        }
        
        ExportNotesUeCsvUseCase uc = new ExportNotesUeCsvUseCase(repositoryFactory);
        
        if (!uc.IsAuthorized(role))
            return Unauthorized();
        
        List<NoteCsvDto> notes;
        try
        {
            notes = await uc.ExecuteAsync(ueId);
        }
        catch (Exception e)
        {
            ModelState.AddModelError(nameof(e), e.Message);
            return ValidationProblem();
        }
        
        // Générer le fichier CSV
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";"
        };
        
        using var memoryStream = new MemoryStream();
        using (var writer = new StreamWriter(memoryStream, Encoding.UTF8))
        using (var csv = new CsvWriter(writer, config))
        {
            csv.WriteRecords(notes);
        }
        
        var csvBytes = memoryStream.ToArray();
        
        return File(csvBytes, "text/csv", $"notes_ue_{ueId}.csv");
    }
    
    // POST: api/Note/import-csv/4 - Upload le CSV rempli
    [HttpPost("import-csv/{ueId}")]
    public async Task<IActionResult> ImportCsv(long ueId, IFormFile file)
    {
        string role;
        string email;
        IUniversiteUser user;
        
        try
        {
            (role, email, user) = await CheckSecuAsync(HttpContext, repositoryFactory);
        }
        catch (Exception)
        {
            return Unauthorized();
        }
        
        ImportNotesUeCsvUseCase uc = new ImportNotesUeCsvUseCase(repositoryFactory);
        
        if (!uc.IsAuthorized(role))
            return Unauthorized();
        
        if (file == null || file.Length == 0)
            return BadRequest("Aucun fichier fourni");
        
        List<NoteCsvDto> notesCsv;
        
        try
        {
            // Lire le fichier CSV
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";"
            };
            
            using var reader = new StreamReader(file.OpenReadStream());
            using var csv = new CsvReader(reader, config);
            
            notesCsv = csv.GetRecords<NoteCsvDto>().ToList();
        }
        catch (Exception e)
        {
            return BadRequest($"Erreur lors de la lecture du fichier CSV : {e.Message}");
        }
        
        try
        {
            await uc.ExecuteAsync(notesCsv, ueId);
        }
        catch (Exception e)
        {
            ModelState.AddModelError(nameof(e), e.Message);
            return ValidationProblem();
        }
        
        return Ok($"{notesCsv.Count} notes ont été enregistrées avec succès");
    }
    
    // MÉTHODE DE SÉCURITÉ
    private async Task<(string role, string email, IUniversiteUser user)> CheckSecuAsync(
        HttpContext httpContext, 
        IRepositoryFactory factory)
    {
        ClaimsPrincipal claims = httpContext.User;

        if (claims.Identity?.IsAuthenticated != true)
            throw new UnauthorizedAccessException();

        var emailClaim = claims.FindFirst(ClaimTypes.Email);
        if (emailClaim == null)
            throw new UnauthorizedAccessException();

        string email = emailClaim.Value;
        if (string.IsNullOrEmpty(email))
            throw new UnauthorizedAccessException();

        var user = await new FindUniversiteUserByEmailUseCase(factory).ExecuteAsync(email);
        if (user == null)
            throw new UnauthorizedAccessException();

        var ident = claims.Identities.FirstOrDefault();
        if (ident == null)
            throw new UnauthorizedAccessException();

        var roleClaim = ident.FindFirst(ClaimTypes.Role);
        if (roleClaim == null)
            throw new UnauthorizedAccessException();

        string role = roleClaim.Value;
        if (string.IsNullOrEmpty(role))
            throw new UnauthorizedAccessException();

        bool isInRole = await new IsInRoleUseCase(factory).ExecuteAsync(email, role);
        if (!isInRole)
            throw new UnauthorizedAccessException();

        return (role, email, user);
    }
}