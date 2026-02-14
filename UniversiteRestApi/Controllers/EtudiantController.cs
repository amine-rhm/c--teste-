using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.EtudiantUseCases.Create;
using UniversiteDomain.UseCases.EtudiantUseCases.Delete;
using UniversiteDomain.UseCases.EtudiantUseCases.Get;
using UniversiteDomain.UseCases.EtudiantUseCases.Update;
using UniversiteDomain.UseCases.SecurityUseCases;

namespace UniversiteRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EtudiantController(IRepositoryFactory repositoryFactory) : ControllerBase
    {
        // GET: api/Etudiant - Récupère tous les étudiants
        [HttpGet]
        public async Task<ActionResult<List<EtudiantDto>>> GetAsync()
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
            
            GetTousLesEtudiantsUseCase uc = new GetTousLesEtudiantsUseCase(repositoryFactory);
            if (!uc.IsAuthorized(role)) 
                return Unauthorized();
            
            List<Etudiant> etud;
            try
            {
                etud = await uc.ExecuteAsync();
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }
            
            return EtudiantDto.ToDtos(etud);
        }

        // GET api/Etudiant/5 - Récupère un étudiant par son ID
        [HttpGet("{id}")]
        public async Task<ActionResult<EtudiantDto>> GetUnEtudiant(long id)
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
            
            GetEtudiantByIdUseCase uc = new GetEtudiantByIdUseCase(repositoryFactory);
            
            if (!uc.IsAuthorized(role, user, id)) 
                return Unauthorized();
            
            Etudiant? etud;
            try
            {
                etud = await uc.ExecuteAsync(id);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }
           
            if (etud == null) 
                return NotFound();
            
            return new EtudiantDto().ToDto(etud);
        }

        // GET api/Etudiant/complet/5 - Récupère un étudiant COMPLET
        [HttpGet("complet/{id}")]
        public async Task<ActionResult<EtudiantCompletDto>> GetUnEtudiantCompletAsync(long id)
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
            
            GetEtudiantCompletUseCase uc = new GetEtudiantCompletUseCase(repositoryFactory);

            if (!uc.IsAuthorized(role, user, id)) 
                return Unauthorized();
            
            Etudiant? etud;
            try
            {
                etud = await uc.ExecuteAsync(id);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }
            
            if (etud == null) 
                return NotFound();
            
            return new EtudiantCompletDto().ToDto(etud);
        }

        // POST api/Etudiant - Crée un nouvel étudiant
        [HttpPost]
        public async Task<ActionResult<EtudiantDto>> PostAsync([FromBody] EtudiantDto etudiantDto)
        {
            CreateEtudiantUseCase createEtudiantUc = new CreateEtudiantUseCase(repositoryFactory);
            CreateUniversiteUserUseCase createUserUc = new CreateUniversiteUserUseCase(repositoryFactory);

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
            
            if (!createEtudiantUc.IsAuthorized(role) || !createUserUc.IsAuthorized(role)) 
                return Unauthorized();
            
            Etudiant etud = etudiantDto.ToEntity();
            
            try
            {
                etud = await createEtudiantUc.ExecuteAsync(etud);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }

            try
            {
                await createUserUc.ExecuteAsync(etud.Email, etud.Email, "Miage2025#", Roles.Etudiant, etud); 
            }
            catch (Exception e)
            {
                await new DeleteEtudiantUseCase(repositoryFactory).ExecuteAsync(etud.Id);
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }

            EtudiantDto dto = new EtudiantDto().ToDto(etud);
            return CreatedAtAction(nameof(GetUnEtudiant), new { id = dto.Id }, dto);
        }

        // PUT api/Etudiant/5 - Modifie un étudiant
        [HttpPut("{id}")]
        public async Task<ActionResult> PutAsync(long id, [FromBody] EtudiantDto etudiantDto)
        {
            UpdateEtudiantUseCase updateEtudiantUc = new UpdateEtudiantUseCase(repositoryFactory);
            UpdateUniversiteUserUseCase updateUserUc = new UpdateUniversiteUserUseCase(repositoryFactory);

            if (id != etudiantDto.Id)
            {
                return BadRequest();
            }
            
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
            
            if (!updateEtudiantUc.IsAuthorized(role) || !updateUserUc.IsAuthorized(role)) 
                return Unauthorized();
            
            try
            {
                await updateUserUc.ExecuteAsync(etudiantDto.ToEntity());
                await updateEtudiantUc.ExecuteAsync(etudiantDto.ToEntity());
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }
            
            return NoContent();
        }

        // DELETE api/Etudiant/5 - Supprime un étudiant
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(long id)
        {
            DeleteEtudiantUseCase etudiantUc = new DeleteEtudiantUseCase(repositoryFactory);
            DeleteUniversiteUserUseCase userUc = new DeleteUniversiteUserUseCase(repositoryFactory);
            
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

            if (!etudiantUc.IsAuthorized(role) || !userUc.IsAuthorized(role)) 
                return Unauthorized();
            
            try
            {
                await userUc.ExecuteAsync(id);
                await etudiantUc.ExecuteAsync(id);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }

            return NoContent();
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
}