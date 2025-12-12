using System;
using System.Xml.Serialization;

namespace CatRoyale.Scripts;

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
    [XmlIgnore] public static int MAXJAUGE = 14;
    [XmlIgnore] private int _jauge;
    
    //contructeur avec paramètres non utilisé actuellement mais déja implémenté par sécurité
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
    
    public void RemplirJauge()
    {
        Jauge = MAXJAUGE;
    }

    public void ReduireJauge(int usage)
    {
        Jauge -= usage;
    }

    public int GetNbCartesInMain()
    {
        return Main.Length();
    }
    
    public int GetNbCartesInDeck()
    {
        return Deck.Length();
    }
    
    //méthode inutilisée mais implémentée car toujours utile pour manipuler des listes
    public void AddCarteInMain(Carte carte)
    {
        Main.AppendCarte(carte);
    }
    
    //méthode inutilisée mais implémentée car toujours utile pour manipuler des listes
    public void AddCarteInMain(string carte)
    {
        Main.AppendId(carte);
    }
    
    //méthode inutilisée mais implémentée car toujours utile pour manipuler des listes
    public void AddCarteInDeck(Carte carte)
    {
        Deck.AppendCarte(carte);
    }
    
    //méthode inutilisée mais implémentée car toujours utile pour manipuler des listes
    public void AddCarteInDeck(string carte)
    {
        Deck.AppendId(carte);
    }

    //méthode inutilisée mais implémentée car toujours utile pour manipuler des listes
    public Carte GetCarteInMainAt(int i, ListeDeCartes cartes)
    {
        return Main.GetCarteAt(i, cartes);
    }

    //méthode inutilisée mais implémentée car toujours utile pour manipuler des listes
    public Carte GetCarteInDeckAt(int i, ListeDeCartes cartes)
    {
        return Deck.GetCarteAt(i, cartes);
    }

    //méthode inutilisée mais implémentée car toujours utile pour manipuler des listes
    public void DeleteCarteInMain(Carte carte)
    {
        int i = Main.GetIndexOf(carte);
        if (i == -1)
        {
            throw new ArgumentException();
        }
        Main.RemoveIdAt(i);
    }

    //méthode inutilisée mais implémentée car toujours utile pour manipuler des listes
    public void DeleteCarteInMainAt(int i)
    {
        if (i >= Main.Length())
        {
            throw new ArgumentException();
        }
        Main.RemoveIdAt(i);
    }

    //méthode inutilisée mais implémentée car toujours utile pour manipuler des listes
    public void DeleteCarteInDeck(Carte carte)
    {
        int i = Deck.GetIndexOf(carte);
        if (i == -1)
        {
            throw new ArgumentException();
        }
        Deck.RemoveIdAt(i);
    }

    //méthode inutilisée mais implémentée car toujours utile pour manipuler des listes
    public void DeleteCarteInDeckAt(int i)
    {
        if (i >= Deck.Length())
        {
            throw new ArgumentException();
        }
        Deck.RemoveIdAt(i);
    }

    public void PutCarteInDeckFromMainAt(int i)
    {
        if (i >= Main.Length())
        {
            throw new ArgumentException();
        }
        string carte = Main.GetIdAt(i);
        Main.RemoveIdAt(i);
        Deck.AppendId(carte);
    }

    public void Pioche()
    {
        Random random = new Random();
        int i = random.Next(0, Deck.Length());
        string cartePioche = Deck.GetIdAt(i);
        Deck.RemoveIdAt(i);
        Main.AppendId(cartePioche);
    }
}