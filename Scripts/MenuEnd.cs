
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TestProjet.Scripts;


namespace TestProjet;

public class MenuEnd
{
    private GraphicsDeviceManager _graphics;
    private Texture2D _background, boutonjouer, boutonjouer2, 
        boutoncharger,boutoncharger2,boutonquitter2,boutonquitter, cadrestat;
    private SpriteBatch _spriteBatch;
    private SpriteFont _font;
    private static int boutonLargeur = 260;
    private static int boutonHauteur = 106;
    private static int x = ((1024  - boutonLargeur) / 2)+180;
    private static int y = ((640 - boutonHauteur) / 2)+150;
    private static int ecart = 5;
    Rectangle cadrestatRect = new Rectangle(300,  180, 420, 320);
    Rectangle boutonStart = new Rectangle(x-boutonLargeur,  y+boutonHauteur+ecart, boutonLargeur, boutonHauteur);
    Rectangle boutonQuitter = new Rectangle(x, y+boutonHauteur+ecart, 160, boutonHauteur);
    
    
    static MouseState EtatActuelSouris;
    static MouseState EtatPrecSouris;

    private int select;
    public void ClicBouton()
    {
        bool ClicGauche = EtatActuelSouris.LeftButton == ButtonState.Pressed &&
                          EtatPrecSouris.LeftButton == ButtonState.Released;
        //Console.WriteLine(EtatActuelSouris);
        int mx = EtatActuelSouris.X;
        int my = EtatActuelSouris.Y;
        select = -1;
        if (boutonStart.Contains(mx, my))
        {
            select = 0;
        }
        
        
        if (boutonQuitter.Contains(mx, my))
        {
            select = 2;
        }

        if (ClicGauche)
        {
            switch (select)
            {
                case 0:
                    Console.WriteLine("RESTART");
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
                    CatRoyal.setMenu(EtatMenu.INGAME);
                    break;
          
                case 2:
                    Console.WriteLine("Quitter");
                    CatRoyal.setMenu(EtatMenu.MENUMAIN);
                    break;
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
        boutoncharger= content.Load<Texture2D>("textures/map/boutoncharger");
        boutoncharger2= content.Load<Texture2D>("textures/map/boutoncharger2");
        _font = content.Load<SpriteFont>("font");
        cadrestat= content.Load<Texture2D>("textures/map/cadrestat");
    }

    public void Update(GameTime gameTime)
    {
        EtatActuelSouris = Mouse.GetState();
        ClicBouton();
        EtatPrecSouris = EtatActuelSouris;

    }

    public void Draw(GameTime gameTime,GraphicsDevice graphics, SpriteBatch spriteBatch, Joueur joueur)
    {
        
       
        Rectangle destbackground = new Rectangle(0, 0, graphics.Viewport.Width , graphics.Viewport.Height);
        
        spriteBatch.Draw(_background, destbackground, Color.White);
        
       
        
        spriteBatch.Draw(cadrestat, cadrestatRect, Color.White );
        spriteBatch.Draw(select == 0 ? boutonjouer2:boutonjouer, boutonStart, Color.White );
        spriteBatch.Draw(select == 2 ?boutonquitter2:boutonquitter, boutonQuitter, Color.White);
        
        spriteBatch.DrawString(_font,  "Gagnant(e) : "+ joueur.Pseudo , new Vector2(cadrestatRect.X+120, cadrestatRect.Y+25), Color.White);
        spriteBatch.DrawString(_font,  "Nombre de partie gagne: "+ joueur.WinStreak , new Vector2(370, 300), Color.Black);
        
    }
}