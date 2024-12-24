namespace UniversiteDomain.Entities;

public class Note
{
    public long IdEt { get; set; }
    public long IdUe { get; set; }
    public long Valeur { get; set; }
    public Etudiant Etudiant { get; set; }
    public Ue Ue { get; set; }
    
    public override string ToString()
    {
        return "ID "+IdEt +" : "+IdUe+" - "+Valeur + Etudiant.ToString() + Ue.ToString();
    }
}