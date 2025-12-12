using System;
using Microsoft.Xna.Framework.Input;
using System.Xml.Serialization;

namespace CatRoyale.Scripts;

/* Ce qu'on aurait aimer ajouter si on avait plus de temps:
 *      -entrer les pseudo et créer un deck sans le charger
 *      -charger/sauvegarder des joueurs (pseudo + winstreak + deck) mais pas toute la partie (donc pas le plateau ni la main)
 *      -afficher un tableau des highscores (en winstreak)
 *      -qu'une invoc encerclée ne puisse plus bouger
 *      -ajouter un bot en face
 *      -ajouter des sorts et des objets (comme des batiments par exemple)
 *      -gérer les paramètres depuis un XML (pas long mais on y a pensé trop tard)
*/

public enum EtatAutomate { SELECTION_CARTE,SELECTION_CASE_CARTE,SELECTION_CASE_SOURCE,SELECTION_CASE_CIBLE }

[Serializable]
[XmlRoot("jeu", Namespace = "http://www.univ-grenoble-alpes.fr/l3miage/CatRoyaleGame")]
public class Jeu
{
    //données du jeu
    [XmlElement("plateau")] public Plateau Plateau { get; set; }
    [XmlElement("joueur1")] public Joueur Joueur1 { get; set; }
    [XmlElement("joueur2")] public Joueur Joueur2 { get; set; }
    [XmlElement("pseudoJoueurActuel")] public string PseudoJoueurActuel
    {
        get => JoueurActuel.Pseudo;
        set => JoueurActuel = (Joueur1.Pseudo == value)?Joueur1:Joueur2;
    }
    [XmlElement("cartesDuJeu")] public ListeDeCartes CartesExistantes { get; set; }

    //variables globales aux fonctions
    [XmlIgnore] public Joueur JoueurActuel; //sert à gérer les tours
    [XmlIgnore] public EtatAutomate Phase;
    [XmlIgnore] public int CarteI, CaseI, CaseJ, LastCaseI, LastCaseJ;

    //paramètres du jeu (pourrait idéalement être stocké et géré dans un XML)
    private const int MaxDistanceDeplacement = 3;
    private const int MaxDistanceAttaque = 2;
    private const int MaxCarteMain = 10;
    private const int NbCartesPiochees = 2;

    //contructeur avec paramètres non utilisé actuellement mais déja implémenté par sécurité
    public Jeu(int longueur, int largeur, string nom1, string nom2)
    {
        Plateau = new Plateau(longueur, largeur);
        Joueur1 = new Joueur(nom1, 0);
        Joueur2 = new Joueur(nom2, 0);
        CartesExistantes = new ListeDeCartes();
        JoueurActuel = Joueur1;
        CarteI = 0;
        CaseI = -1;
        CaseJ = -1;
        LastCaseI = -1;
        LastCaseJ = -1;
        InitTurn();
    }
    
    //contructeur vide pour le XMLSerializer
    public Jeu()
    {
        Phase = EtatAutomate.SELECTION_CARTE;
        CarteI = 0;
        CaseI = -1;
        CaseJ = -1;
        LastCaseI = -1;
        LastCaseJ = -1;
    }
    
    public bool Victory() { return Plateau.Victory(JoueurActuel); }
    
    //-------------------------gestion des tours-------------------------//
    public void InitTurn()
    {
        //on rempli sa jauge
        JoueurActuel.RemplirJauge();
        int n = 0;
        //tant qu'on a pas pioché trop de carte, qu'on a pas la main pleine, et qu'il nous reste des cartes à piocher dans le deck
        while (n < NbCartesPiochees && JoueurActuel.GetNbCartesInMain() < MaxCarteMain && JoueurActuel.GetNbCartesInDeck() > 0)
        {
            //on pioche
            JoueurActuel.Pioche();
            n++;
        }
        //toutes nos invocs sont de nouveau jouables (sauf la tour)
        for (int i = 0; i < Plateau.Largeur(); i++)
        {
            for (int j = 0; j < Plateau.Longueur(); j++)
            {
                Invocation? invoc = Plateau.GetEntityAt(i, j);
                if (invoc != null && invoc.PseudoInvocateur == JoueurActuel.Pseudo && !Plateau.IsTower(invoc))
                {
                    invoc.PeutAttaquer = true;
                    invoc.PeutBouger = true;
                }
            }
        }
        //puis on commence le tour
        Phase = EtatAutomate.SELECTION_CARTE;
        CarteI = 0;
        CaseI = -1;
        CaseJ = -1;
        LastCaseI = -1;
        LastCaseJ = -1;
    }
    
    public void EndTurn()
    {
        //fonction qui pourrait servir si on ajoute des effets qui s'applique en fin de manche (comme dans le jeu "invocation des 7")
        
        //on fait une sauvegarde automatique
        CatRoyal.SaveGame(CatRoyal.autoSaveFileName);
        //on inverse le joueur actuel
        if (JoueurActuel == Joueur1)
        {
            JoueurActuel = Joueur2;
        }
        else
        {
            JoueurActuel = Joueur1;
        }
        //et on commence son tour
        InitTurn();
    }

    //-------------------------gestion des inputs-------------------------//
    //fonctions pour la lisibilité des inputs et pour pouvoir prendre en charge différentes touches à la fois (pourrait servir pour ajouter les contrôle manette au besoin)
    private bool AppuieSurGauche(KeyboardState current, KeyboardState last)
    {
        return current.IsKeyDown(Keys.Q) && !last.IsKeyDown(Keys.Q)
               || current.IsKeyDown(Keys.Left) && !last.IsKeyDown(Keys.Left);
    }
    private bool AppuieSurDroite(KeyboardState current, KeyboardState last)
    {
        return current.IsKeyDown(Keys.D) && !last.IsKeyDown(Keys.D)
               || current.IsKeyDown(Keys.Right) && !last.IsKeyDown(Keys.Right);
    }
    private bool AppuieSurHaut(KeyboardState current, KeyboardState last)
    {
        return current.IsKeyDown(Keys.Z) && !last.IsKeyDown(Keys.Z)
               || current.IsKeyDown(Keys.Up) && !last.IsKeyDown(Keys.Up);
    }
    private bool AppuieSurBas(KeyboardState current, KeyboardState last)
    {
        return current.IsKeyDown(Keys.S) && !last.IsKeyDown(Keys.S)
               || current.IsKeyDown(Keys.Down) && !last.IsKeyDown(Keys.Down);
    }
    private bool AppuieSurRetour(KeyboardState current, KeyboardState last)
    {
        return current.IsKeyDown(Keys.A) && !last.IsKeyDown(Keys.A)
               || current.IsKeyDown(Keys.Back) && !last.IsKeyDown(Keys.Back);
    }
    private bool AppuieSurValide(KeyboardState current, KeyboardState last)
    {
        return current.IsKeyDown(Keys.E) && !last.IsKeyDown(Keys.E)
               || current.IsKeyDown(Keys.Enter) && !last.IsKeyDown(Keys.Enter);
    }
    private bool AppuieSurFinTour(KeyboardState current, KeyboardState last)
    {
        return current.IsKeyDown(Keys.Space) && !last.IsKeyDown(Keys.Space);
    }
    
    public void Transition(KeyboardState current, KeyboardState last, ClicPhase phaseSouris)
    {
        //déselectionne = met à -1 (et donc pas visible à l'affichage)
        //reset = met à position par défaut (et donc de nouveau visible)
        
        if (AppuieSurFinTour(current,last)){
            //fini le tour
            EndTurn();
            //return pour pas faire le switch (et indenterait éviter l'indentation supplémentaire que causerait un else)
            return;
        }
        switch (Phase)
        {
            case EtatAutomate.SELECTION_CARTE:
                if (JoueurActuel.GetNbCartesInMain() < 1)
                {
                    //si la main est vide, on a pas de carte à selectionner, on passe donc aux mouvements :
                    //passe phase à SELECTION_CASE_SOURCE
                    Phase = EtatAutomate.SELECTION_CASE_SOURCE;
                    //déselectionne carte
                    CarteI = -1;
                    //reset case
                    CaseI = (JoueurActuel==Joueur1) ? 1 : Plateau.Longueur()-2;
                    CaseJ = Plateau.Largeur()/2;

                }
                else if (AppuieSurGauche(current,last) && JoueurActuel == Joueur1 || AppuieSurDroite(current,last) &&  JoueurActuel == Joueur2){
                    //décrémente carteI
                    CarteI = (CarteI>0) ? CarteI-1 : JoueurActuel.GetNbCartesInMain()-1;
                }
                else if (AppuieSurDroite(current,last) && JoueurActuel == Joueur1 || AppuieSurGauche(current,last) &&  JoueurActuel == Joueur2){
                    //incrément carteI
                    CarteI = (CarteI<JoueurActuel.GetNbCartesInMain()-1) ? CarteI+1 : 0;
                }
                else if ((AppuieSurValide(current,last) || phaseSouris == ClicPhase.CONFIRMER_CARTE) && PeutSelectionnerCarte(CarteI)){
                    //passe phase à SELECTION_CASE_CARTE
                    Phase = EtatAutomate.SELECTION_CASE_CARTE;
                    //reset case
                    CaseI = (JoueurActuel==Joueur1) ? 1 : Plateau.Longueur()-2;
                    CaseJ = Plateau.Largeur()/2;
                }
                else if (AppuieSurHaut(current,last) && JoueurActuel ==  Joueur1 || AppuieSurBas(current,last) && JoueurActuel ==  Joueur2 || phaseSouris == ClicPhase.CLIC_SUR_CASE_INVOCATION){
                    //passe phase à SELECTION_CASE_SOURCE
                    Phase = EtatAutomate.SELECTION_CASE_SOURCE;
                    //déselectionne carte
                    CarteI = -1;
                    if (phaseSouris != ClicPhase.CLIC_SUR_CASE_INVOCATION)
                    {
                        //reset case
                        CaseI = (JoueurActuel==Joueur1) ? 1 : Plateau.Longueur()-2;
                        CaseJ = Plateau.Largeur()/2;
                    }
                }
                break;
            case EtatAutomate.SELECTION_CASE_CARTE:
                if (AppuieSurGauche(current,last) && (JoueurActuel == Joueur1 || CaseI > Plateau.Longueur()/2)){
                    //décrémente caseI
                    CaseI = (CaseI>0) ? CaseI-1 : CaseI;
                }
                else if (AppuieSurDroite(current,last) && (JoueurActuel == Joueur2 || CaseI+1 < Plateau.Longueur()/2)){
                    //incrémente caseI
                    CaseI = (CaseI<Plateau.Longueur()-1) ? CaseI+1 : CaseI;
                }
                else if (AppuieSurHaut(current,last)){
                    //décrémente caseJ
                    CaseJ = (CaseJ>0) ? CaseJ-1 : CaseJ;
                }
                else if (AppuieSurBas(current,last)){
                    //incrémente caseJ
                    CaseJ = (CaseJ<Plateau.Largeur()-1) ? CaseJ+1 : CaseJ;
                }
                else if ((AppuieSurValide(current,last) || phaseSouris == ClicPhase.CONFIRMER_CASE) && PeutInvoquer(CarteI,CaseJ,CaseI))
                {
                    //passe phase à SELECTION_CARTE
                    Phase = EtatAutomate.SELECTION_CARTE;
                    //invoque la carte sur la case
                    Plateau.Invoke(JoueurActuel,JoueurActuel.GetCarteInMainAt(CarteI,CartesExistantes),CaseJ,CaseI);
                    //consomme le mana
                    JoueurActuel.ReduireJauge(JoueurActuel.GetCarteInMainAt(CarteI,CartesExistantes).Cout);
                    //retire la carte de la main et la remet dans le deck
                    JoueurActuel.PutCarteInDeckFromMainAt(CarteI);
                    
                    //déselectionne la case
                    CaseI = -1;
                    CaseJ = -1;
                    //reset carte
                    CarteI = 0;
                }

                else if (AppuieSurRetour(current,last) || phaseSouris == ClicPhase.ANNULER)
                {
                    //passe phase à SELECTION_CARTE
                    Phase = EtatAutomate.SELECTION_CARTE;
                    //déselectionne la case
                    CaseI = -1;
                    CaseJ = -1;
                }
                break;
            case EtatAutomate.SELECTION_CASE_SOURCE:
                if (AppuieSurGauche(current,last)){
                    //décrémente caseI
                    CaseI = (CaseI>0) ? CaseI-1 : CaseI;
                }
                else if (AppuieSurDroite(current,last)){
                    //incrémente caseI
                    CaseI = (CaseI<Plateau.Longueur()-1) ? CaseI+1 : CaseI;
                }
                else if (AppuieSurHaut(current,last)){
                    //décrémente caseJ
                    CaseJ = (CaseJ>0) ? CaseJ-1 : CaseJ;
                }
                else if (AppuieSurBas(current,last)){
                    //incrémente caseJ
                    CaseJ = (CaseJ<Plateau.Largeur()-1) ? CaseJ+1 : CaseJ;
                }
                else if ((AppuieSurValide(current,last) || phaseSouris == ClicPhase.CONFIRMER_CASE) && PeutSelectionnerInvocation(CaseJ,CaseI))
                {
                    //pass phase à SELECTION_CASE_CIBLE
                    Phase = EtatAutomate.SELECTION_CASE_CIBLE;
                    //positionne lastCase à case
                    LastCaseI = CaseI;
                    LastCaseJ = CaseJ;
                }
                else if ((AppuieSurRetour(current,last) || phaseSouris == ClicPhase.ANNULER) && JoueurActuel.GetNbCartesInMain() > 0)
                {
                    //pass phase à SELECTION_CARTE
                    Phase = EtatAutomate.SELECTION_CARTE;
                    //déselectionne case
                    CaseI = -1;
                    CaseJ = -1;
                    //reset carte
                    CarteI = 0;
                }
                break;
            case EtatAutomate.SELECTION_CASE_CIBLE:
                if (AppuieSurGauche(current,last)){
                    //décrémente caseI
                    CaseI = (CaseI>0) ? CaseI-1 : CaseI;
                }
                else if (AppuieSurDroite(current,last)){
                    //incrémente caseI
                    CaseI = (CaseI<Plateau.Longueur()-1) ? CaseI+1 : CaseI;
                }
                else if (AppuieSurHaut(current,last)){
                    //décrémente caseJ
                    CaseJ = (CaseJ>0) ? CaseJ-1 : CaseJ;
                }
                else if (AppuieSurBas(current,last)){
                    //incrémente caseJ
                    CaseJ = (CaseJ<Plateau.Largeur()-1) ? CaseJ+1 : CaseJ;
                }
                else if ((AppuieSurValide(current,last) || phaseSouris == ClicPhase.CONFIRMER_CASE) && PeutAttaquerOuDeplacer(LastCaseJ,LastCaseI,CaseJ,CaseI))
                {
                    //passe phase SELECTION_CASE_SOURCE
                    Phase = EtatAutomate.SELECTION_CASE_SOURCE;
                    //attaque ou déplace l'invoc en prevCase vers case
                    AttaqueOuDeplace(LastCaseJ, LastCaseI,CaseJ,CaseI);
                    //déselectionne prevCase
                    LastCaseI = -1;
                    LastCaseJ = -1;
                }

                else if (AppuieSurRetour(current,last)|| phaseSouris == ClicPhase.ANNULER)
                {
                    //passe phase SELECTION_CASE_SOURCE
                    Phase = EtatAutomate.SELECTION_CASE_SOURCE;
                    //déselectionne prevCase
                    LastCaseI = -1;
                    LastCaseJ = -1;
                }
                break;
        }
    }

    //-------------------------gérer les vérifications-------------------------//
    //pour l'ergonomie, pour éviter qu'on puisse séléctionner une carte non jouable
    public bool PeutSelectionnerCarte(int i)
    {
        Carte carte = JoueurActuel.GetCarteInMainAt(i,CartesExistantes);
        return JoueurActuel.Jauge >= carte.Cout;
    }
    
    //pour l'ergonomie, pour éviter qu'on puisse séléctionner une invocation non jouable
    public bool PeutSelectionnerInvocation(int lig, int col)
    {
        Invocation? source = Plateau.GetEntityAt(lig, col);
        return source != null && source.PseudoInvocateur == JoueurActuel.Pseudo && (source.PeutAttaquer || source.PeutBouger);
    }
    
    public bool PeutInvoquer(int i, int lig, int col)
    {
        bool rep = false;
        Carte carte = JoueurActuel.GetCarteInMainAt(i, CartesExistantes);

        //verifier que la jauge est suffisante
       if(JoueurActuel.Jauge < carte.Cout) return false;
        // verif que la case d'invocation fait partie de l'endroit possible

        switch (carte.Type)
        {
            case TypeDeCarte.SORT:
                //si case vide sort impossible
                if (Plateau.IsEmpty(lig, col))
                {
                    return false;
                }
                Invocation invoc = Plateau.GetEntityAt(lig, col);
                if (carte.Degat < 0)
                {
                    //le sort de soin ne peut être utilisé que sur ses propres invocations
                    rep =  invoc.PseudoInvocateur == JoueurActuel.Pseudo;
                }
                else
                {
                    //le sort de dégat ne peut être utilisé que sur les invocations adverses
                    rep =  invoc.PseudoInvocateur != JoueurActuel.Pseudo;
                }
                break;
            case TypeDeCarte.COMBATTANT:
                //si la case est vide et dans la zone admissible
                rep = Plateau.IsEmpty(lig, col)
                      && (JoueurActuel == Joueur1 && col < Plateau.Longueur()/2
                          || JoueurActuel == Joueur2 && col+1 > Plateau.Longueur()/2);
                break;
            case TypeDeCarte.OBJET:
                rep = false; //pas encore implémenté les objets
                break;
        }
        return rep;
    }

    public bool PeutAttaquer(int lig1, int col1, int lig2, int col2)
    {
        Invocation? source = Plateau.GetEntityAt(lig1, col1);
        Invocation? cible = Plateau.GetEntityAt(lig2, col2);
        int distance = Math.Abs(lig2-lig1)+Math.Abs(col2-col1);
        return distance <= MaxDistanceAttaque && source != null && cible != null && source.PseudoInvocateur==JoueurActuel.Pseudo && cible.PseudoInvocateur != JoueurActuel.Pseudo && source.PeutAttaquer;
    }
    
    public bool PeutDeplacer(int lig1, int col1, int lig2, int col2)
    {
        Invocation? source = Plateau.GetEntityAt(lig1, col1);
        int distance = Math.Abs(lig2-lig1)+Math.Abs(col2-col1);
        return distance <= MaxDistanceDeplacement && source != null && Plateau.IsEmpty(lig2,col2) && source.PseudoInvocateur==JoueurActuel.Pseudo && source.PeutBouger;
    }
    
    public bool PeutAttaquerOuDeplacer(int lig1, int col1, int lig2, int col2)
    {
     
        return (PeutAttaquer(lig1, col1, lig2, col2) ||  PeutDeplacer(lig1, col1, lig2, col2));
    }

    private void AttaqueOuDeplace(int lig1, int col1, int lig2, int col2)
    {
        if (PeutDeplacer(lig1, col1, lig2, col2))
        {
            Plateau.Move(lig1, col1, lig2, col2);
        }
        else
        {
            Plateau.Attack(lig1, col1, lig2, col2);
            //revenir a l'invoc qui a attaqué
            CaseI = LastCaseI;
            CaseJ = LastCaseJ;
        }
    }
}