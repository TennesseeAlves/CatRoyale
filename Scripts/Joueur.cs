using System;

namespace TestProjet.Scripts;

public class Joueur
{
    private string _pseudo;
    private int _winStreak;
    private int _jauge;
    private ListeDeCartes _main;
    private ListeDeCartes _deck;
    public static int MAXJAUGE = 15;
    
    //constructeurs
    public Joueur(string pseudo, int winStreak)
    {
        setPseudo(pseudo);
        setWinStreak(winStreak);
        setJauge(0);
        _main = new ListeDeCartes();
        _deck = new ListeDeCartes();
    }

    
    //getters et setters
    public string getPseudo()
    {
        return _pseudo;
    }
    public void setPseudo(string pseudo)
    {
        _pseudo = pseudo;
    }

    public int getWinStreak()
    {
        return _winStreak;
    }
    public void setWinStreak(int winStreak)
    {
        _winStreak = winStreak;
    }

    public int getJauge()
    
    {
        return _jauge;
    }
    public void setJauge(int jauge)
    {
        if (jauge >= 0 && jauge <= MAXJAUGE)
        {
            _jauge = jauge;
        }
    }
    
    //methodes autres
    public void remplirJauge(int niveau)
    {
        if (_jauge < niveau)
        {
            _jauge = niveau;
        }
    }

    public void reduireJauge(int usage)
    {
        _jauge -=  usage;
        if (_jauge < 0)
        {
            _jauge = 0;
        }
    }

    public void addCarteInMain(Carte carte)
    {
        _main.appendCarte(carte);
    }
    
    public int nbCarteInMain()
    {
        return _main.Length();
    }
    public void addCarteInDeck(Carte carte)
    {
        _deck.appendCarte(carte);
    }

    public Carte getCarteInMainAt(int i)
    {
        return _main.getCarteAt(i);
    }

    public Carte getCarteInDeckAt(int i)
    {
        return _deck.getCarteAt(i);
    }

    public void deleteCarteInMain(Carte carte)
    {
        int i = _main.getIndexOf(carte);
        if (i == -1)
        {
            throw new ArgumentException();
        }
        _main.removeCarteAt(i);
    }

    public void deleteCarteInDeck(Carte carte)
    {
        int i = _deck.getIndexOf(carte);
        if (i == -1)
        {
            throw new ArgumentException();
        }
        _deck.removeCarteAt(i);
    }

    public void pioche()
    {
        Random random = new Random();
        int i = random.Next(0, _deck.Length());
        Carte cartePioche = _deck.getCarteAt(i);
        _deck.removeCarteAt(i);
        _main.appendCarte(cartePioche);
    }
}