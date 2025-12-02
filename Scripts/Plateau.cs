using System;

namespace TestProjet.Scripts;

public class Plateau
{
    private Invocation _TowerJ1;
    private Invocation _TowerJ2;
    private Invocation?[,] _map;

    public Plateau(int longueur, int largeur)
    {
        _map = new Invocation?[largeur, longueur];
    }

    public int getLongueur()
    {
        return _map.GetLength(1);
    }

    public int getLargeur()
    {
        return _map.GetLength(0);
    }

    public Invocation getTowerJ1()
    {
        return _TowerJ1;
    }

    public void setTowerJ1(Invocation TowerJ1)
    {
        _TowerJ1 = TowerJ1;
    }

    public Invocation getTowerJ2()
    {
        return _TowerJ2;
    }

    public void setTowerJ2(Invocation TowerJ2)
    {
        _TowerJ2 = TowerJ2;
    }

    public void setEntityAt(Invocation invoc, int ligne, int colonne)
    {
        _map[ligne, colonne] = invoc;
    }
    public Invocation? getEntityAt(int ligne, int colonne)
    {
        if (ligne >= _map.GetLength(0) || ligne < 0 || colonne >= _map.GetLength(1) || colonne < 0)
        {
            throw new IndexOutOfRangeException();
        }
        return _map[ligne, colonne];
    }

    public bool isEmpty(int ligne, int colonne)
    {
        if (ligne >= _map.GetLength(0) || ligne < 0 || colonne >= _map.GetLength(1) || colonne < 0)
        {
            throw new IndexOutOfRangeException();
        }
        return (_map[ligne, colonne] == null);
    }

    public void deleteAt(int ligne, int colonne)
    {
        if (ligne >= _map.GetLength(0) || ligne < 0 || colonne >= _map.GetLength(1) || colonne < 0)
        {
            throw new IndexOutOfRangeException();
        }
        _map[ligne, colonne] = null;
    }

    public void invoke(Joueur joueur, Carte carte, int ligne, int colonne)
    {   
        
        _map[ligne, colonne] = carte.generateInvocation();
        _map[ligne, colonne].setInvocateur(joueur);
    }

    public void move(int ligneDepart, int colonneDepart, int ligneArrive, int colonneArrive)
    {
            _map[ligneArrive, colonneArrive] = _map[ligneDepart, colonneDepart];
            _map[ligneDepart, colonneDepart] = null;
            _map[ligneArrive, colonneArrive].setPeutBouger(false);
    }

    public void attack(int ligneDepart, int colonneDepart, int ligneArrive, int colonneArrive)
    {
        //effectue l'attaque et vérifie s'il est mort
        if (_map[ligneArrive, colonneArrive].takeDamage(_map[ligneDepart, colonneDepart].getDegat()))
        {
            _map[ligneArrive, colonneArrive] = null;
        }
        _map[ligneDepart,colonneDepart].setPeutAttaquer(false);
    }

    public bool victory(Joueur joueur)
    {
        return _TowerJ1.getInvocateur() == joueur && _TowerJ2.getVie() == 0 || _TowerJ2.getInvocateur() == joueur && _TowerJ1.getVie() == 0;
    }

    public bool isTower(Invocation invoc)
    {
        return invoc == _TowerJ1 || invoc == _TowerJ2;
    }
}