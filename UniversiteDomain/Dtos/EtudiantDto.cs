using UniversiteDomain.Entities;

namespace UniversiteDomain.Dtos;

public class EtudiantDto
{
    public long Id { get; set; }
    public string NumEtud { get; set; }
    public string Nom { get; set; }
    public string Prenom { get; set; }
    public string Email { get; set; }

    public EtudiantDto ToDto(Etudiant etudiant)
    {
        Id = etudiant.Id;
        NumEtud = etudiant.NumEtud;
        Nom = etudiant.Nom;
        Prenom = etudiant.Prenom;
        Email = etudiant.Email;
        return this;
    }
    public static List<EtudiantDto> ToDtos(List<Etudiant> etudiants)
    {
        List<EtudiantDto> dtos = new();
        foreach (var etudiant in etudiants)
        {
            dtos.Add(new EtudiantDto().ToDto(etudiant));
        }
        return dtos;
    }
    public Etudiant ToEntity()
    {
        return new Etudiant {Id = this.Id, NumEtud = this.NumEtud, Nom = this.Nom, Prenom = this.Prenom, Email = this.Email};
    }
}