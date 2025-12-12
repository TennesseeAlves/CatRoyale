using System.Xml.Serialization;

namespace CatRoyale.Scripts;

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
    [XmlIgnore] private int _vie;
    
    //contructeur avec paramètres non utilisé actuellement mais déja implémenté par sécurité
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
    //contructeur vide pour le XMLSerializer
    public Carte() { }

    public Invocation GenerateInvocation()
    {
        return new Invocation(Vie, Degat, Nom, SpriteInvocation);
    }

    public override string ToString()
    {
        switch (Type)
        {
            case TypeDeCarte.COMBATTANT:
                return "Nom: "+Nom+"\n"+ 
                       "Cout: "+Cout+"\n"+
                       "Type: "+Type+"\n"+
                       "Rareté: "+Rarete+"\n"+
                       "Degat: "+Degat+"\n"+ 
                       "Vie: "+Vie+"\n" ;
            case TypeDeCarte.SORT:
                if (Degat < 0)
                {
                    return "Nom: "+Nom+"\n"+ 
                           "Cout: "+Cout+"\n"+
                           "Type: "+Type+"\n"+
                           "Rareté: "+Rarete+"\n"+
                           "Soigne :"+(-1*Degat)+"\n";
                }
                return "Nom: "+Nom+"\n"+ 
                       "Cout: "+Cout+"\n"+
                       "Type: "+Type+"\n"+
                       "Rareté: "+Rarete+"\n"+
                       "Inflige :"+Degat+"\n";
            case TypeDeCarte.OBJET:
                return "Nom: "+Nom+"\n" +
                       "Cout: "+Cout+"\n" +
                       "Type: "+Type+"\n" +
                       "Rareté: "+Rarete+"\n";
        }
        return "";
    }
}