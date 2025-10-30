using System;

namespace TestProjet.Scripts;

public class ListeDeCartes
{
    private Carte[] _cartes;

    public ListeDeCartes()
    {
        _cartes = new Carte[0];
    }

    public int Length()
    {
        return _cartes.Length;
    }

    public Carte getCarteAt(int i) 
    {
        if (i >= _cartes.Length || i < 0)
        {
            throw new IndexOutOfRangeException();
        }
        return _cartes[i];
    }

    public void setCarteAt(int i, Carte carte) 
    {
        if (i >= _cartes.Length || i < 0)
        {
            throw new IndexOutOfRangeException();
        }
        _cartes[i] =  carte;
    }

    public Carte removeCarteAt(int i) 
    {
        if (i >= _cartes.Length || i < 0)
        {
            throw new IndexOutOfRangeException();
        }
        Carte rep = _cartes[i];
        Carte[] newCartes = new Carte[_cartes.Length - 1];
        for (int j = 0; j < _cartes.Length; j++)
        {
            if (j > i)
            {
                newCartes[j - 1] = _cartes[j];
            }
            else
            {
                newCartes[j] = _cartes[j];
            }
        }
        _cartes = newCartes;
        return rep;
    }

    public int getIndexOf(Carte carte) 
    {
        int rep = -1;
        int i = 0;
        while (rep == -1 && i < _cartes.Length)
        {
            if (_cartes[i] == carte)
            {
                rep = i;
            }
            i++;
        }
        return rep;
    }

    public void insertCarteAt(int i, Carte carte) 
    {
        if (i >= _cartes.Length || i < 0)
        {
            throw new IndexOutOfRangeException();
        }
        Carte[] newCartes = new Carte[_cartes.Length + 1];
        for (int j = 0; j < _cartes.Length; j++)
        {
            if (j > i)
            {
                newCartes[j + 1] = _cartes[j];
            }
            else if (j == i)
            {
                newCartes[j] = carte;
                newCartes[j + 1] = _cartes[j];
            }
            else
            {
                newCartes[j] = _cartes[j];
            }
        }
        _cartes = newCartes;
    }

    public void appendCarte(Carte carte) 
    {
        Carte[] newCartes = new Carte[_cartes.Length + 1];
        for (int j = 0; j < _cartes.Length; j++)
        {
            newCartes[j] = _cartes[j];
        }
        newCartes[_cartes.Length] = carte;

        _cartes = newCartes;
    }
}