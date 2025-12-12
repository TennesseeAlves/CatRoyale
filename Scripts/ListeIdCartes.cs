using System;
using System.Xml.Serialization;

namespace CatRoyale.Scripts;

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
        if (IdCartes == null) IdCartes = new string[0]; //je n'arrive pas à comprendre la raison exact, mais sans cette ligne-là certaine sauvegarde font crash quand on les charges
        return IdCartes.Length;
    }

    public Carte GetCarteAt(int i, ListeDeCartes cartes) 
    {
        if (i >= IdCartes.Length || i < 0)
        {
            throw new IndexOutOfRangeException();
        }

        return cartes.GetCarteByName(IdCartes[i]);
    }

    public string GetIdAt(int i) 
    {
        if (i >= IdCartes.Length || i < 0)
        {
            throw new IndexOutOfRangeException();
        }
        
        return IdCartes[i];
    }
    
    //fonction inutilisé mais implémenté car toujours utile pour manipuler des listes
    public void SetCarteAt(int i, Carte carte) 
    {
        if (i >= IdCartes.Length || i < 0)
        {
            throw new IndexOutOfRangeException();
        }
        IdCartes[i] =  carte.Nom;
    }

    //fonction inutilisé mais implémenté car toujours utile pour manipuler des listes
    public void SetIdAt(int i, string carte) 
    {
        if (i >= IdCartes.Length || i < 0)
        {
            throw new IndexOutOfRangeException();
        }
        IdCartes[i] =  carte;
    }

    //fonction inutilisé mais implémenté car toujours utile pour manipuler des listes
    public Carte RemoveCarteAt(int i, ListeDeCartes cartes) 
    {
        if (i >= IdCartes.Length || i < 0)
        {
            throw new IndexOutOfRangeException();
        }
        
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

        return cartes.GetCarteByName(IdCartes[i]);
    }

    public string RemoveIdAt(int i) 
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

    public int GetIndexOf(Carte carte) 
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

    public int GetIndexOf(string carte) 
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

    //fonction inutilisé mais implémenté car toujours utile pour manipuler des listes
    public void InsertCarteAt(int i, Carte carte) 
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

    //fonction inutilisé mais implémenté car toujours utile pour manipuler des listes
    public void InsertIdAt(int i, string carte) 
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

    public void AppendCarte(Carte carte) 
    {
        string[] newIdCartes = new string[IdCartes.Length + 1];
        for (int j = 0; j < IdCartes.Length; j++)
        {
            newIdCartes[j] = IdCartes[j];
        }
        newIdCartes[IdCartes.Length] = carte.Nom;

        IdCartes = newIdCartes;
    }

    public void AppendId(string carte) 
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