using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using TestProjet.Scripts;

namespace TestProjet;
public enum EtatMenu { MENUMAIN, INGAME, ENDGAME }
public class CatRoyal : Game
{
    
    
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _background, _case;
    private KeyboardState _previousKeyboardState;
    private String _carteBaseImage;
    private Texture2D jauge;

    private static EtatMenu _menu= EtatMenu.MENUMAIN;
    
    public static Jeu jeuChat;
    private MainMenu menuMain;
    private InGame menuInGame;
    private MenuEnd menuEnd;
    
    private SpriteFont _font;
    private MouseState _previousMouseState;
    
    private static int taillecase = 55;

    private static int manaY = 130;  
    private static Boolean quitter = false;
    
    public static string savePath = "../../../data/xml/";
    public static string defaultSaveFileName = "defaultSave.xml";
    public static string autoSaveFileName = "autoSave.xml";

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
        
    }
    
    
    protected override void Initialize()
    {
        menuMain = new MainMenu();
        menuInGame = new InGame();
        menuEnd= new MenuEnd();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        menuMain.LoadContent(Content);
        menuInGame.LoadContent(Content, GraphicsDevice);
        menuEnd.LoadContent(Content);
  
    }

    protected override void Update(GameTime gameTime)
    {
        if (quitter)
        {
            Exit();
        }
        switch (_menu)
        {
            case EtatMenu.MENUMAIN:
                menuMain.Update(gameTime);
                break;
            case EtatMenu.INGAME:
                menuInGame.Update(gameTime, _graphics);
                break;
            case EtatMenu.ENDGAME:
                menuEnd.Update(gameTime);
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
            case EtatMenu.ENDGAME:
                Joueur joueur = menuInGame.joueurWin;
                String texte = jeuChat.PseudoJoueurActuel;
                menuEnd.Draw(gameTime, GraphicsDevice, _spriteBatch, texte);
                break;
                
        }
        _spriteBatch.End(); 
        base.Draw(gameTime);
    }

    public static void setMenu(EtatMenu menu)
    {
        _menu = menu;
    }

    public static void Quitter()
    {
        quitter = true;
    }
    
    
    
    
    
    public static void SaveGame(String FileName)
    {
        XMLManager<Jeu> manager = new XMLManager<Jeu>();
        manager.Save(savePath+FileName, jeuChat);
    }
    
    public static void LoadGame(String FileName)
    {
        //on charge la partie
        XMLManager<Jeu> manager = new XMLManager<Jeu>();
        jeuChat = (Jeu)manager.Load(savePath+FileName);
        jeuChat.Plateau.InitAfterLoad();
    }
    
}