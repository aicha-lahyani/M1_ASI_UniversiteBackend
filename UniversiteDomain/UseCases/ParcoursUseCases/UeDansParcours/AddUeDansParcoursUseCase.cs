namespace UniversiteDomain.UseCases.ParcoursUseCases.UeDansParcours;

using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursExceptions;
using UniversiteDomain.Exceptions.UeExceptions;


public class AddUeDansParcoursUseCase(IRepositoryFactory repositoryFactory)
{
    // Rajout d'une Ue dans un parcours
      public async Task<Parcours> ExecuteAsync(Parcours parcours, Ue ue)
      {
          ArgumentNullException.ThrowIfNull(parcours);
          ArgumentNullException.ThrowIfNull(ue);
          return await ExecuteAsync(parcours.Id, ue.Id); 
      }
public async Task<Parcours> ExecuteAsync(long idParcours, long idUe)
      {
          await CheckBusinessRules(idParcours, idUe); 
          return await repositoryFactory.ParcoursRepository().AddUeAsync(idParcours, idUe);
      }

      // Rajout de plusieurs étudiants dans un parcours
      public async Task<Parcours> ExecuteAsync(Parcours parcours, List<Ue> ues)
      {
          ArgumentNullException.ThrowIfNull(ues);
          ArgumentNullException.ThrowIfNull(parcours);
          long[] idUes = ues.Select(x => x.Id).ToArray();
          return await ExecuteAsync(parcours.Id, idUes); 
      }
      public async Task<Parcours> ExecuteAsync(long idParcours, long [] idUes)
      { 
        // Comme demandé par le client, on teste tous les règles avant de modifier les données
        foreach(var id in idUes) await CheckBusinessRules(idParcours, id);
        return await repositoryFactory.ParcoursRepository().AddUeAsync(idParcours, idUes);
      }
private async Task CheckBusinessRules(long idParcours, long idUe)
    {
        // Vérification des paramètres
        ArgumentNullException.ThrowIfNull(idParcours);
        ArgumentNullException.ThrowIfNull(idUe);

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idParcours);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idUe);

        // Vérifions tout d'abord que nous sommes bien connectés aux datasources
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        ArgumentNullException.ThrowIfNull(repositoryFactory.UeRepository());
        ArgumentNullException.ThrowIfNull(repositoryFactory.UeRepository());

        // On recherche l'ue
        List<Ue> ue = await repositoryFactory.UeRepository().FindByConditionAsync(e=>e.Id.Equals(idUe));;
        if (ue ==null) throw new UeNotFoundException(idUe.ToString());
        // On recherche le parcours
        List<Parcours> parcours = await repositoryFactory.ParcoursRepository().FindByConditionAsync(p=>p.Id.Equals(idParcours));;
        if (parcours ==null) throw new ParcoursNotFoundException(idParcours.ToString());

        // On vérifie que l'Ue n'est pas déjà dans le parcours
        if (parcours[0].UesEnseignees!=null)
        {
            // Des ues sont déjà enregistrées dans le parcours
            // On recherche si l'ue qu'on veut ajouter n'existe pas déjà
            List<Ue> inscrites = parcours[0].UesEnseignees;
            var trouve=inscrites.FindAll(e=>e.Id.Equals(idUe));
            if (trouve is {Count:>0}) throw new DuplicateUeDansParcoursException(idUe+" est déjà présente dans le parcours : "+idParcours);
        }
    }
}