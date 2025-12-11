using System;
using System.Xml.Serialization;

namespace TestProjet.Scripts;

[XmlType("ListeIdCartes")]
public class ListeIdCartes
{
    [XmlElement("idCarte")] public string[] IdCartes {get; set;}

    public ListeIdCartes()
    {
        IdCartes = new string[0];
    }

    public int Length()
    {
        if (IdCartes == null) IdCartes = new string[0]; //--------------------------------------------------------------
        return IdCartes.Length;
    }

    public Carte getCarteAt(int i, ListeDeCartes Cartes) 
    {
        if (i >= IdCartes.Length || i < 0)
        {
            throw new IndexOutOfRangeException();
        }

        for (int j = 0; j < Cartes.Length(); j++)
        {
            if (Cartes.getCarteAt(j).Nom == IdCartes[i])
            {
                return Cartes.getCarteAt(j);
            }
        }
        throw new Exception("un id incorrect a été entré dans une ListeIdCartes : "+IdCartes[i]);
    }

    public string getIdAt(int i) 
    {
        if (i >= IdCartes.Length || i < 0)
        {
            throw new IndexOutOfRangeException();
        }
        
        return IdCartes[i];
    }

    public void setCarteAt(int i, Carte carte) 
    {
        if (i >= IdCartes.Length || i < 0)
        {
            throw new IndexOutOfRangeException();
        }
        IdCartes[i] =  carte.Nom;
    }

    public void setIdAt(int i, string carte) 
    {
        if (i >= IdCartes.Length || i < 0)
        {
            throw new IndexOutOfRangeException();
        }
        IdCartes[i] =  carte;
    }

    public Carte removeCarteAt(int i, ListeDeCartes Cartes) 
    {
        if (i >= IdCartes.Length || i < 0)
        {
            throw new IndexOutOfRangeException();
        }
        string rep = IdCartes[i];
        string[] newIdCartes = new string[IdCartes.Length - 1];

        int k = 0;
        for (int j = 0; j < IdCartes.Length; j++)
        {
            if (j == i)
            {
                continue;
            } 

            newIdCartes[k] = IdCartes[j];
            k++;
        }
        /* alternative
        for (int j = 0; j < _cartes.Length; j++)
        {
            if (j > i)
            {
                newIdCartes[j - 1] = _cartes[j];
            }
            else if (j < i)
            {
                newIdCartes[j] = _cartes[j];
            }
        }
        */
        IdCartes = newIdCartes;
        
        

        for (int j = 0; j < Cartes.Length(); j++)
        {
            if (Cartes.getCarteAt(j).Nom == rep)
            {
                return Cartes.getCarteAt(j);
            }
        }
        throw new Exception("un id incorrect a été entré dans une ListeIdCartes : "+IdCartes[i]);
    }

    public string removeIdAt(int i) 
    {
        if (i >= IdCartes.Length || i < 0)
        {
            throw new IndexOutOfRangeException();
        }
        string rep = IdCartes[i];
        string[] newIdCartes = new string[IdCartes.Length - 1];

        int k = 0;
        for (int j = 0; j < IdCartes.Length; j++)
        {
            if (j == i)
            {
                continue;
            } 

            newIdCartes[k] = IdCartes[j];
            k++;
        }
        /* alternative
        for (int j = 0; j < _cartes.Length; j++)
        {
            if (j > i)
            {
                newIdCartes[j - 1] = _cartes[j];
            }
            else if (j < i)
            {
                newIdCartes[j] = _cartes[j];
            }
        }
        */
        IdCartes = newIdCartes;
        return rep;
    }

    public int getIndexOf(Carte carte) 
    {
        int rep = -1;
        int i = 0;
        while (rep == -1 && i < IdCartes.Length)
        {
            if (IdCartes[i] == carte.Nom)
            {
                rep = i;
            }
            i++;
        }
        return rep;
    }

    public int getIndexOf(string carte) 
    {
        int rep = -1;
        int i = 0;
        while (rep == -1 && i < IdCartes.Length)
        {
            if (IdCartes[i] == carte)
            {
                rep = i;
            }
            i++;
        }
        return rep;
    }

    public void insertCarteAt(int i, Carte carte) 
    {
        if (i >= IdCartes.Length || i < 0)
        {
            throw new IndexOutOfRangeException();
        }
        string[] newIdCartes = new string[IdCartes.Length + 1];
        for (int j = 0; j < IdCartes.Length; j++)
        {
            if (j > i)
            {
                newIdCartes[j + 1] = IdCartes[j];
            }
            else if (j == i)
            {
                newIdCartes[j] = carte.Nom;
                newIdCartes[j + 1] = IdCartes[j];
            }
            else
            {
                newIdCartes[j] = IdCartes[j];
            }
        }
        IdCartes = newIdCartes;
    }

    public void insertIdAt(int i, string carte) 
    {
        if (i >= IdCartes.Length || i < 0)
        {
            throw new IndexOutOfRangeException();
        }
        string[] newIdCartes = new string[IdCartes.Length + 1];
        for (int j = 0; j < IdCartes.Length; j++)
        {
            if (j > i)
            {
                newIdCartes[j + 1] = IdCartes[j];
            }
            else if (j == i)
            {
                newIdCartes[j] = carte;
                newIdCartes[j + 1] = IdCartes[j];
            }
            else
            {
                newIdCartes[j] = IdCartes[j];
            }
        }
        IdCartes = newIdCartes;
    }

    public void appendCarte(Carte carte) 
    {
        string[] newIdCartes = new string[IdCartes.Length + 1];
        for (int j = 0; j < IdCartes.Length; j++)
        {
            newIdCartes[j] = IdCartes[j];
        }
        newIdCartes[IdCartes.Length] = carte.Nom;

        IdCartes = newIdCartes;
    }

    public void appendId(string carte) 
    {
        string[] newIdCartes = new string[IdCartes.Length + 1];
        for (int j = 0; j < IdCartes.Length; j++)
        {
            newIdCartes[j] = IdCartes[j];
        }
        newIdCartes[IdCartes.Length] = carte;

        IdCartes = newIdCartes;
    }
}