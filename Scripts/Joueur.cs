using System;
using System.Xml.Serialization;

namespace TestProjet.Scripts;

[XmlType("Joueur")]
public class Joueur
{
    [XmlElement("pseudo")] public string Pseudo {get; set;}
    [XmlElement("winStreak")] public int WinStreak {get; set;}
    [XmlElement("jauge")] public int Jauge {
        get => _jauge;
        set => _jauge = (value < 0) ? 0 : ((value > MAXJAUGE) ? MAXJAUGE : value);
    }
    [XmlElement("main")] public ListeIdCartes Main {get; set;}
    [XmlElement("deck")] public ListeIdCartes Deck {get; set;}
    [XmlIgnore] public static int MAXJAUGE = 15;
    //champs internes
    [XmlIgnore] private int _jauge;
    
    //constructeurs
    public Joueur(string pseudo, int winStreak)
    {
        Pseudo = pseudo;
        WinStreak = winStreak;
        Jauge = 0;
        Main = new ListeIdCartes();
        Deck = new ListeIdCartes();
    }
    //constructeur vide pour le XMLSerializer
    public Joueur(){}
    
    //methodes autres
    public void remplirJauge()
    {
        Jauge = MAXJAUGE;
    }

    public void reduireJauge(int usage)
    {
        Jauge -= usage;
    }

    public int getNbCartesInMain()
    {
        return Main.Length();
    }
    
    public int getNbCartesInDeck()
    {
        return Deck.Length();
    }
    
    public void addCarteInMain(Carte carte)
    {
        Main.appendCarte(carte);
    }
    public void addCarteInMain(string carte)
    {
        Main.appendId(carte);
    }
    
    public void addCarteInDeck(Carte carte)
    {
        Deck.appendCarte(carte);
    }
    public void addCarteInDeck(string carte)
    {
        Deck.appendId(carte);
    }

    public Carte getCarteInMainAt(int i, ListeDeCartes Cartes)
    {
        return Main.getCarteAt(i, Cartes);
    }

    public Carte getCarteInDeckAt(int i, ListeDeCartes Cartes)
    {
        return Deck.getCarteAt(i, Cartes);
    }

    public void deleteCarteInMain(Carte carte)
    {
        int i = Main.getIndexOf(carte);
        if (i == -1)
        {
            throw new ArgumentException();
        }
        Main.removeIdAt(i);
    }

    public void deleteCarteInMainAt(int i)
    {
        if (i >= Main.Length())
        {
            throw new ArgumentException();
        }
        Main.removeIdAt(i);
    }

    public void deleteCarteInDeck(Carte carte)
    {
        int i = Deck.getIndexOf(carte);
        if (i == -1)
        {
            throw new ArgumentException();
        }
        Deck.removeIdAt(i);
    }

    public void deleteCarteInDeckAt(int i)
    {
        if (i >= Deck.Length())
        {
            throw new ArgumentException();
        }
        Deck.removeIdAt(i);
    }

    public void putCarteInDeckFromMainAt(int i)
    {
        if (i >= Main.Length())
        {
            throw new ArgumentException();
        }
        string carte = Main.getIdAt(i);
        Main.removeIdAt(i);
        Deck.appendId(carte);
    }

    public void pioche()
    {
        Random random = new Random();
        int i = random.Next(0, Deck.Length());
        string cartePioche = Deck.getIdAt(i);
        Deck.removeIdAt(i);
        Main.appendId(cartePioche);
    }
}