using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Graphics;

namespace TestProjet.Scripts;

public enum TypeDeCarte
{
    [XmlEnum("combattant")] COMBATTANT,
    [XmlEnum("sort")] SORT,
    [XmlEnum("objet")] OBJET
}

public enum TypeRarete
{
    [XmlEnum("commune")] COMMUNE,
    [XmlEnum("rare")] RARE,
    [XmlEnum("épique")] EPIQUE,
    [XmlEnum("légendaire")] LEGENDAIRE
}

[XmlType("Carte")]
public class Carte
{
    [XmlElement("vie")] public int Vie {
        get => _vie;
        set => _vie = (value < 0) ? 0 : value;
    }
    [XmlElement("degat")] public int Degat { get; set; }
    [XmlElement("nom")] public string Nom { get; set; }
    [XmlElement("imageCarte")] public string Image { get; set; }
    [XmlElement("imageInvocation")] public string SpriteInvocation { get; set; }
    [XmlElement("cout")] public int Cout { get; set; }
    [XmlElement("type")] public TypeDeCarte Type { get; set; }
    [XmlElement("rarete")] public TypeRarete Rarete { get; set; }
    //champs internes
    [XmlIgnore] private int _vie;
    
    //constructeurs
    public Carte(int vie, int degat, int cout, string nom, string image, TypeDeCarte type, TypeRarete rarete, string spriteInvocation)
    {
        Vie = vie;
        Degat = degat;
        Nom = nom;
        Image = image;
        SpriteInvocation = spriteInvocation;
        Cout = cout;
        Type = type;
        Rarete = rarete;
        
    }
    //constructeur vide pour le XMLSerializer
    public Carte() { }

    public Invocation generateInvocation()
    {
        
        return new Invocation(Vie, Degat, SpriteInvocation);
    }

    public override string ToString()
    {
        String affichage= "Nom: "+Nom+"\n"+ 
                          "Cout: "+Cout+"\n"+
                          "Type: "+Type+"\n"+
                          "Rareté: "+Rarete+"\n"+
                          "Degat: "+Degat+"\n"+ 
                          "Vie: "+Vie+"\n" ;
        
        if (Type == TypeDeCarte.SORT)
        {
            String soinoudegat= "Inflige :" + Degat;
            if (Degat < 0)
            {
                soinoudegat = "Soigne :" + Math.Abs(Degat);
            }
            affichage= "Nom: "+Nom+"\n"+ 
                   "Cout: "+Cout+"\n"+
                   "Type: "+Type+"\n"+
                   "Rareté: "+Rarete+"\n"+
                   soinoudegat+"\n";
        }

        return affichage;

    }
}