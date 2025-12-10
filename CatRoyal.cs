using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using TestProjet.Scripts;

namespace TestProjet;
public enum EtatMenu { MENUMAIN,INGAME, ENDGAME }
public class CatRoyal : Game
{
    
        
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _background, _case;
    private KeyboardState _previousKeyboardState;
    private String _carteBaseImage;
    private Texture2D jauge;

    private static EtatMenu _menu= EtatMenu.MENUMAIN;
    private Jeu jeuChat;
    
    private MainMenu menuMain;
    private InGame menuInGame;
    
    private SpriteFont _font;
    private MouseState _previousMouseState;
    
    private static int taillecase = 55;

    private static int manaY = 130;  

    public CatRoyal()
    {
    
         //creation de l'objet pour gerer la fenetre
        _graphics = new GraphicsDeviceManager(this);
        
        //Changement des dimensions + nom de la fenetre 
        _graphics.PreferredBackBufferWidth = 1024;
        _graphics.PreferredBackBufferHeight = 640;
        _graphics.ApplyChanges();
        Window.Title = "Cat Royale";
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        
        //jeuChat = new Jeu(10, 5, "Alice", "Bob");
    }
    
    
    protected override void Initialize()
    {
        menuMain = new MainMenu();
        menuInGame = new InGame();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        menuMain.LoadContent(Content);
        menuInGame.LoadContent(Content, GraphicsDevice);
  
    }

    protected override void Update(GameTime gameTime)
    {
        switch (_menu)
        {
            case EtatMenu.MENUMAIN:
                menuMain.Update(gameTime);
                break;
            case EtatMenu.INGAME:
                menuInGame.Update(gameTime, _graphics);
                break;
                
        }
        
        base.Update(gameTime);
        
    }

    protected override void Draw(GameTime gameTime)
    {
        _spriteBatch.Begin();
        switch (_menu)
        {
            case EtatMenu.MENUMAIN:
                menuMain.Draw(gameTime, GraphicsDevice, _spriteBatch);
                break;
            case EtatMenu.INGAME:
                menuInGame.Draw(gameTime,  GraphicsDevice, Content,_spriteBatch);
                break;
                
        }
        _spriteBatch.End(); 
        base.Draw(gameTime);
    }

    public static void setMenu(EtatMenu menu)
    {
        _menu = menu;
    }

    public void Quitter()
    {
        //Instance.Exit();
    }
}