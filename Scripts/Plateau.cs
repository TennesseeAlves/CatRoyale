using System;

namespace TestProjet.Scripts;

public class Plateau
{
    private Invocation?[,] _map;

    public Plateau(int longueur, int largeur)
    {
        _map = new Invocation?[longueur, largeur];
    }

    public int getLongueur()
    {
        return _map.GetLength(0);
    }

    public int getLargeur()
    {
        return _map.GetLength(1);
    }

    public Invocation? getEntityAt(int ligne, int colonne)
    {
        if (ligne >= _map.GetLength(0) || ligne < 0 || colonne >= _map.GetLength(1) || colonne < 0)
        {
            throw new IndexOutOfRangeException();
        }
        return _map[ligne, colonne];
    }

    public void deleteAt(int ligne, int colonne)
    {
        if (ligne >= _map.GetLength(0) || ligne < 0 || colonne >= _map.GetLength(1) || colonne < 0)
        {
            throw new IndexOutOfRangeException();
        }
        _map[ligne, colonne] = null;
    }

    public void invoke(Carte carte, int ligne, int colonne);
    public void move(int ligneDepart, int colonneDepart, int ligneArrive, int colonneArrive);
    public void attack(int ligneDepart, int colonneDepart, int ligneArrive, int colonneArrive);
    
}