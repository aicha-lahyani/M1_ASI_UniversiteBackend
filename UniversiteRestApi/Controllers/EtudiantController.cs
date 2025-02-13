using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.EtudiantUseCases.Create;
using UniversiteDomain.UseCases.EtudiantUseCases.Get;
using UniversiteDomain.UseCases.EtudiantUseCases.Delete;
using UniversiteDomain.UseCases.SecurityUseCases.Create;
using UniversiteDomain.UseCases.SecurityUseCases.Get;
using UniversiteEFDataProvider.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Université_Domain.UseCases.EtudiantUseCases.Update;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;

namespace UniversiteRestApi.Controllers
{
    //[AllowAnonymous]
    [Authorize] // Sécurisation globale du contrôleur
    [Route("api/[controller]")]
    [ApiController]
    public class EtudiantController : ControllerBase
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly CreateEtudiantUseCase _createEtudiantUseCase;

        public EtudiantController(IRepositoryFactory repositoryFactory, CreateEtudiantUseCase createEtudiantUseCase)
        {
            _repositoryFactory = repositoryFactory;
            _createEtudiantUseCase = createEtudiantUseCase;
        }
        private void CheckSecu(out string role, out string email, out IUniversiteUser user)
        {
            role = "";
            ClaimsPrincipal claims = HttpContext.User;

            if (claims.Identity?.IsAuthenticated != true)
            {
                Console.WriteLine("⚠️ L'utilisateur n'est pas authentifié.");
                throw new UnauthorizedAccessException();
            }

            var emailClaim = claims.FindFirst(ClaimTypes.Email);
            if (emailClaim == null)
            {
                Console.WriteLine("⚠️ Aucun email trouvé dans les claims.");
                throw new UnauthorizedAccessException();
            }

            email = emailClaim.Value;
            Console.WriteLine($"✅ Utilisateur authentifié : {email}");

            user = new FindUniversiteUserByEmailUseCase(_repositoryFactory).ExecuteAsync(email).Result;
            if (user == null)
            {
                Console.WriteLine("⚠️ L'utilisateur n'existe pas en base.");
                throw new UnauthorizedAccessException();
            }

            var roleClaim = claims.FindFirst(ClaimTypes.Role);
            if (roleClaim == null)
            {
                Console.WriteLine("⚠️ Aucun rôle trouvé dans les claims.");
                throw new UnauthorizedAccessException();
            }

            role = roleClaim.Value;
            Console.WriteLine($"✅ Rôle détecté : {role}");

            bool isInRole = new IsInRoleUseCase(_repositoryFactory).ExecuteAsync(email, role).Result;
            if (!isInRole)
            {
                Console.WriteLine($"❌ L'utilisateur {email} n'a pas le rôle requis ({role}).");
                throw new UnauthorizedAccessException();
            }

            Console.WriteLine($"✅ L'utilisateur {email} a bien le rôle {role}");
        }


        [HttpGet]
        public async Task<ActionResult<List<EtudiantDto>>> GetAllEtudiants()
        {
            var etudiants = await _repositoryFactory.EtudiantRepository().GetAllAsync();
            return Ok(EtudiantDto.ToDtos(etudiants));
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<EtudiantDto>> GetUnEtudiant(long id)
        {
            var etudiant = await _repositoryFactory.EtudiantRepository().GetByIdAsync(id);
            if (etudiant == null)
            {
                return NotFound();
            }
            return Ok(new EtudiantDto().ToDto(etudiant));
        }
        [HttpPost]
        public async Task<ActionResult<EtudiantDto>> PostAsync([FromBody] EtudiantDto etudiantDto)
        {
            if (etudiantDto == null)
            {
                return BadRequest();
            }

            CreateUniversiteUserUseCase createUserUc = new CreateUniversiteUserUseCase(_repositoryFactory);

            string role = "";
            string email = "";
            IUniversiteUser user = null;

            CheckSecu(out role, out email, out user);

            if (!_createEtudiantUseCase.IsAuthorized(role) || !createUserUc.IsAuthorized(role))
                return Unauthorized();

            Etudiant etud = etudiantDto.ToEntity();

            try
            {
                etud = await _createEtudiantUseCase.ExecuteAsync(etud);
            }   
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }

            try
            {
                user = new UniversiteUser { UserName = etudiantDto.Email, Email = etudiantDto.Email, Etudiant = etud };
                await createUserUc.ExecuteAsync(etud.Email, etud.Email, "Miage2025#", Roles.Etudiant, etud);
            }
            catch (Exception e)
            {
                await new DeleteEtudiantUseCase(_repositoryFactory).ExecuteAsync(etud.Id);
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }

            EtudiantDto dto = new EtudiantDto().ToDto(etud);
            return CreatedAtAction(nameof(GetUnEtudiant), new { id = dto.Id }, dto);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(long id, [FromBody] EtudiantDto etudiantDto)
        {
            if (etudiantDto == null || id != etudiantDto.Id)
            {
                return BadRequest();
            }

            UpdateEtudiantUseCase updateEtudiantUc = new UpdateEtudiantUseCase(_repositoryFactory);

            string role = "";
            string email = "";
            IUniversiteUser user = null;
            CheckSecu(out role, out email, out user);

            if (!updateEtudiantUc.IsAuthorized(role)) return Unauthorized();

            try
            {
                await updateEtudiantUc.ExecuteAsync(etudiantDto.ToEntity());
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }

            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            DeleteEtudiantUseCase etudiantUc = new DeleteEtudiantUseCase(_repositoryFactory);

            string role = "";
            string email = "";
            IUniversiteUser user = null;
            CheckSecu(out role, out email, out user);

            if (!etudiantUc.IsAuthorized(role)) return Unauthorized();

            try
            {
                await etudiantUc.ExecuteAsync(id);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }

            return NoContent();
        }
        [HttpGet("complet/{id}")]
        public async Task<ActionResult<EtudiantCompletDto>> GetUnEtudiantCompletAsync(long id)
        {
            string role = "";
            string email = "";
            IUniversiteUser user = null;

            try
            {
                CheckSecu(out role, out email, out user);
            }
            catch (Exception e)
            {
                return Unauthorized();
            }

            GetEtudiantCompletUseCase uc = new GetEtudiantCompletUseCase(_repositoryFactory);

            if (!uc.IsAuthorized(role, user, id)) return Unauthorized();

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

            if (etud == null) return NotFound();
            return Ok(new EtudiantCompletDto().ToDto(etud));
        }
    }
}