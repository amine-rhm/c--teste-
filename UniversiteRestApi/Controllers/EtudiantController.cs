using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
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
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/Etudiant/5 - Récupère un étudiant par son ID
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/Etudiant - Crée un nouvel étudiant
        [HttpPost]
        public async Task<Etudiant> PostAsync([FromBody] Etudiant etudiant)
        {
            CreateEtudiantUseCase uc=new CreateEtudiantUseCase(repositoryFactory.EtudiantRepository());
            return await uc.ExecuteAsync(etudiant);
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