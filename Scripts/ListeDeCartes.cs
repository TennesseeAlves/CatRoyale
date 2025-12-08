using System;
using System.Xml.Serialization;

namespace TestProjet.Scripts;

[XmlType("ListeDeCartes")]
public class ListeDeCartes
{
    [XmlElement("carte")] public Carte[] Cartes {get; set;}

    public ListeDeCartes()
    {
        Cartes = new Carte[0];
    }

    public int Length()
    {
        return Cartes.Length;
    }

    public Carte getCarteAt(int i) 
    {
        if (i >= Cartes.Length || i < 0)
        {
            throw new IndexOutOfRangeException();
        }
        return Cartes[i];
    }

    public void setCarteAt(int i, Carte carte) 
    {
        if (i >= Cartes.Length || i < 0)
        {
            throw new IndexOutOfRangeException();
        }
        Cartes[i] =  carte;
    }

    public Carte removeCarteAt(int i) 
    {
        if (i >= Cartes.Length || i < 0)
        {
            throw new IndexOutOfRangeException();
        }
        Carte rep = Cartes[i];
        Carte[] newCartes = new Carte[Cartes.Length - 1];

        int k = 0;
        for (int j = 0; j < Cartes.Length; j++)
        {
            if (j == i)
            {
                continue;
            } 

            newCartes[k] = Cartes[j];
            k++;
        }
        /* alternative
        for (int j = 0; j < _cartes.Length; j++)
        {
            if (j > i)
            {
                newCartes[j - 1] = _cartes[j];
            }
            else if (j < i)
            {
                newCartes[j] = _cartes[j];
            }
        }
        */
        Cartes = newCartes;
        return rep;
    }

    public int getIndexOf(Carte carte) 
    {
        int rep = -1;
        int i = 0;
        while (rep == -1 && i < Cartes.Length)
        {
            if (Cartes[i] == carte)
            {
                rep = i;
            }
            i++;
        }
        return rep;
    }

    public void insertCarteAt(int i, Carte carte) 
    {
        if (i >= Cartes.Length || i < 0)
        {
            throw new IndexOutOfRangeException();
        }
        Carte[] newCartes = new Carte[Cartes.Length + 1];
        for (int j = 0; j < Cartes.Length; j++)
        {
            if (j > i)
            {
                newCartes[j + 1] = Cartes[j];
            }
            else if (j == i)
            {
                newCartes[j] = carte;
                newCartes[j + 1] = Cartes[j];
            }
            else
            {
                newCartes[j] = Cartes[j];
            }
        }
        Cartes = newCartes;
    }

    public void appendCarte(Carte carte) 
    {
        Carte[] newCartes = new Carte[Cartes.Length + 1];
        for (int j = 0; j < Cartes.Length; j++)
        {
            newCartes[j] = Cartes[j];
        }
        newCartes[Cartes.Length] = carte;

        Cartes = newCartes;
    }
}