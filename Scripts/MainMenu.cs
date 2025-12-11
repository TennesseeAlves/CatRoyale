
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TestProjet.Scripts;


namespace TestProjet;

public class MainMenu
{
    private GraphicsDeviceManager _graphics;
    private Texture2D _background, boutonjouer, boutonjouer2, 
        boutoncharger,boutoncharger2,boutonquitter2,boutonquitter;
    private SpriteBatch _spriteBatch;

    private static int boutonLargeur = 390;
    private static int boutonHauteur = 160;
    private static int x = (1024  - boutonLargeur) / 2;
    private static int y = (640 - boutonHauteur) / 2;
    private static int ecart = 4;
    private static int offsety = 55;
    Rectangle boutonStart = new Rectangle(x, y+offsety-(boutonHauteur+ecart), boutonLargeur, boutonHauteur);
    Rectangle boutonCharger = new Rectangle(x, y+offsety, boutonLargeur, boutonHauteur);
    Rectangle boutonQuitter = new Rectangle(x+115, y+boutonHauteur+ecart+offsety, 160, boutonHauteur);
    
    
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
        
        if (boutonCharger.Contains(mx, my))
        {
           
            select = 1;
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
                    Console.WriteLine("START");
                    CatRoyal.LoadGame(CatRoyal.defaultSaveFileName);
                    CatRoyal.jeuChat.InitTurn();
                    CatRoyal.setMenu(EtatMenu.INGAME);
                    
                    break;
                case 1:
                    Console.WriteLine("CHARGER");
                    CatRoyal.LoadGame(CatRoyal.autoSaveFileName);
                    CatRoyal.setMenu(EtatMenu.INGAME);
                    break;
                case 2:
                    Console.WriteLine("Quitter");
                    CatRoyal.Quitter();
                    break;
            }
        }

    }

    public void LoadContent(ContentManager content)
    {
        _background = content.Load<Texture2D>("textures/map/mainmenu");
        boutonjouer = content.Load<Texture2D>("textures/map/boutonjouer");
        boutonjouer2 = content.Load<Texture2D>("textures/map/boutonjouer2");
        boutoncharger= content.Load<Texture2D>("textures/map/boutoncharger");
        boutoncharger2= content.Load<Texture2D>("textures/map/boutoncharger2");
        boutonquitter= content.Load<Texture2D>("textures/map/boutonquitter");
        boutonquitter2= content.Load<Texture2D>("textures/map/boutonquitter2");
    }

    public void Update(GameTime gameTime)
    {
        EtatActuelSouris = Mouse.GetState();
        ClicBouton();
        EtatPrecSouris = EtatActuelSouris;

    }

    public void Draw(GameTime gameTime,GraphicsDevice graphics, SpriteBatch spriteBatch)
    {
        
       
        Rectangle destbackground = new Rectangle(0, 0, graphics.Viewport.Width , graphics.Viewport.Height);
        spriteBatch.Draw(_background, destbackground, Color.White);
        

        spriteBatch.Draw(select == 0 ? boutonjouer2:boutonjouer, boutonStart, Color.White );
        spriteBatch.Draw(select == 1 ? boutoncharger2:boutoncharger, boutonCharger, Color.White);
        spriteBatch.Draw(select == 2 ?boutonquitter2:boutonquitter, boutonQuitter, Color.White);
        
    }
    
}