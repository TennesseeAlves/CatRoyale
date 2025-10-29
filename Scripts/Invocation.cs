namespace TestProjet.Scripts;

public class Invocation
{
    private int _vie;
    private int _degat;
    private string _image;
    
    //constructeur
    public Invocation(int vie, int degat, string image)
    {
        setVie(vie);
        setDegat(degat);
        setImage(image);
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
}