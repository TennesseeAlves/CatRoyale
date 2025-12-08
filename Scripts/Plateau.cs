using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace TestProjet.Scripts;


//J'ai tout revu à cause de la serialization.
//Il y avait très certainement bien mieux et très facilement, mais j'ai pas trouvé et j'en ai eu marre donc j'ai tout refait de 0.

//classes intermédiaire pour la sérialization
[XmlType("CasePlateau")]
public class CasePlateau
{
    [XmlAttribute("estVide")] public bool EstVide { get; set; }
    [XmlElement("invocation")] public Invocation? Invoc { get; set; }

    public CasePlateau()
    {
        EstVide = true;
        Invoc = null;
    }

    public CasePlateau(bool estVide, Invocation? invocation)
    {
        EstVide = estVide;
        Invoc = invocation;
    }
}
[XmlType("LigneDeCases")]
public class LigneDeCases
{
    [XmlElement("case")] public List<CasePlateau> Cases { get; set; }

    public LigneDeCases()
    {
        Cases = new List<CasePlateau>();
    }
}


[XmlType("Plateau")]
public class Plateau
{
    [XmlIgnore] public Invocation TowerJ1 { get; set; }
    [XmlIgnore] public Invocation TowerJ2 { get; set; }

    [XmlElement("ligneDeCases")]
    public List<LigneDeCases> Map
    {
        get => _map;
        set
        {
            int largeur = value.Count;
            int longueur = value[0].Cases.Count;

            for (int y = 0; y < largeur; y++)
            {
                for (int x = 0; x < longueur; x++)
                {
                    _map[y].Cases[x] = value[y].Cases[x];
                }
            }
            TowerJ1 = _map[largeur/2].Cases[1].Invoc;
            TowerJ1 = _map[largeur/2].Cases[longueur-2].Invoc;
        }
    }
    //champs internes
    [XmlIgnore] private List<LigneDeCases> _map;

    public Plateau(int longueur, int largeur)
    {
        List<LigneDeCases> lignes = new List<LigneDeCases>();
        for (int i = 0; i < largeur; i++)
        {
            LigneDeCases ligne = new LigneDeCases();
            for (int j = 0; j < longueur; j++)
            {
                ligne.Cases.Add(new CasePlateau());
            }
            lignes.Add(ligne);
        }
        _map = lignes;
    }
    
    //contructeur vide pour le XMLSerializer
    public Plateau() { }

    public int Longueur()
    {
        return Map[0].Cases.Count;
    }

    public int Largeur()
    {
        return Map.Count;
    }

    public void setEntityAt(Invocation invoc, int ligne, int colonne)
    {
        Map[ligne].Cases[colonne].EstVide = (invoc==null);
        Map[ligne].Cases[colonne].Invoc = invoc;
    }
    public Invocation? getEntityAt(int ligne, int colonne)
    {
        if (ligne >= Largeur() || ligne < 0 || colonne >= Longueur() || colonne < 0)
        {
            throw new IndexOutOfRangeException();
        }
        return Map[ligne].Cases[colonne].Invoc;
    }

    public bool isEmpty(int ligne, int colonne)
    {
        if (ligne >= Largeur() || ligne < 0 || colonne >= Longueur() || colonne < 0)
        {
            throw new IndexOutOfRangeException();
        }
        return Map[ligne].Cases[colonne].EstVide;
    }

    public void deleteAt(int ligne, int colonne)
    {
        if (ligne >= Largeur() || ligne < 0 || colonne >= Longueur() || colonne < 0)
        {
            throw new IndexOutOfRangeException();
        }
        Map[ligne].Cases[colonne].EstVide = true;
        Map[ligne].Cases[colonne].Invoc = null;
    }

    public void invoke(Joueur joueur, Carte carte, int ligne, int colonne)
    {   
        Map[ligne].Cases[colonne].EstVide = false;
        Map[ligne].Cases[colonne].Invoc = carte.generateInvocation();
        Map[ligne].Cases[colonne].Invoc.Invocateur = joueur;
    }

    public void move(int ligneDepart, int colonneDepart, int ligneArrive, int colonneArrive)
    {
        Map[ligneArrive].Cases[colonneArrive].EstVide = false;
        Map[ligneArrive].Cases[colonneArrive].Invoc = Map[ligneDepart].Cases[colonneDepart].Invoc;
        Map[ligneDepart].Cases[colonneDepart].EstVide = true;
        Map[ligneDepart].Cases[colonneDepart].Invoc = null;
        Map[ligneArrive].Cases[colonneArrive].Invoc.PeutBouger = false;
    }

    public void attack(int ligneDepart, int colonneDepart, int ligneArrive, int colonneArrive)
    {
        //effectue l'attaque et vérifie s'il est mort
        if (Map[ligneArrive].Cases[colonneArrive].Invoc.takeDamage(Map[ligneDepart].Cases[colonneDepart].Invoc.Degat))
        {
            Map[ligneArrive].Cases[colonneArrive].EstVide = true;
            Map[ligneArrive].Cases[colonneArrive].Invoc = null;
        }
        Map[ligneDepart].Cases[colonneDepart].Invoc.PeutAttaquer = false;
    }

    public bool victory(Joueur joueur)
    {
        return TowerJ1.Invocateur == joueur && TowerJ2.Vie == 0 || TowerJ2.Invocateur == joueur && TowerJ1.Vie == 0;
    }

    public bool isTower(Invocation invoc)
    {
        return invoc == TowerJ1 || invoc == TowerJ2;
    }
}