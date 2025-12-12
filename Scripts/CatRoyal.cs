using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TestProjet.Scripts;

public enum EtatMenu { MENUMAIN, INGAME, ENDGAME }
public class CatRoyal : Game
{
    
    
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private static EtatMenu _menu= EtatMenu.MENUMAIN;
    
    public static Jeu jeuChat;
    private MainMenu menuMain;
    private InGame menuInGame;
    private MenuEnd menuEnd;
    
    private static bool quitter = false;
    
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
                menuMain.Update();
                break;
            case EtatMenu.INGAME:
                menuInGame.Update();
                break;
            case EtatMenu.ENDGAME:
                menuEnd.Update();
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
                menuMain.Draw(GraphicsDevice, _spriteBatch);
                break;
            case EtatMenu.INGAME:
                menuInGame.Draw(GraphicsDevice, Content,_spriteBatch);
                break;
            case EtatMenu.ENDGAME:
               
                Joueur joueur = jeuChat.JoueurActuel;
                menuEnd.Draw(GraphicsDevice, _spriteBatch, joueur);
                break;
                
        }
        _spriteBatch.End(); 
        base.Draw(gameTime);
    }

    public static void SetMenu(EtatMenu menu)
    {
        _menu = menu;
    }

    public static void Quitter()
    {
        quitter = true;
    }
    
    
    
    
    
    public static void SaveGame(string FileName)
    {
        XMLManager<Jeu> manager = new XMLManager<Jeu>();
        manager.Save(savePath+FileName, jeuChat);
    }
    
    public static void LoadGame(string FileName)
    {
        //on charge la partie
        XMLManager<Jeu> manager = new XMLManager<Jeu>();
        jeuChat = (Jeu)manager.Load(savePath+FileName);
        jeuChat.Plateau.InitAfterLoad();
    }
    
}