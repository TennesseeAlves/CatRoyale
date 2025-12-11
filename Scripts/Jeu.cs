using System;
using Microsoft.Xna.Framework.Input;
using System.Xml.Serialization;

namespace TestProjet.Scripts;


/* Ce qu'on voudrait pouvoir faire:
 *      -entrer les pseudo et charger le deck par défaut (et mettre winstreak à 0) si on ne charge pas une partie
 *      -charger/sauvegarder des joueurs (pseudo + winstreak + deck) mais pas une partie (donc pas plateau ni main)
 *      -charger/sauvegarder une partie (en cours ou fini) depuis/vers un XML
 *      -afficher un tableau des highscores (en winstreak)
 *      -controler à la souris
 *      -voir les stats de nos cartes et invoc (vie, attaque, si elles sont jouables...)
 *      -encercler une invoc et qu'elle ne puisse plus bouger
*/
/*
 * Problèmes restants:
 *                      -pas de load and save
 *                      -pas de deck
 *                      -pas de menu (parties en boucles)
 *                      -doit copier coller controle souris et faire partie sauvegarde
 *                      -deplacement en noclip
 *                      -mana max écrit en dur dans l'affichage
 *                      -pas de bot en face?
 *                      -sort de vie et de dégats (degat negatif pour vie)
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
    [XmlIgnore] public Joueur JoueurActuel; //pour savoir à qui c'est le tour
    [XmlIgnore] public EtatAutomate Phase;
    [XmlIgnore] public int CarteI, CaseI, CaseJ, LastCaseI, LastCaseJ;

    //paramètres du jeu (à voir ce qu'on a fait)
    private int maxDistanceDeplacement = 3;
    private int maxDistanceAttaque = 2;
    private int vieTour = 30;
    private int maxCarteMain = 10;
    private int nbCartesPiochees = 2;

    //longeur doit être pair et strictement supérieur à 2, largeur doit être impair et srtictement supérieur à 1
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
    
    public Jeu()
    {
        Phase = EtatAutomate.SELECTION_CARTE;
        CarteI = 0;
        CaseI = -1;
        CaseJ = -1;
        LastCaseI = -1;
        LastCaseJ = -1;
    }
    
    //-------------------------accesseur---------------------------//
    public bool victory() { return Plateau.victory(JoueurActuel); }
    
    public int Longueur()
    {
        return Plateau.Longueur();
    }
    
    public int Largeur()
    {
        return Plateau.Largeur();
    }
    //-------------------------turnManager
    //début de tour(remplir jauge, piocher cartes, actualiser les peutBouger et peut Attaquer(mais pas du cristal))
    public void InitTurn()
    {
        //on rempli notre jauge
        JoueurActuel.remplirJauge();
        int n = 0;
        //tant qu'on a pas pioché trop de carte, qu'on a pas la main pleine, et qu'il nous reste des cartes à piocher dans le deck
        while (n < nbCartesPiochees && JoueurActuel.getNbCartesInMain() < maxCarteMain && JoueurActuel.getNbCartesInDeck() > 0)
        {
            //on pioche
            JoueurActuel.pioche();
            n++;
        }
        //toutes nos invocs sont de nouveau jouables
        for (int i = 0; i < Plateau.Largeur(); i++)
        {
            for (int j = 0; j < Plateau.Longueur(); j++)
            {
                Invocation? invoc = Plateau.getEntityAt(i, j);
                if (invoc != null && invoc.PseudoInvocateur == JoueurActuel.Pseudo && !Plateau.isTower(invoc))
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
    //fin de tour(passer la main à l'autre joueur)
    public void EndTurn()
    {

        CatRoyal.SaveGame(CatRoyal.autoSaveFileName);
        
        //fonction à garder pour si on ajoute des effets qui s'applique en fin de manche (comme dans l'invocation des 7)
        if (JoueurActuel == Joueur1)
        {
            JoueurActuel = Joueur2;
        }
        else
        {
            JoueurActuel = Joueur1;
        }
        InitTurn();
    }

    //-------------------------inputManager---------------------------//
    //---CLAVIER---//
    //fonctions pour la lisibilité des inputs
    private bool appuieSurGauche(KeyboardState current, KeyboardState last)
    {
        return current.IsKeyDown(Keys.Q) && !last.IsKeyDown(Keys.Q)
               || current.IsKeyDown(Keys.Left) && !last.IsKeyDown(Keys.Left);
    }
    private bool appuieSurDroite(KeyboardState current, KeyboardState last)
    {
        return current.IsKeyDown(Keys.D) && !last.IsKeyDown(Keys.D)
               || current.IsKeyDown(Keys.Right) && !last.IsKeyDown(Keys.Right);
    }
    private bool appuieSurHaut(KeyboardState current, KeyboardState last)
    {
        return current.IsKeyDown(Keys.Z) && !last.IsKeyDown(Keys.Z)
               || current.IsKeyDown(Keys.Up) && !last.IsKeyDown(Keys.Up);
    }
    private bool appuieSurBas(KeyboardState current, KeyboardState last)
    {
        return current.IsKeyDown(Keys.S) && !last.IsKeyDown(Keys.S)
               || current.IsKeyDown(Keys.Down) && !last.IsKeyDown(Keys.Down);
    }
    private bool appuieSurRetour(KeyboardState current, KeyboardState last)
    {
        return current.IsKeyDown(Keys.A) && !last.IsKeyDown(Keys.A)
               || current.IsKeyDown(Keys.Back) && !last.IsKeyDown(Keys.Back);
    }
    private bool appuieSurValide(KeyboardState current, KeyboardState last)
    {
        return current.IsKeyDown(Keys.E) && !last.IsKeyDown(Keys.E)
               || current.IsKeyDown(Keys.Enter) && !last.IsKeyDown(Keys.Enter);
    }
    private bool appuieSurFinTour(KeyboardState current, KeyboardState last)
    {
        return current.IsKeyDown(Keys.Space) && !last.IsKeyDown(Keys.Space);
    }
    
    public void transition(KeyboardState current, KeyboardState last, int phase)
    {
        //déselectionne = met à -1 (donc pas visible à l'affichage)
        //reset = met à 0 (donc de nouveau visible et pos par défaut)
        
        if (appuieSurFinTour(current,last)){
            //fini le tour
            EndTurn();
            //return pour pas faire le switch (un else indenterait trop)
            return;
        }
        switch (Phase)
        {
            case EtatAutomate.SELECTION_CARTE:
                if (JoueurActuel.getNbCartesInMain() < 1)
                {
                    //si la main est vide, on a pas de carte à selectionner, on passe donc aux mouvements :
                    //passe phase à SELECTION_CASE_SOURCE
                    Phase = EtatAutomate.SELECTION_CASE_SOURCE;
                    //déselectionne carte
                    CarteI = -1;
                    //reset case
                    CaseI = (JoueurActuel==Joueur1)?1:Plateau.Longueur()-2;
                    CaseJ = Plateau.Largeur()/2;

                }
                else if (appuieSurGauche(current,last) && JoueurActuel == Joueur1 || appuieSurDroite(current,last) &&  JoueurActuel == Joueur2){
                    //décrémente carteI
                    CarteI = (CarteI>0) ? CarteI-1 : JoueurActuel.getNbCartesInMain()-1;
                }
                else if (appuieSurDroite(current,last) && JoueurActuel == Joueur1 || appuieSurGauche(current,last) &&  JoueurActuel == Joueur2){
                    //incrément carteI
                    CarteI = (CarteI<JoueurActuel.getNbCartesInMain()-1) ? CarteI+1 : 0;
                }
                else if ((appuieSurValide(current,last) || phase == 0) && peutSelectionnerCarte(CarteI)){
                    //passe phase à SELECTION_CASE_CARTE
                    Phase = EtatAutomate.SELECTION_CASE_CARTE;
                    //reset case
                    CaseI = (JoueurActuel==Joueur1)?1:Plateau.Longueur()-2;
                    CaseJ = Plateau.Largeur()/2;
                }
                else if ((appuieSurHaut(current,last) && JoueurActuel ==  Joueur1) || (appuieSurBas(current,last) && JoueurActuel ==  Joueur2) || phase == 1){
                    //passe phase à SELECTION_CASE_SOURCE
                    Phase = EtatAutomate.SELECTION_CASE_SOURCE;
                    //déselectionne carte
                    CarteI = -1;
                    //reset case
                    CaseI = (JoueurActuel==Joueur1)?1:Plateau.Longueur()-2;
                    CaseJ = Plateau.Largeur()/2;
                }
                break;
            case EtatAutomate.SELECTION_CASE_CARTE:
                if (appuieSurGauche(current,last) && (JoueurActuel == Joueur1 || CaseI > Plateau.Longueur()/2)){
                    //décrémente caseI
                    CaseI = (CaseI>0) ? CaseI-1 : CaseI;
                }
                else if (appuieSurDroite(current,last) && (JoueurActuel == Joueur2 || CaseI+1 < Plateau.Longueur()/2)){
                    //incrémente caseI
                    CaseI = (CaseI<Plateau.Longueur()-1) ? CaseI+1 : CaseI;
                }
                else if (appuieSurHaut(current,last)){
                    //décrémente caseJ
                    CaseJ = (CaseJ>0) ? CaseJ-1 : CaseJ;
                }
                else if (appuieSurBas(current,last)){
                    //incrémente caseJ
                    CaseJ = (CaseJ<Plateau.Largeur()-1) ? CaseJ+1 : CaseJ;
                }
                else if ((appuieSurValide(current,last) || phase == 2) && peutInvoquer(CarteI,CaseJ,CaseI))
                {
                    //passe phase à SELECTION_CARTE
                    Phase = EtatAutomate.SELECTION_CARTE;
                    //invoque la carte sur la case
                    Plateau.invoke(JoueurActuel,JoueurActuel.getCarteInMainAt(CarteI,CartesExistantes),CaseJ,CaseI);
                    //consomme le mana
                    JoueurActuel.reduireJauge(JoueurActuel.getCarteInMainAt(CarteI,CartesExistantes).Cout);
                    //retire la carte de la main et la remet dans le deck
                    JoueurActuel.putCarteInDeckFromMainAt(CarteI);
                    
                    //déselectionne la case
                    CaseI = -1;
                    CaseJ = -1;
                    //reset carte
                    CarteI = 0;
                }

                else if (appuieSurRetour(current,last)||phase == -1)
                {
                    //passe phase à SELECTION_CARTE
                    Phase = EtatAutomate.SELECTION_CARTE;
                    //déselectionne la case
                    CaseI = -1;
                    CaseJ = -1;
                }
                break;
            case EtatAutomate.SELECTION_CASE_SOURCE:
                if (appuieSurGauche(current,last)){
                    //décrémente caseI
                    CaseI = (CaseI>0) ? CaseI-1 : CaseI;
                }
                else if (appuieSurDroite(current,last)){
                    //incrémente caseI
                    CaseI = (CaseI<Plateau.Longueur()-1) ? CaseI+1 : CaseI;
                }
                else if (appuieSurHaut(current,last)){
                    //décrémente caseJ
                    CaseJ = (CaseJ>0) ? CaseJ-1 : CaseJ;
                }
                else if (appuieSurBas(current,last)){
                    //incrémente caseJ
                    CaseJ = (CaseJ<Plateau.Largeur()-1) ? CaseJ+1 : CaseJ;
                }
                else if ((appuieSurValide(current,last) || phase == 3) && peutSelectionnerInvocation(CaseJ,CaseI))
                {
                    //pass phase à SELECTION_CASE_CIBLE
                    Phase = EtatAutomate.SELECTION_CASE_CIBLE;
                    //positionne lastCase à case
                    LastCaseI = CaseI;
                    LastCaseJ = CaseJ;
                }
                else if (appuieSurRetour(current,last)|| phase == -1 && JoueurActuel.getNbCartesInMain() > 0)
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
                if (appuieSurGauche(current,last)){
                    //décrémente caseI
                    CaseI = (CaseI>0) ? CaseI-1 : CaseI;
                }
                else if (appuieSurDroite(current,last)){
                    //incrémente caseI
                    CaseI = (CaseI<Plateau.Longueur()-1) ? CaseI+1 : CaseI;
                }
                else if (appuieSurHaut(current,last)){
                    //décrémente caseJ
                    CaseJ = (CaseJ>0) ? CaseJ-1 : CaseJ;
                }
                else if (appuieSurBas(current,last)){
                    //incrémente caseJ
                    CaseJ = (CaseJ<Plateau.Largeur()-1) ? CaseJ+1 : CaseJ;
                }
                else if ((appuieSurValide(current,last) || phase == 4) && peutAttaquerOuDeplacer(LastCaseJ,LastCaseI,CaseJ,CaseI))
                {
                    //passe phase SELECTION_CASE_SOURCE
                    Phase = EtatAutomate.SELECTION_CASE_SOURCE;
                    //attaque ou déplace l'invoc en prevCase vers case
                    AttaqueOuDeplace(LastCaseJ, LastCaseI,CaseJ,CaseI);
                    //déselectionne prevCase
                    LastCaseI = -1;
                    LastCaseJ = -1;
                }

                else if (appuieSurRetour(current,last)|| phase == -1)
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
    
    //---SOURIS---//
    //TO DO


    //-------------------------testManager---------------------------//
    //pour l'ergonomie, pour éviter qu'on puisse séléctionner une carte non jouable
    public bool peutSelectionnerCarte(int i)
    {
        Carte carte = JoueurActuel.getCarteInMainAt(i,CartesExistantes);
        return JoueurActuel.Jauge >= carte.Cout;
    }
    //pour l'ergonomie, pour éviter qu'on puisse séléctionner une invocation non jouable
    private bool peutSelectionnerInvocation(int lig, int col)
    {
        Invocation? source = Plateau.getEntityAt(lig, col);
        return source != null && source.PseudoInvocateur == JoueurActuel.Pseudo && (source.PeutAttaquer || source.PeutBouger);
    }
    public bool peutInvoquer(int i, int lig, int col)
    {
        Carte carte = JoueurActuel.getCarteInMainAt(i,CartesExistantes);
        return Plateau.isEmpty(lig,col) && JoueurActuel.Jauge >= carte.Cout;
    }
    private bool peutAttaquer(int lig1, int col1, int lig2, int col2)
    {
        Invocation? source = Plateau.getEntityAt(lig1, col1);
        Invocation? cible = Plateau.getEntityAt(lig2, col2);
        int distance = Math.Abs(lig2-lig1)+Math.Abs(col2-col1);
        return distance <= maxDistanceAttaque &&  source != null && cible != null && source.PseudoInvocateur==JoueurActuel.Pseudo && cible.PseudoInvocateur != JoueurActuel.Pseudo && source.PeutAttaquer;
    }
    public bool peutDeplacer(int lig1, int col1, int lig2, int col2)
    {
        Invocation? source = Plateau.getEntityAt(lig1, col1);
        int distance = Math.Abs(lig2-lig1)+Math.Abs(col2-col1);
        return distance <= maxDistanceDeplacement && source != null && Plateau.isEmpty(lig2,col2) && source.PseudoInvocateur==JoueurActuel.Pseudo && source.PeutBouger;
    }
    public bool peutAttaquerOuDeplacer(int lig1, int col1, int lig2, int col2)
    
    {
     
        return (peutAttaquer(lig1, col1, lig2, col2) ||  peutDeplacer(lig1, col1, lig2, col2));
    }

    private void AttaqueOuDeplace(int lig1, int col1, int lig2, int col2)
    {
        if (peutDeplacer(lig1, col1, lig2, col2))
        {
            Plateau.move(lig1, col1, lig2, col2);
        }
        else
        {
            Plateau.attack(lig1, col1, lig2, col2);
        }
    }
}