using System;

namespace TestProjet.Scripts;

public class Plateau
{
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

    public void invoke(Carte carte, int ligne, int colonne, Joueur invocateur)
    {
        _map[ligne, colonne] = carte.getInvocation();
        _map[ligne, colonne].setVie(carte.getVie());
        _map[ligne, colonne].setInvocateur(invocateur);
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
        return false;
    }
}