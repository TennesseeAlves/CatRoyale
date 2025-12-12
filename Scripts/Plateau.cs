using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace CatRoyale.Scripts;

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
    public List<LigneDeCases> Map {get; set;}

    //contructeur avec paramètres non utilisé actuellement mais déja implémenté par sécurité
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
        Map = lignes;
        InitAfterLoad();
    }
    
    //contructeur vide pour le XMLSerializer
    public Plateau()
    {
        Map = new List<LigneDeCases>();
    }
    
    //méthode à appeler après chaque Load
    public void InitAfterLoad()
    {
        int largeur = Map.Count;
        int longueur = Map[0].Cases.Count;

        TowerJ1 = Map[largeur/2].Cases[1].Invoc;
        TowerJ2 = Map[largeur/2].Cases[longueur-2].Invoc;
    }

    public int Longueur()
    {
        if (Largeur() == 0)
        {
            return 0;
        }
        return Map[0].Cases.Count;
    }

    public int Largeur()
    {
        return Map.Count;
    }

    //méthode inutilisée mais implémentée car toujours utile pour manipuler des listes de listes
    public void SetEntityAt(Invocation invoc, int ligne, int colonne)
    {
        Map[ligne].Cases[colonne].EstVide = (invoc==null);
        Map[ligne].Cases[colonne].Invoc = invoc;
    }
    
    public Invocation? GetEntityAt(int ligne, int colonne)
    {
        if (ligne >= Largeur() || ligne < 0 || colonne >= Longueur() || colonne < 0)
        {
            throw new IndexOutOfRangeException();
        }
        return Map[ligne].Cases[colonne].Invoc;
    }

    public bool IsEmpty(int ligne, int colonne)
    {
        if (ligne >= Largeur() || ligne < 0 || colonne >= Longueur() || colonne < 0)
        {
            throw new IndexOutOfRangeException();
        }
        return Map[ligne].Cases[colonne].EstVide;
    }

    //méthode inutilisée mais implémentée car toujours utile pour manipuler des listes de listes
    public void DeleteAt(int ligne, int colonne)
    {
        if (ligne >= Largeur() || ligne < 0 || colonne >= Longueur() || colonne < 0)
        {
            throw new IndexOutOfRangeException();
        }
        Map[ligne].Cases[colonne].EstVide = true;
        Map[ligne].Cases[colonne].Invoc = null;
    }
    
    //on ne gère pas les tests ici, on admet que le test a été effectué avant l'appel
    public void Invoke(Joueur joueur, Carte carte, int ligne, int colonne)
    {
        
        if (carte.Type == TypeDeCarte.SORT)
        {
            //puisque pour l'instant les 2 seuls sorts sont une potion de vie (avec dégat négatif ducoup) et une potion de poison (sans dégat sur la durée)
            Map[ligne].Cases[colonne].Invoc.TakeDamage(carte.Degat);
        }
        else
        {
            Map[ligne].Cases[colonne].EstVide = false;
            Map[ligne].Cases[colonne].Invoc = carte.GenerateInvocation();
            Map[ligne].Cases[colonne].Invoc.PseudoInvocateur = joueur.Pseudo;
        }
        
    }
    
    //on ne gère pas les tests ici, on admet que le test a été effectué avant l'appel
    public void Move(int ligneDepart, int colonneDepart, int ligneArrive, int colonneArrive)
    {
        Map[ligneArrive].Cases[colonneArrive].EstVide = false;
        Map[ligneArrive].Cases[colonneArrive].Invoc = Map[ligneDepart].Cases[colonneDepart].Invoc;
        Map[ligneDepart].Cases[colonneDepart].EstVide = true;
        Map[ligneDepart].Cases[colonneDepart].Invoc = null;
        Map[ligneArrive].Cases[colonneArrive].Invoc.PeutBouger = false;
    }
    
    //on ne gère pas les tests ici, on admet que le test a été effectué avant l'appel
    public void Attack(int ligneDepart, int colonneDepart, int ligneArrive, int colonneArrive)
    {
        CasePlateau caseSource = Map[ligneDepart].Cases[colonneDepart];
        CasePlateau caseCible = Map[ligneArrive].Cases[colonneArrive];
        //effectue l'attaque et la contre-attaque
        bool mortCible = caseCible.Invoc.TakeDamage(caseSource.Invoc.Degat);
        bool mortSource = caseSource.Invoc.TakeDamage(caseCible.Invoc.Degat);
        caseSource.Invoc.PeutAttaquer = false;
        //vérifie s'ils sont morts
        if (mortCible)
        {
            caseCible.EstVide = true;
            caseCible.Invoc = null;
        }
        if (mortSource)
        {
            caseSource.EstVide = true;
            caseSource.Invoc = null;
        }
    }

    public bool Victory(Joueur joueur)
    {
        return TowerJ1.PseudoInvocateur == joueur.Pseudo && TowerJ2.Vie == 0 || TowerJ2.PseudoInvocateur == joueur.Pseudo && TowerJ1.Vie == 0;
    }

    public bool IsTower(Invocation invoc)
    {
        return invoc == TowerJ1 || invoc == TowerJ2;
    }
}