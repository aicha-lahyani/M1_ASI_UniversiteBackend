using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions;
using UniversiteDomain.Exceptions.NoteException;
using UniversiteDomain.UseCases.NoteUseCases.Export;
using UniversiteDomain.UseCases.NoteUseCases.Import;
using UniversiteDomain.UseCases.NoteUseCases.Validate;
using UniversiteDomain.UseCases.SecurityUseCases.Get;

namespace UniversiteRestApi.Controllers
{
    [Route("api/note")]
    [ApiController]
    public class NoteApiController : ControllerBase
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly CreateNoteUseCase _createNoteUseCase;

        // Injection de dépendances
        public NoteApiController(IRepositoryFactory repositoryFactory, CreateNoteUseCase createNoteUseCase)
        {
            _repositoryFactory = repositoryFactory;
            _createNoteUseCase = createNoteUseCase;
        }

        // Méthode pour vérifier la sécurité et les autorisations de l'utilisateur
        private void CheckSecu(out string role, out string email, out IUniversiteUser user)
        {
            role = "";
            ClaimsPrincipal claims = HttpContext.User;

            // Vérification de l'authentification
            if (claims.Identity?.IsAuthenticated != true)
                throw new UnauthorizedAccessException("L'utilisateur n'est pas authentifié.");

            // Récupération de l'email depuis les claims
            if (claims.FindFirst(ClaimTypes.Email) == null)
                throw new UnauthorizedAccessException("L'email est introuvable dans les claims.");

            email = claims.FindFirst(ClaimTypes.Email).Value;
            if (email == null)
                throw new UnauthorizedAccessException("L'email est nul.");

            // Recherche de l'utilisateur par email
            user = new FindUniversiteUserByEmailUseCase(_repositoryFactory).ExecuteAsync(email).Result;
            if (user == null)
                throw new UnauthorizedAccessException("Aucun utilisateur trouvé avec cet email.");

            // Vérification du rôle de l'utilisateur
            if (claims.FindFirst(ClaimTypes.Role) == null)
                throw new UnauthorizedAccessException("Le rôle est introuvable dans les claims.");

            var ident = claims.Identities.FirstOrDefault();
            if (ident == null)
                throw new UnauthorizedAccessException("Aucune identité trouvée.");

            role = ident.FindFirst(ClaimTypes.Role).Value;
            if (role == null)
                throw new UnauthorizedAccessException("Le rôle est nul.");

            // Vérification que l'utilisateur a bien le rôle approprié
            bool isInRole = new IsInRoleUseCase(_repositoryFactory).ExecuteAsync(email, role).Result;
            if (!isInRole)
                throw new UnauthorizedAccessException("L'utilisateur n'a pas les permissions nécessaires.");
        }

        [HttpGet("export/{ueId}")]
        public async Task<IActionResult> GenerateCsv(long ueId)
        {
            try
            {
                // Vérifier si l'utilisateur est autorisé à exporter
                string role = "";
                string email = "";
                IUniversiteUser user = null;
                CheckSecu(out role, out email, out user);

                // Vérification des autorisations
                if (!_createNoteUseCase.IsAuthorized(role))
                {
                    return Unauthorized(new { error = "Vous n'êtes pas autorisé à effectuer cette action." });
                }

                // Exécution du cas d'utilisation pour générer le CSV
                var exportNoteCsv = new ExportNotesCsvUseCase(_repositoryFactory);
                var csvContent = await exportNoteCsv.ExecuteAsync(ueId);
                return File(System.Text.Encoding.UTF8.GetBytes(csvContent), "text/csv", $"UE_{ueId}_Notes.csv");
            }
            catch (CsvProcessingException e)
            {
                return BadRequest(new { error = e.Message });
            }
            catch (UnauthorizedAccessException)
            {
                // Retourner un Unauthorized (401) explicite
                return Unauthorized(new { error = "Accès non autorisé. Vous n'avez pas les permissions nécessaires." });
            }
        }

        /// <summary>
        /// Uploade un fichier CSV rempli et enregistre les notes si le fichier est valide.
        /// </summary>
        [HttpPost("upload/{ueId}")]
        public async Task<IActionResult> UploadCsv(long ueId, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { error = "Fichier invalide ou vide." });
            }

            try
            {
                // Vérifier si l'utilisateur est autorisé à importer
                string role = "";
                string email = "";
                IUniversiteUser user = null;
                CheckSecu(out role, out email, out user);

                // Vérification des autorisations
                if (!_createNoteUseCase.IsAuthorized(role))
                {
                    return Unauthorized(new { error = "Vous n'êtes pas autorisé à effectuer cette action." });
                }

                using (var stream = file.OpenReadStream())
                {
                    var validate = new ValidationUseCase(_repositoryFactory);
                    var importNoteCsv = new ImportNotesCsvUseCase(_repositoryFactory, validate);
                    await importNoteCsv.ExecuteAsync(stream, ueId);
                }

                return Ok(new { message = "Notes enregistrées avec succès." });
            }
            catch (CsvProcessingException e)
            {
                return BadRequest(new { error = e.Message });
            }
            catch (UnauthorizedAccessException)
            {
                // Retourner un Unauthorized (401) explicite
                return Unauthorized(new { error = "Accès non autorisé. Vous n'avez pas les permissions nécessaires." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Une erreur interne s'est produite." });
            }
        }
    }
}
