namespace TestProjet;

public class ListeDeCartes
{
    Carte[] Cartes;
    int nbCartes;

    ListeDeCartes() {
        this.Cartes = new Carte[5];
        this.nbCartes = 0;
    }

    bool addCarte(Carte carte)
    {
        if (this.nbCartes < 5)
        {
            Cartes[nbCartes]= carte;
            this.nbCartes++;
            return true;
        }
        return false;
        
    }

    void removeCarte(int indice)
    {
        if (indice < this.nbCartes && indice >= 0)
        {
            Cartes[indice]=  Cartes[nbCartes];
            this.nbCartes--;

        }
        
    }

    public bool aCarte(Carte carte)
    {
        for (int i = 0; i < 5; i++)
        {
            if (carte == this.Cartes[i])
            {
                return true;
            }
        }
        return false;
    }
    

   
    
    
}