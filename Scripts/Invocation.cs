using System.Xml.Serialization;
namespace CatRoyale.Scripts;

[XmlType("Invocation")]
public class Invocation
{
    [XmlElement("pseudoInvocateur")] public string PseudoInvocateur { get; set; }
    [XmlElement("maxVie")] public int MaxVie {
        get => _maxVie;
        set => _maxVie = (value < 0) ? 0 : value;
    }
    [XmlElement("vie")] public int Vie {
        get => _vie;
        set => _vie = (value < 0) ? 0 : ((value > MaxVie) ? MaxVie : value);
    }
    [XmlElement("degat")] public int Degat { get; set; }
    [XmlElement("nom")] public string Nom { get; set; }
    [XmlElement("image")] public string Image { get; set; }
    [XmlElement("peutBouger")] public bool PeutBouger { get; set; }
    [XmlElement("peutAttaquer")] public bool PeutAttaquer { get; set; }
    [XmlIgnore] private int _vie;
    [XmlIgnore] private int _maxVie;
    
    //contructeur avec paramètres non utilisé actuellement mais déja implémenté par sécurité
    public Invocation(int vie, int degat, string nom, string image)
    {
        MaxVie = vie;
        Vie = vie;
        Degat = degat;
        Nom = nom;
        Image = image;
        PeutBouger = false;
        PeutAttaquer = false;
    }
    //constructeur vide pour le XMLSerializer
    public Invocation() { }

    public bool TakeDamage(int degat)
    {
        Vie -= degat;
        //pas besoin de vérifier <0 car le setter assure que Vie ne passe pas en dessous de 0
        if (Vie == 0)
        {
            return true;
        }
        return false;
    }
}