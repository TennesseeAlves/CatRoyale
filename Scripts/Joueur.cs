namespace TestProjet.Scripts;

public class Joueur
{
    private string _pseudo;
    private int _winStreak;
    private int _jauge;
    private ListeDeCartes _main;
    private ListeDeCartes _deck;

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
        _jauge = jauge;
    }
}