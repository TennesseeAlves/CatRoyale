namespace TestProjet.Scripts;

public class Invocation
{
    private int _vie;
    private int _maxvie;
    private int _degat;
    private string _image;
    private bool _peutBouger;
    private bool _peutAttaquer;
    private Joueur _invocateur;
    
    //constructeur
    public Invocation(int vie, int degat, string image)
    {
        setVie(vie);
        setMaxVie(vie);
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
    
    public int getMaxVie()
    {
        return _maxvie;
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
    public void setMaxVie(int maxvie)
    {
        if (maxvie < 0)
        {
            _maxvie = 0;
        }
        else
        {
            _maxvie = maxvie;
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

    public bool takeDamage(int degat)
    {
        bool mort = false;
        _vie -= degat;
        if (_vie < 0)
        {
            _vie = 0;
            mort = true;
        }
        return mort;
    }
    
    public void takeVie(int vie)
    {
        _vie += vie;
        if (_vie > _maxvie)
        {
            _vie = _maxvie;
        }
    }

    public bool isCristal()
    {
        //pas une bonne méthode
        return _image == "cristal.png";
        Console.WriteLine(_vie);
    }
}