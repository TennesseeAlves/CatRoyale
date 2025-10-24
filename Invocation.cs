namespace TestProjet;

public class Invocation
{
    private int vie;
    private int degat;
    private Joueur invocateur;
    private bool estJouable;

    

    public Invocation(int vie, int degat, Joueur invocateur) {
        this.vie = vie;
        this.degat = degat;
        this.invocateur = invocateur;
        this.estJouable = true;
    }

    void setEstJouable(bool estJouable)
    {
        this.estJouable =  estJouable;
    }

    public int getDegat()
    {
        return this.degat;
    }

    public bool reduireVie(int nbdegat)
    {   
        vie= vie-nbdegat;
        if (this.vie <= 0)
        {
            return true;
        }

        return false;
    }
    

   
    
    
}