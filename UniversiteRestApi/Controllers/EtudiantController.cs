using Microsoft.AspNetCore.Http;
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
        // GET: api/<API>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<API>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // Crée un nouvel étudiant sans parcours
        // POST api/<EtudiantApi>
        [HttpPost]
        public async Task PostAsync([FromBody] Etudiant etudiant)
        {
            CreateEtudiantUseCase uc=new CreateEtudiantUseCase(repositoryFactory);
            await uc.ExecuteAsync(etudiant);
        }

        // PUT api/<API>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<API>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        
    }
}
