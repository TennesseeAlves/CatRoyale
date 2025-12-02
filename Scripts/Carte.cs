using Microsoft.Xna.Framework.Graphics;

namespace TestProjet.Scripts;

public enum TypeDeCarte { COMBATTANT, SORT, OBJET }

public enum TypeRarete { COMMUNE, RARE, EPIQUE, LEGENDAIRE }

public class Carte
{
    private int _vie;
    private int _degat;
    private int _cout;
    private string _nom;
    private string _image;
    private TypeDeCarte _type;
    private TypeRarete _rarete;
    private string spriteInvocation;
    //constructeur
    public Carte(int vie, int degat, int cout, string nom, string image, TypeDeCarte type, TypeRarete rarete, string spriteInvocation)
    {
        setVie(vie);
        setDegat(degat);
        setCout(cout);
        setNom(nom);
        setImage(image);
        this.spriteInvocation = spriteInvocation;
        setType(type);
        setRarete(rarete);
        
    }
    
    //getters et setters
    public int getVie()
    {
        return _vie;
    }
    public void setVie(int vie)
    {
        if (vie < 0)
        {
            _vie = 0;
        }
        else
        {
            _vie = vie;
        }
    }

    public int getDegat()
    {
        return _degat;
    }
    public void setDegat(int degat)
    {
        _degat =  degat;
    }

    public int getCout()
    {
        return _cout;
    }
    public void setCout(int cout)
    {
        _cout = cout;
    }

    public string getNom()
    {
        return _nom;
    }
    public void setNom(string nom)
    {
        _nom = nom;
    }

    public string getImage()
    {
        return _image;
    }
    public void setImage(string image)
    {
        _image = image;
    }

    public Invocation generateInvocation()
    {
        
        return new Invocation(getVie(), getDegat(), spriteInvocation);
    }

    public TypeDeCarte getType()
    {
        return _type;
    }
    public void setType(TypeDeCarte type)
    {
        _type  = type;
    }

    public TypeRarete getRarete()
    {
        return _rarete;
    }
    public void setRarete(TypeRarete rarete)
    {
        _rarete  = rarete;
    }

    public override string ToString()
    {
        return "Nom: "+getNom()+"\n"+ 
               "Cout: "+getCout()+"\n"+
               "Type: "+getType()+"\n"+
               "Rareté: "+getRarete()+"\n"+
               "Degat: "+getDegat()+"\n"+ 
               "Vie: "+getVie()+"\n" ;
        
    }
}