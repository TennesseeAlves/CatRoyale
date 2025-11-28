using System;
using Microsoft.Xna.Framework.Input;

namespace TestProjet.Scripts;


/* Ce qu'on voudrait pouvoir faire:
 *      -commencer son tour             --> piocher(acces main et deck), remplir jauge(acces joueur), rendre invocs jouable(acces plateau.invoc)
 *      -invoquer une carte             --> selection carte(acces main), verification jauge>=cout(acces joueur et carte), verification case vide (acces plateau), spawn entité(acces plateau et carte.invocation), suppression main(acces main), remise dans deck(acces deck)
 *      -deplacer une entité            --> selection invoc(acces plateau), verification invocateur et jouable(acces invoc), verification case vide(acces plateau), deplacer invoc dans tab(acces plateau), mis a jour de "jouable"(acces invoc)
 *      -attaquer avec une entité       --> selection invoc(acces plateau), verification invocateur et jouable(acces invoc), verification cible valide(acces plateau et invoc), actualiser vie(acces invoc(+cible))
 *      -finir son tour                 --> bouton ou touche
 *      -gagner/perdre/finir une partie --> verification reguliere(acces joueur.vie ou plateau.cristaux)
 *      
 *      -sauvegarder/charger un deck depuis un XML
 *      -sauvegarder/charger un joueur(pseudo,winstreak,deck) depuis un XML
 *      -sauvegarder/charger une partie (en cours ou fini, avec un historique des coups) depuis un XML
 *      -afficher un tableau des highscores (en winstreak)
 *
 *
 * Donc, IHM (à peu près):
 *          -selection carte                                                   clavier: .   souris: .
 *          -selection case                                                    clavier: .   souris: .
 *          -selection cible                                                   clavier: .   souris: .
 *          -bouton fin tour                                                   clavier: .   souris: .
 * Main:
 *          -addCarte(Carte carte) : void                                               +
 *          -getCarteAt(int i) : Carte                                                  +
 *          -deleteCarte(Carte carte) : void                                            +
 * Deck:
 *          -piocherCarte() : carte                                                     +
 *          -addCarte(Carte carte) : void                                               +
 * Plateau:
 *          -getEntityAt(int x, int y) : Invocation?                                    +
 *          -isEmpty(int x, int y) : bool                                               +
 *          -invoke(Invocation invoc, int x, int y) : void                              +
 *          -move(int x1, int y1, int x2, int y2) : void                                +
 *          -attack(int x1, int y1, int x2, int y2) : void                              +
 *          (-victoire() : bool?(=joueur_gagnant ou null))                              .
 * Invoc:
 *          -getPeutBouger() : bool                                                     +
 *          -setPeutBouger(bool peutBouger) : void                                      +
 *          -getPeutAttaquer() : bool                                                   +
 *          -setPeutAttaquer(bool peutAttaquer) : void                                  +
 *          -getInvocateur() : Joueur                                                   +
 *          -getAttaque() : int                                                         +
 *          -takeDamage(int degat) : void                                               ~
 * Joueur:
 *          -getJauge() : int                                                           +
 *          -remplirJauge(int niveau) : void                                            +
 *          -reduireJauge(int usage) : void                                             +
*/
/*
 * Problèmes restants:
 *                      -pas de menu (parties en boucles)
 *                      -pas fait le controle souris ni la partie sauvegarde pour l'instant (ni le XML mais bon)
 *                      -pas de deck
 *                      -deplacement en noclip
 *                      -peut invoquer partout
 *                      -critsal indistingable des autres invocs (algoritmiquement)
 *                      -
*/
/*
 * TO DO:
 *          -load and save (donc serialization) --> InitGame et EndGame s'en serviront
 *          -gérer les inputs (clavier & souris) --> MainTurn s'en sert (et peut-être EndGame pour si on veut rejouer, et ptet même InitGame pour si on veut lancer une sauvegarde ou un nouvelle)
 *          -afficher le jeu (pour pouvoir tester notamment) --> les inputs souris en dépendront
 *          -vérifier victoire (et de manière générale accéder au cristal)
 *          -créer la partie d'init (pour pouvoir tester notamment) --> requiert le XML et le load
 *          -créer un premier deck (et les images associés) --> utile pour XML et affichage, pas pour le code
*/
/*
 * Première version jouable :
 *                              -pas de menu --> pas de choix (relance des parties en boucles et pas de sauvegarde)
 *                              -deck minimal --> une carte à 5HP 1ATK cout3 et un à 2HP 2ATK cout2
 *                              -pas d'IA --> le joueur controle les 2 joueurs
 *                              -pas d'extra --> ni effet de carte, ni grosse animations, ni deathmatch en endgame...
 * nécéssite:
 *              -XML            --> pas à moi
 *              -load           --> à moi mais nécéssite XML
 *              -affichage      --> pas à moi
 *              -inputs         --> à moi, clavier OK mais souris nécéssite affichage
 *              -verif victoire --> à moi
 *              -verif actions  --> à moi
 *              -changer logique--> à moi
 */


/* Traduction logique de ce code --> logique MonoGame
 *
 * MonoGame.Constructeur() --> Jeu.Constructeur()
 * MonoGame.Init() --> Jeu.InitGame() mais pas d'appel à MainTurn
 * MonoGame.LoadContent() --> rien à faire pour moi
 * MonoGame.Update() ~~> Jeu.MainTurn(), un if(Victory) then Jeu.EndGame() (donc peut-être un nouveau InitGame derrière) else Jeu.Transition() (qui appelera EndTurn, qui lui même appelera InitTurn)
 * MonoGame.Draw() --> pas besoin pour moi
 */

public enum EtatAutomate { SELECTION_CARTE,SELECTION_CASE_CARTE,SELECTION_CASE_SOURCE,SELECTION_CASE_CIBLE }

public class Jeu
{
    //données du jeu
    private Plateau _plateau;
    private Joueur _joueur1;
    private Joueur _joueur2;

    //variables globales aux fonctions
    private Joueur _joueurActuel; //pour savoir à qui c'est le tour
    private EtatAutomate _phase;
    private int _carteI, _caseI, _caseJ,  _lastCaseI, _lastCaseJ;

    //paramètres du jeu
    private string defaultSaveFileName = "InitGame.xml";
    private int maxJauge = 10;
    private int maxCarteMain = 6;
    private int nbCartesPiochees = 2;

    //longeur doit être pair et strictement supérieur à 2, largeur doit être impair et srtictement supérieur à 1
    public Jeu(int longueur, int largeur, string nom1, string nom2)
    {
        _plateau = new Plateau(longueur, largeur);
        _joueur1 = new Joueur(nom1, 0);
        _joueur2 = new Joueur(nom2, 0);
        _joueurActuel = _joueur1;
        _carteI = 0;
        _caseI = -1;
        _caseJ = -1;
        _lastCaseI = -1;
        _lastCaseJ = -1;
        InitGame(defaultSaveFileName);
    } 
    
    //-------------------------accesseur---------------------------//
    public Plateau plateau() { return _plateau; }
    public Joueur joueur1() { return _joueur1; }
    public Joueur joueur2() { return _joueur2; }
    public Joueur joueurActuel() { return _joueurActuel; }
    public EtatAutomate phase() { return _phase; }
    public int carteI() { return _carteI; }
    public int caseI() { return _caseI; }
    public int caseJ() { return _caseJ; }
    public int lastCaseI() { return _lastCaseI; }
    public int lastCaseJ() { return _lastCaseJ; }
    public bool victory() { return _plateau.victory(_joueurActuel); }
    
    //-------------------------turnManager---------------------------//
    //début de partie(charger carte et invoc, placer les cristaux, remplir les jauges, piocher les cartes pour tout le monde, donner la main J1)
    public void InitGame(string SaveFileName)
    {
        //load game (soit une en cours qui a été sauvegarder, soit par défaut une partie au tour 1 avec rien de placé et les jauges rempli)
        LoadGame(SaveFileName);
    }
    //début de tour(remplir jauge, piocher cartes, actualiser les peutBouger et peut Attaquer(mais pas du cristal))
    public void InitTurn()
    {
        //on rempli notre jauge
        _joueurActuel.remplirJauge(maxJauge);
        int n = 0;
        //tant qu'on a pas pioché trop de carte, qu'on a pas la main pleine, et qu'il nous reste des cartes à piocher dans le deck
        while (n < nbCartesPiochees && _joueurActuel.getNbCartesInMain() < maxCarteMain && _joueurActuel.getNbCartesInDeck() > 0)
        {
            //on pioche
            _joueurActuel.pioche();
            n++;
        }
        //toutes nos invocs sont de nouveau jouables
        for (int i = 0; i < _plateau.getLargeur(); i++)
        {
            for (int j = 0; j < _plateau.getLongueur(); j++)
            {
                Invocation? invoc = _plateau.getEntityAt(i, j);
                if (invoc != null && invoc.getInvocateur() == _joueurActuel && !invoc.isCristal())
                {
                    invoc.setPeutAttaquer(true);
                    invoc.setPeutBouger(true);
                }
            }
        }
        //puis on commence le tour
        _phase = EtatAutomate.SELECTION_CARTE;
        _carteI = 0;
        _caseI = -1;
        _caseJ = -1;
        _lastCaseI = -1;
        _lastCaseJ = -1;
    }
    //verification pendant le tour(mana suffisant pour carte, case contenant une invocation ou non, invocation appartenant au bon joueur, invocation peutJouer ou peutBouger, partie fini/gagné)
    public void MainTurn(){}
    //fin de tour(passer la main à l'autre joueur)
    public void EndTurn()
    {
        //fonction à garder pour si on ajoute des effets qui s'applique en fin de manche (comme dans l'invocation des 7)
        if (_joueurActuel == _joueur1)
        {
            _joueurActuel = _joueur2;
        }
        else
        {
            _joueurActuel = _joueur1;
        }
        InitTurn();
    }
    //fin de partie(sauvegarder?)
    public void EndGame()
    {
        //on augmente la winstreak du gagnant
        _joueurActuel.setWinStreak(_joueurActuel.getWinStreak()+1);
        //on actualise le fichier des highscore
        //si le joueur veut encore jouer
            //alors on relance une nouvelle game de 0
            InitGame(defaultSaveFileName);
        //sinon
            //quitter le jeu/retourner au menu
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
    
    public void transition(KeyboardState current, KeyboardState last)
    {
        //déselectionne = met à -1 (donc pas visible à l'affichage)
        //reset = met à 0 (donc de nouveau visible et pos par défaut)
        
        if (appuieSurFinTour(current,last)){
            //fini le tour
            EndTurn();
            //return pour pas faire le switch (un else indenterait trop)
            return;
        }
        switch (_phase)
        {
            case EtatAutomate.SELECTION_CARTE:
                if (_joueurActuel.getNbCartesInMain() < 1)
                {
                    //si la main est vide, on a pas de carte à selectionner, on passe donc aux mouvements :
                    //passe phase à SELECTION_CASE_SOURCE
                    _phase = EtatAutomate.SELECTION_CASE_SOURCE;
                    //déselectionne carte
                    _carteI = -1;
                    //reset case
                    _caseI = 0;
                    _caseJ = 0;

                }
                else if (appuieSurGauche(current,last)){
                    //décrémente carteI
                    _carteI = (_carteI>0) ? _carteI-1 : _carteI;
                }
                else if (appuieSurDroite(current,last)){
                    //incrément carteI
                    _carteI = (_carteI<_joueurActuel.getNbCartesInMain()-1) ? _carteI+1 : _carteI;
                }
                else if (appuieSurValide(current,last) && peutSelectionnerCarte(_carteI)){
                    //passe phase à SELECTION_CASE_CARTE
                    _phase = EtatAutomate.SELECTION_CASE_CARTE;
                    //reset case
                    _caseI = 0;
                    _caseJ = 0;
                }
                else if ((appuieSurHaut(current,last) && _joueurActuel ==  _joueur1) || (appuieSurBas(current,last) && _joueurActuel ==  _joueur2)){
                    //passe phase à SELECTION_CASE_SOURCE
                    _phase = EtatAutomate.SELECTION_CASE_SOURCE;
                    //déselectionne carte
                    _carteI = -1;
                    //reset case
                    _caseI = 0;
                    _caseJ = 0;
                }
                break;
            case EtatAutomate.SELECTION_CASE_CARTE:
                if (appuieSurGauche(current,last) && (_joueurActuel == _joueur1 && _caseI < _plateau.getLongueur()/2 || _joueurActuel == _joueur2 && _caseI >= _plateau.getLongueur()/2 )){
                    //décrémente caseI
                    _caseI = (_caseI>0) ? _caseI-1 : _caseI;
                }
                else if (appuieSurDroite(current,last)){
                    //incrémente caseI
                    _caseI = (_caseI<_plateau.getLongueur()-1) ? _caseI+1 : _caseI;
                }
                else if (appuieSurHaut(current,last)){
                    //décrémente caseJ
                    _caseJ = (_caseJ>0) ? _caseJ-1 : _caseJ;
                }
                else if (appuieSurBas(current,last)){
                    //incrémente caseJ
                    _caseJ = (_caseJ<_plateau.getLargeur()-1) ? _caseJ+1 : _caseJ;
                }
                else if (appuieSurValide(current,last) && peutInvoquer(_carteI,_caseJ,_caseI))
                {
                    //passe phase à SELECTION_CARTE
                    _phase = EtatAutomate.SELECTION_CARTE;
                    //invoque la carte sur la case
                    _plateau.invoke(_joueurActuel.getCarteInMainAt(_carteI),_caseJ,_caseI, _joueurActuel);
                    //consomme le mana
                    _joueurActuel.reduireJauge(_joueurActuel.getCarteInMainAt(_carteI).getCout());
                    //retire la carte de la main et la remet dans le deck
                    _joueurActuel.putCarteInDeckFromMainAt(_carteI);
                    
                    //déselectionne la case
                    _caseI = -1;
                    _caseJ = -1;
                    //reset carte
                    _carteI = 0;
                }

                else if (appuieSurRetour(current,last))
                {
                    //passe phase à SELECTION_CARTE
                    _phase = EtatAutomate.SELECTION_CARTE;
                    //déselectionne la case
                    _caseI = -1;
                    _caseJ = -1;
                }
                break;
            case EtatAutomate.SELECTION_CASE_SOURCE:
                if (appuieSurGauche(current,last)){
                    //décrémente caseI
                    _caseI = (_caseI>0) ? _caseI-1 : _caseI;
                }
                else if (appuieSurDroite(current,last)){
                    //incrémente caseI
                    _caseI = (_caseI<_plateau.getLongueur()-1) ? _caseI+1 : _caseI;
                }
                else if (appuieSurHaut(current,last)){
                    //décrémente caseJ
                    _caseJ = (_caseJ>0) ? _caseJ-1 : _caseJ;
                }
                else if (appuieSurBas(current,last)){
                    //incrémente caseJ
                    _caseJ = (_caseJ<_plateau.getLargeur()-1) ? _caseJ+1 : _caseJ;
                }
                else if (appuieSurValide(current,last) && peutSelectionnerInvocation(_caseJ,_caseI))
                {
                    //pass phase à SELECTION_CASE_CIBLE
                    _phase = EtatAutomate.SELECTION_CASE_CIBLE;
                    //positionne lastCase à case
                    _lastCaseI = _caseI;
                    _lastCaseJ = _caseJ;
                }
                else if (appuieSurRetour(current,last))
                {
                    //pass phase à SELECTION_CARTE
                    _phase = EtatAutomate.SELECTION_CARTE;
                    //déselectionne case
                    _caseI = -1;
                    _caseJ = -1;
                    //reset carte
                    _carteI = 0;
                }
                break;
            case EtatAutomate.SELECTION_CASE_CIBLE:
                if (appuieSurGauche(current,last)){
                    //décrémente caseI
                    _caseI = (_caseI>0) ? _caseI-1 : _caseI;
                }
                else if (appuieSurDroite(current,last)){
                    //incrémente caseI
                    _caseI = (_caseI<_plateau.getLongueur()-1) ? _caseI+1 : _caseI;
                }
                else if (appuieSurHaut(current,last)){
                    //décrémente caseJ
                    _caseJ = (_caseJ>0) ? _caseJ-1 : _caseJ;
                }
                else if (appuieSurBas(current,last)){
                    //incrémente caseJ
                    _caseJ = (_caseJ<_plateau.getLargeur()-1) ? _caseJ+1 : _caseJ;
                }
                else if (appuieSurValide(current,last)&& peutAttaquerOuDeplacer(_lastCaseJ,_lastCaseI,_caseJ,_caseI))
                {
                    //passe phase SELECTION_CASE_SOURCE
                    _phase = EtatAutomate.SELECTION_CASE_SOURCE;
                    //attaque ou déplace l'invoc en prevCase vers case
                    AttaqueOuDeplace(_lastCaseJ, _lastCaseI,_caseJ,_caseI);
                    //déselectionne prevCase
                    _lastCaseI = -1;
                    _lastCaseJ = -1;
                }

                else if (appuieSurRetour(current,last))
                {
                    //passe phase SELECTION_CASE_SOURCE
                    _phase = EtatAutomate.SELECTION_CASE_SOURCE;
                    //déselectionne prevCase
                    _lastCaseI = -1;
                    _lastCaseJ = -1;
                }
                break;
        }
    }
    
    //---SOURIS---//
    //TO DO

    //-------------------------saveManager---------------------------//
    private void LoadGame(String FileName)
    {
        Carte tmpJ1C1 = new Carte(5, 3, 2, "LUI", "l'image en .png", TypeDeCarte.COMBATTANT, TypeRarete.COMMUNE, "le sprite en .png");
        Carte tmpJ1C2 = new Carte(5, 3, 2, "LUI", "l'image en .png", TypeDeCarte.COMBATTANT, TypeRarete.COMMUNE, "le sprite en .png");
        Carte tmpJ1C3 = new Carte(5, 3, 2, "LUI", "l'image en .png", TypeDeCarte.COMBATTANT, TypeRarete.COMMUNE, "le sprite en .png");
        Carte tmpJ1C4 = new Carte(5, 3, 2, "LUI", "l'image en .png", TypeDeCarte.COMBATTANT, TypeRarete.COMMUNE, "le sprite en .png");
        Carte tmpJ1C5 = new Carte(5, 3, 2, "LUI", "l'image en .png", TypeDeCarte.COMBATTANT, TypeRarete.COMMUNE, "le sprite en .png");
        Carte tmpJ1C6 = new Carte(5, 3, 2, "LUI", "l'image en .png", TypeDeCarte.COMBATTANT, TypeRarete.COMMUNE, "le sprite en .png");
        Carte tmpJ1C7 = new Carte(5, 3, 2, "LUI", "l'image en .png", TypeDeCarte.COMBATTANT, TypeRarete.COMMUNE, "le sprite en .png");
        Carte tmpJ1C8 = new Carte(5, 3, 2, "LUI", "l'image en .png", TypeDeCarte.COMBATTANT, TypeRarete.COMMUNE, "le sprite en .png");
        Carte tmpJ1C9 = new Carte(5, 3, 2, "LUI", "l'image en .png", TypeDeCarte.COMBATTANT, TypeRarete.COMMUNE, "le sprite en .png");
        Carte tmpJ1C10 = new Carte(5, 3, 2, "LUI", "l'image en .png", TypeDeCarte.COMBATTANT, TypeRarete.COMMUNE, "le sprite en .png");
        Carte tmpJ1C11 = new Carte(5, 3, 2, "LUI", "l'image en .png", TypeDeCarte.COMBATTANT, TypeRarete.COMMUNE, "le sprite en .png");
        Carte tmpJ1C12 = new Carte(5, 3, 2, "LUI", "l'image en .png", TypeDeCarte.COMBATTANT, TypeRarete.COMMUNE, "le sprite en .png");
        Carte tmpJ1C13 = new Carte(5, 3, 2, "LUI", "l'image en .png", TypeDeCarte.COMBATTANT, TypeRarete.COMMUNE, "le sprite en .png");
        Carte tmpJ1C14 = new Carte(5, 3, 2, "LUI", "l'image en .png", TypeDeCarte.COMBATTANT, TypeRarete.COMMUNE, "le sprite en .png");
        Carte tmpJ1C15 = new Carte(5, 3, 2, "LUI", "l'image en .png", TypeDeCarte.COMBATTANT, TypeRarete.COMMUNE, "le sprite en .png");
        _joueur1.addCarteInDeck(tmpJ1C1);
        _joueur1.addCarteInDeck(tmpJ1C2);
        _joueur1.addCarteInDeck(tmpJ1C3);
        _joueur1.addCarteInDeck(tmpJ1C4);
        _joueur1.addCarteInDeck(tmpJ1C5);
        _joueur1.addCarteInDeck(tmpJ1C6);
        _joueur1.addCarteInDeck(tmpJ1C7);
        _joueur1.addCarteInDeck(tmpJ1C8);
        _joueur1.addCarteInDeck(tmpJ1C9);
        _joueur1.addCarteInDeck(tmpJ1C10);
        _joueur1.addCarteInDeck(tmpJ1C11);
        _joueur1.addCarteInDeck(tmpJ1C12);
        _joueur1.addCarteInDeck(tmpJ1C13);
        _joueur1.addCarteInDeck(tmpJ1C14);
        _joueur1.addCarteInDeck(tmpJ1C15);
        Carte tmpJ2C1 = new Carte(5, 3, 2, "LUI", "l'image en .png", TypeDeCarte.COMBATTANT, TypeRarete.COMMUNE, "le sprite en .png");
        Carte tmpJ2C2 = new Carte(5, 3, 2, "LUI", "l'image en .png", TypeDeCarte.COMBATTANT, TypeRarete.COMMUNE, "le sprite en .png");
        Carte tmpJ2C3 = new Carte(5, 3, 2, "LUI", "l'image en .png", TypeDeCarte.COMBATTANT, TypeRarete.COMMUNE, "le sprite en .png");
        Carte tmpJ2C4 = new Carte(5, 3, 2, "LUI", "l'image en .png", TypeDeCarte.COMBATTANT, TypeRarete.COMMUNE, "le sprite en .png");
        Carte tmpJ2C5 = new Carte(5, 3, 2, "LUI", "l'image en .png", TypeDeCarte.COMBATTANT, TypeRarete.COMMUNE, "le sprite en .png");
        Carte tmpJ2C6 = new Carte(5, 3, 2, "LUI", "l'image en .png", TypeDeCarte.COMBATTANT, TypeRarete.COMMUNE, "le sprite en .png");
        Carte tmpJ2C7 = new Carte(5, 3, 2, "LUI", "l'image en .png", TypeDeCarte.COMBATTANT, TypeRarete.COMMUNE, "le sprite en .png");
        Carte tmpJ2C8 = new Carte(5, 3, 2, "LUI", "l'image en .png", TypeDeCarte.COMBATTANT, TypeRarete.COMMUNE, "le sprite en .png");
        Carte tmpJ2C9 = new Carte(5, 3, 2, "LUI", "l'image en .png", TypeDeCarte.COMBATTANT, TypeRarete.COMMUNE, "le sprite en .png");
        Carte tmpJ2C10 = new Carte(5, 3, 2, "LUI", "l'image en .png", TypeDeCarte.COMBATTANT, TypeRarete.COMMUNE, "le sprite en .png");
        Carte tmpJ2C11 = new Carte(5, 3, 2, "LUI", "l'image en .png", TypeDeCarte.COMBATTANT, TypeRarete.COMMUNE, "le sprite en .png");
        Carte tmpJ2C12 = new Carte(5, 3, 2, "LUI", "l'image en .png", TypeDeCarte.COMBATTANT, TypeRarete.COMMUNE, "le sprite en .png");
        Carte tmpJ2C13 = new Carte(5, 3, 2, "LUI", "l'image en .png", TypeDeCarte.COMBATTANT, TypeRarete.COMMUNE, "le sprite en .png");
        Carte tmpJ2C14 = new Carte(5, 3, 2, "LUI", "l'image en .png", TypeDeCarte.COMBATTANT, TypeRarete.COMMUNE, "le sprite en .png");
        Carte tmpJ2C15 = new Carte(5, 3, 2, "LUI", "l'image en .png", TypeDeCarte.COMBATTANT, TypeRarete.COMMUNE, "le sprite en .png");
        _joueur2.addCarteInDeck(tmpJ2C1);
        _joueur2.addCarteInDeck(tmpJ2C2);
        _joueur2.addCarteInDeck(tmpJ2C3);
        _joueur2.addCarteInDeck(tmpJ2C4);
        _joueur2.addCarteInDeck(tmpJ2C5);
        _joueur2.addCarteInDeck(tmpJ2C6);
        _joueur2.addCarteInDeck(tmpJ2C7);
        _joueur2.addCarteInDeck(tmpJ2C8);
        _joueur2.addCarteInDeck(tmpJ2C9);
        _joueur2.addCarteInDeck(tmpJ2C10);
        _joueur2.addCarteInDeck(tmpJ2C11);
        _joueur2.addCarteInDeck(tmpJ2C12);
        _joueur2.addCarteInDeck(tmpJ2C13);
        _joueur2.addCarteInDeck(tmpJ2C14);
        _joueur2.addCarteInDeck(tmpJ2C15);
        Carte tmpCristalJ1 = new Carte(20, 0, 0, "Cristal", "cristalCarte.png", TypeDeCarte.OBJET, TypeRarete.LEGENDAIRE, "cristal.png");
        Carte tmpCristalJ2 = new Carte(20, 0, 0, "Cristal", "cristalCarte.png", TypeDeCarte.OBJET, TypeRarete.LEGENDAIRE, "cristal.png");
        _plateau.invoke(tmpCristalJ1,_plateau.getLargeur()/2,1, _joueur1);
        _plateau.invoke(tmpCristalJ2,_plateau.getLargeur()/2,_plateau.getLongueur()-2, _joueur2);
        InitTurn();
    }

    //-------------------------testManager---------------------------//
    //pour l'ergonomie, pour éviter qu'on puisse séléctionner une carte non jouable
    private bool peutSelectionnerCarte(int i)
    {
        Carte carte = _joueurActuel.getCarteInMainAt(i);
        return _joueurActuel.getJauge() >= carte.getCout();
    }
    //pour l'ergonomie, pour éviter qu'on puisse séléctionner une invocation non jouable
    private bool peutSelectionnerInvocation(int lig, int col)
    {
        Invocation? source = _plateau.getEntityAt(lig, col);
        return source != null && source.getInvocateur() == _joueurActuel && (source.getPeutAttaquer() || source.getPeutBouger());
    }
    private bool peutInvoquer(int i, int lig, int col)
    {
        Carte carte = _joueurActuel.getCarteInMainAt(i);
        return _plateau.isEmpty(lig,col) && _joueurActuel.getJauge() >= carte.getCout();
    }
    private bool peutAttaquer(int lig1, int col1, int lig2, int col2)
    {
        Invocation? source = _plateau.getEntityAt(lig1, col1);
        Invocation? cible = _plateau.getEntityAt(lig2, col2);
        int distance = Math.Abs(lig2-lig1)+Math.Abs(col2-col1);
        int MAXDISTANCE = 2; // pourra être remplacé par la range de l'invoc si on rajoute ça plus tard
        return distance < MAXDISTANCE &&  source != null && cible != null && source.getInvocateur()==_joueurActuel && cible.getInvocateur() != _joueurActuel && source.getPeutAttaquer();
    }
    private bool peutDeplacer(int lig1, int col1, int lig2, int col2)
    {
        Invocation? source = _plateau.getEntityAt(lig1, col1);
        int distance = Math.Abs(lig2-lig1)+Math.Abs(col2-col1);
        int MAXDISTANCE = 3; // pourra être remplacé par la range de l'invoc si on rajoute ça plus tard
        return distance < MAXDISTANCE && source != null && _plateau.isEmpty(lig2,col2) && source.getInvocateur()==_joueurActuel && source.getPeutBouger();
    }
    private bool peutAttaquerOuDeplacer(int lig1, int col1, int lig2, int col2)
    {
        return peutAttaquer(lig1, col1, lig2, col2) ||  peutDeplacer(lig1, col1, lig2, col2);
    }
    private void AttaqueOuDeplace(int lig1, int col1, int lig2, int col2)
    {
        if (peutDeplacer(lig1, col1, lig2, col2))
        {
            _plateau.move(lig1, col1, lig2, col2);
        }
        else
        {
            _plateau.attack(lig1, col1, lig2, col2);
        }
    }
    
    /* MONOGAME FUNCTIONS
     * protected override void Initialize();
     * protected override void LoadContent();
     * protected override void Update(GameTime gameTime);
     * protected override void Draw(GameTime gameTime);
     */
}