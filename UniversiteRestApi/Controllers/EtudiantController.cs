using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.EtudiantUseCases.Create;

namespace UniversiteRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EtudiantController(IRepositoryFactory repositoryFactory) : ControllerBase
    {
        // private readonly IRepositoryFactory _repositoryFactory;

        // GET: api/Etudiant - Récupère tous les étudiants
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var etudiants = await repositoryFactory.EtudiantRepository().FindAllAsync();
    
            // Convertit chaque étudiant en DTO
            var etudiantsDto = etudiants.Select(e => new EtudiantDto().ToDto(e)).ToList();
    
            return Ok(etudiantsDto);
        }

        // GET api/Etudiant/5 - Récupère un étudiant par son ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUnEtudiant(int id)
        {
            var etudiant = await repositoryFactory.EtudiantRepository().FindAsync(id);
            if (etudiant == null)
                return NotFound();
            return Ok(new EtudiantDto().ToDto(etudiant));
        }
        
        // POST api/Etudiant - Crée un nouvel étudiant
        //[HttpPost]
        //public async Task<Etudiant> PostAsync([FromBody] Etudiant etudiant)
        // {
        //     CreateEtudiantUseCase uc=new CreateEtudiantUseCase(repositoryFactory.EtudiantRepository());
        //    return await uc.ExecuteAsync(etudiant);
        // }
       
       
        // teste new version with DTO ====> Au lieu de recevoir un Etudiant complet, on reçoit un EtudiantDto (version simplifiée depend de ce que nous avant choisi à recevoir dans le fichier DTO )
       
        // POST api/<EtudiantController>
        [HttpPost]
        public async Task<ActionResult<EtudiantDto>> PostAsync([FromBody] EtudiantDto etudiantDto)
        {
            CreateEtudiantUseCase createEtudiantUc = new CreateEtudiantUseCase(repositoryFactory.EtudiantRepository());           
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
            EtudiantDto dto = new EtudiantDto().ToDto(etud);
            return CreatedAtAction(nameof(GetUnEtudiant), new { id = dto.Id }, dto);
        }
        
        // PUT api/Etudiant/5 - Modifie un étudiant
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/Etudiant/5 - Supprime un étudiant
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}