namespace TestProjet;

public class Carte
{
    private int vie;
    private int degat;
    private string nom;
    private string image;
    private int cout;
    private TypeDeCarte type;
    private Invocation invocation;
    TypeRarete rarete;
    
    
    Carte(string nom, int vie, int degat, int cout, Invocation invocation)
    {
        setImage("default.jpg");
        setRarete(TypeRarete.Commune);
        setType(TypeDeCarte.Combattant);
        
        this.nom = nom;
        this.vie = vie;
        this.degat = degat;
        this.cout = cout;
        this.invocation = invocation;
    }

    void setImage(string image)
    {
        this.image = image;
    }
    
    void setRarete(TypeRarete rarete)
    {
        this.rarete = rarete;
    }
    
    void setType(TypeDeCarte type)
    {
        this.type = type;
    }

    public int getVie()
    {
        return this.vie;
    }
    
    public int getDegat()
    {
        return this.degat;
    }
    public int getCout()
    {
        return this.cout;
    }
    
    
   
    
    
}