namespace TestProjet.Scripts;

public class Invocation
{
    private int _vie;
    private int _degat;
    private string _image;
    private bool _peutBouger;
    private bool _peutAttaquer;
    private Joueur _invocateur;
    
    //constructeur
    public Invocation(int vie, int degat, string image)
    {
        setVie(vie);
        setDegat(degat);
        setImage(image);
        setPeutBouger(false);
        setPeutAttaquer(false);
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

    public string getImage()
    {
        return _image;
    }
    public void setImage(string image)
    {
        _image = image;
    }

    public bool getPeutBouger()
    {
        return _peutBouger;
    }
    public void setPeutBouger(bool peutBouger)
    {
        _peutBouger = peutBouger;
    }

    public bool getPeutAttaquer()
    {
        return _peutAttaquer;
    }
    public void setPeutAttaquer(bool peutAttaquer)
    {
        _peutAttaquer = peutAttaquer;
    }

    public Joueur getInvocateur()
    {
        return _invocateur;
    }
    public void setInvocateur(Joueur invocateur)
    {
        _invocateur = invocateur;
    }

    public void takeDamage(int degat)
    {
        _vie -= degat;
        if (_vie < 0)
        {
            _vie = 0;//là bah on meurt
        }
    }
}