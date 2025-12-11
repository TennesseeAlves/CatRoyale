using System.Xml.Serialization;
using System;
namespace TestProjet.Scripts;

[XmlType("Invocation")]
public class Invocation
{
    [XmlElement("pseudoInvocateur")] public string PseudoInvocateur { get; set; }
    [XmlElement("vie")] public int Vie {
        get => _vie;
        set => _vie = (value < 0) ? 0 : value;
    }
    [XmlElement("maxVie")] public int MaxVie {
        get => _maxVie;
        set => _maxVie = (value < 0) ? 0 : value;
    }
    [XmlElement("degat")] public int Degat { get; set; }
    [XmlElement("image")] public string Image { get; set; }
    [XmlElement("peutBouger")] public bool PeutBouger { get; set; }
    [XmlElement("peutAttaquer")] public bool PeutAttaquer { get; set; }
    //champs internes
    [XmlIgnore] private int _vie;
    [XmlIgnore] private int _maxVie;
    
    //constructeur
    public Invocation(int vie, int degat, string image)
    {
        Vie = vie;
        MaxVie = vie;
        Degat = degat;
        Image = image;
        PeutBouger = false;
        PeutAttaquer = false;
    }
    //constructeur vide pour le XMLSerializer
    public Invocation() { }

    public bool takeDamage(int degat)
    {
        if (degat < 0)
        {
            takeVie(Math.Abs(degat));
            return false;
        }
        Vie -= degat;
        if (Vie == 0)
        {
            return true;
        }
        return false;
    }
    
    public void takeVie(int vie)
    {
        Vie += vie;
        if (Vie > MaxVie)
        {
            Vie = MaxVie;
        }
    }
}