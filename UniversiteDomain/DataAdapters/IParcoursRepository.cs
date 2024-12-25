using System.Linq.Expressions;
using UniversiteDomain.Entities;

namespace UniversiteDomain.DataAdapters;

public  interface IParcoursRepository : IRepository<Parcours>
{
    public  Task<Parcours> AddEtudiantAsync(Parcours parcours, Etudiant etudiant);
    public  Task<Parcours> AddEtudiantAsync(long idParcours, long idEtudiant);
    public  Task<Parcours> AddEtudiantAsync(Parcours? parcours, List<Etudiant> etudiants);
    public  Task<Parcours> AddEtudiantAsync(long idParcours, long[] idEtudiants);
    public  Task<Parcours> AddUeAsync(long idParcours, long[] idUe);
    public  Task<Parcours> AddUeAsync(long idParcours, long idUe);
}