namespace TestProjet;

public class Joueur
{
    private string pseudo;
    private int winStreak;
    private int jauge;
    private ListeDeCartes main;
    private ListeDeCartes deck;
    

    Joueur(string pseudo,int winStreak,int jauge) {
        this.pseudo = pseudo;
        this.winStreak = winStreak;
        this.jauge = jauge;
        
        this.main = null;
        this.deck = null;
        
    }

    void setMain(ListeDeCartes main)
    {
        this.main = main;
    }

    void setDeck(ListeDeCartes deck)
    {
        this.deck = deck;
    }

    public bool aCarte(Carte carte)
    {
        return main.aCarte(carte);
    }

    public int getJauge()
    {
        return jauge;
    }

   
    
    
}