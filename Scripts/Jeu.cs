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
 *
 * Problèmes restants:
 *                      -les invoc à  HP ne meurt pas
 *                      -les invoc ne passent jamais à peutBouger et peutAttacker
 *                      -aucun moyen de savoir/gérer à qui c'est le tour pour l'instant
 *                      -pas fait ni l'IHM ni la partie sauvegarde pour l'instant (ni le XML mais bon)
 *                      -donc voir les instructions de : -début de partie(charger carte et invoc, placer les cristaux, remplir les jauges, piocher les cartes pour tout le monde, donner la main J1)
 *                                                       -début de tour(remplir jauge, piocher cartes, actualiser les peutBouger et peut Attaquer(mais pas du cristal))
 *                                                       -verification pendant le tour(mana suffisant pour carte, case contenant une invocation ou non, invocation appartenant au bon joueur, invocation peutJouer ou peutBouger, partie fini/gagné)
 *                                                       -fin de tour(passer la main à l'autre joueur)
 *                                                       -fin de partie(sauvegarder?)
 */


public class Jeu
{
    private Plateau _plateau;
    private Joueur _joueur1;
    private Joueur _joueur2;
    private Boolean tour;
    private Invocation TourJ1;
    private Invocation TourJ2;
    public Jeu(int longueur, int largeur, string nom1, string nom2)
    {
        _plateau = new Plateau(longueur, largeur);
        _joueur1 = new Joueur(nom1, 0);
        _joueur2 = new Joueur(nom2, 0);
        //true joueur1 false joueur2
        tour = true;

        InitTours();
    }


    public void InitTours()
    {
        TourJ1= new Invocation(1000, 0, "textures/mobs/TourJ1");
        TourJ2= new Invocation(1000, 0, "textures/mobs/TourJ2");
        TourJ1.setInvocateur(_joueur1);
        TourJ2.setInvocateur(_joueur2);
        TourJ1.setPeutAttaquer(false);
        TourJ2.setPeutAttaquer(false);
        
        TourJ1.setPeutBouger(false);
        TourJ2.setPeutBouger(false);
        
        _plateau.setEntityAt(TourJ1, getLongueur()/2, getLargeur()-1);
        _plateau.setEntityAt(TourJ2, (getLongueur()/2)-1, 0);
    
        _joueur1.setJauge(15);
        _joueur2.setJauge(15);
    }
    public int getLongueur()
    {
        return _plateau.getLongueur();
    }
    
    public int getLargeur()
    {
        return _plateau.getLargeur();
    }

    public Invocation getEntityAt(int ligne, int colonne)
    {
        return _plateau.getEntityAt(ligne, colonne);
    }

    public bool isEmpty(int ligne, int colonne)
    {
        return _plateau.isEmpty(ligne, colonne);
    }
    
    public Boolean getTour()
    {
        return tour;
    }
    
    public Joueur getTourJoueur()
    {
        if (tour)
        {
            return _joueur1;
        }
        return _joueur2;
    }
    
    public Boolean nextTour()
    {
        
        this.tour = !this.tour;
        return tour;
    }
    public Joueur GetJoueur1()
    {
        return _joueur1;
    }
    
    public Joueur GetJoueur2()
    {
        return _joueur2;
    }

    public bool move(Joueur joueur, int ligneDepart, int colonneDepart, int ligneArrive, int colonneArrive)
    {
        Invocation invocation = _plateau.getEntityAt(ligneDepart, colonneDepart);
    
        if (invocation.getInvocateur() == joueur && invocation.getPeutBouger() && _plateau.isEmpty(ligneArrive, colonneArrive))
        {
            _plateau.move(ligneDepart, colonneDepart, ligneArrive, colonneArrive);
            return true;
        }
        else
        {
            if (!attack(joueur, ligneDepart, colonneDepart, ligneArrive, colonneArrive))
            {
                Console.WriteLine("pas au joueur/ne peut pas bouger/la case darrive est occupe");
            }
            else
            {
                return true;
            }
          
        }
        return false;
    }

    public Plateau getPlateau()
    {
        return _plateau;
    }
    public bool invoke(Joueur joueur,Carte carte, int ligne, int colonne)
    {
        Invocation invocation = _plateau.getEntityAt(ligne, colonne);
        Console.Write("Oui");
        if (carte.getType() == TypeDeCarte.SORT)
        {
            if (!(_plateau.isEmpty(ligne, colonne)) && invocation.getInvocateur() == joueur)
            {
                Console.Write("Oui");
                invocation.takeVie(carte.getVie());
                return true;
            }
        } else if (_plateau.isEmpty(ligne, colonne) && joueur.getJauge()>= carte.getCout())
        {
            _plateau.invoke(joueur, carte, ligne, colonne);
            joueur.reduireJauge(carte.getCout());
            return true;
        }
        else
        {
            Console.WriteLine("mana faible");
            Console.WriteLine(joueur.getJauge());
            Console.WriteLine(carte.getCout());
        }
        
        return false;
    }

    public bool attack(Joueur joueur, int ligneDepart, int colonneDepart, int ligneArrive, int colonneArrive)
    {
        Invocation invocation = _plateau.getEntityAt(ligneDepart, colonneDepart);
        Invocation invocationVictim = _plateau.getEntityAt(ligneArrive, colonneArrive);
        if (invocation.getInvocateur() == joueur && invocation.getPeutBouger() &&
            !(_plateau.isEmpty(ligneArrive, colonneArrive))&&invocationVictim.getInvocateur()!=joueur && invocation.getPeutAttaquer())
        {
            _plateau.attack(ligneDepart, colonneDepart, ligneArrive, colonneArrive);

            if (invocationVictim.getVie() == 0)
            {
                _plateau.deleteAt(ligneArrive, colonneArrive); 
            }
   
            return true;
        }
        Console.WriteLine("n'appartient pas au joueur/ ne peut pas bouger/la case darrive est vide/la victime est le joueur");
        return false;
    }

    private void transition(int etatActuel, KeyboardState etatActuelClavier)
    {
        
    }
    


    
    
    
    /*
     * protected override void Initialize();
     * protected override void LoadContent();
     * protected override void Update(GameTime gameTime);
     * protected override void Draw(GameTime gameTime);
     */
}