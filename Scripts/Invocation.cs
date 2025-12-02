using System;

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
        setDegat(degat);
        setImage(image);
        setPeutBouger(true);
        setPeutAttaquer(true);
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
        {   _maxvie=vie;
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
        Console.WriteLine(_vie);
    }
    
    public void takeVie(int vie)
    {
        Console.WriteLine(_vie);
        _vie += vie;
        if (_vie>_maxvie)
        {
            _vie = _maxvie;
        }
    }
}