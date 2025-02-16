namespace UniversiteDomain.Dtos;

public class NoteCsvDto
{
    public string Nom { get; set; }
    public string Prenom { get; set; }
    public float? Note { get; set; } // ✅ Float nullable (permet les valeurs vides)
}