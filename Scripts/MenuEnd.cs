using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CatRoyale.Scripts;

public class MenuEnd
{
    private GraphicsDeviceManager _graphics;
    private Texture2D _background, boutonjouer, boutonjouer2, boutonquitter2, boutonquitter, cadrestat;
    private SpriteBatch _spriteBatch;
    private SpriteFont font;
    private const int boutonLargeur = 260;
    private const int boutonHauteur = 106;
    private const int x = ((1024  - boutonLargeur) / 2)+180;
    private const int y = ((640 - boutonHauteur) / 2)+150;
    private const int ecart = 5;
    private Rectangle cadrestatRect = new Rectangle(300,  180, 420, 320);
    private Rectangle boutonStart = new Rectangle(x-boutonLargeur,  y+boutonHauteur+ecart, boutonLargeur, boutonHauteur);
    private Rectangle boutonQuitter = new Rectangle(x, y+boutonHauteur+ecart, 160, boutonHauteur);
    
    private MouseState EtatActuelSouris;
    private MouseState EtatPrecSouris;
    
    private void ClicBouton()
    {
        bool ClicGauche = EtatActuelSouris.LeftButton == ButtonState.Pressed &&
                          EtatPrecSouris.LeftButton == ButtonState.Released;
        int mx = EtatActuelSouris.X;
        int my = EtatActuelSouris.Y;

        if (ClicGauche)
        {
            if (boutonStart.Contains(mx, my))
            {
                    string pseudoJ1 = CatRoyal.jeuChat.Joueur1.Pseudo;
                    string pseudoJ2 = CatRoyal.jeuChat.Joueur2.Pseudo;
                    int winStreakJ1 = CatRoyal.jeuChat.Joueur1.WinStreak;
                    int winStreakJ2 = CatRoyal.jeuChat.Joueur2.WinStreak;
                    CatRoyal.LoadGame(CatRoyal.defaultSaveFileName);
                    CatRoyal.jeuChat.Joueur1.Pseudo = pseudoJ1;
                    CatRoyal.jeuChat.Joueur2.Pseudo = pseudoJ2;
                    CatRoyal.jeuChat.Joueur1.WinStreak = winStreakJ1;
                    CatRoyal.jeuChat.Joueur2.WinStreak = winStreakJ2;
                    CatRoyal.jeuChat.InitTurn();
                    CatRoyal.SetMenu(EtatMenu.INGAME);
            }
            if (boutonQuitter.Contains(mx, my))
            {
                    CatRoyal.SetMenu(EtatMenu.MENUMAIN);
            }
        }
    }

    public void LoadContent(ContentManager content)
    {
        _background = content.Load<Texture2D>("textures/map/endmenu");
        boutonquitter= content.Load<Texture2D>("textures/map/boutonquitter");
        boutonquitter2= content.Load<Texture2D>("textures/map/boutonquitter2");
        boutonjouer = content.Load<Texture2D>("textures/map/boutonjouer");
        boutonjouer2 = content.Load<Texture2D>("textures/map/boutonjouer2");
        font = content.Load<SpriteFont>("font");
        cadrestat= content.Load<Texture2D>("textures/map/cadrestat");
    }

    public void Update()
    {
        EtatActuelSouris = Mouse.GetState();
        ClicBouton();
        EtatPrecSouris = EtatActuelSouris;
    }

    public void Draw(GraphicsDevice graphics, SpriteBatch spriteBatch, Joueur joueur)
    {
        Rectangle destbackground = new Rectangle(0, 0, graphics.Viewport.Width , graphics.Viewport.Height);
        spriteBatch.Draw(_background, destbackground, Color.White);
        
        int mx = EtatActuelSouris.X;
        int my = EtatActuelSouris.Y;
        
        spriteBatch.Draw(cadrestat, cadrestatRect, Color.White );
        spriteBatch.Draw(boutonStart.Contains(mx, my) ? boutonjouer2:boutonjouer, boutonStart, Color.White );
        spriteBatch.Draw(boutonQuitter.Contains(mx, my) ?boutonquitter2:boutonquitter, boutonQuitter, Color.White);
        
        spriteBatch.DrawString(font,  "Gagnant(e) : "+ joueur.Pseudo , new Vector2(cadrestatRect.X+120, cadrestatRect.Y+25), Color.White);
        spriteBatch.DrawString(font,  "Nombre de partie gagne: "+ joueur.WinStreak , new Vector2(370, 300), Color.Black);
    }
}