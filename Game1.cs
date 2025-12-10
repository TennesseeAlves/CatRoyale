using System;
using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using TestProjet.Scripts;

namespace TestProjet;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _background, _case;
    private KeyboardState _previousKeyboardState;
    private String _carteBaseImage;
    private Texture2D jauge;

    
    private Jeu jeuChat;
    
    private SpriteFont _font;
    private MouseState _previousMouseState;
    
    private static int taillecase = 55;

    private static int manaY = 130;  

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        
        _graphics.PreferredBackBufferWidth = 1024;
        _graphics.PreferredBackBufferHeight = 640;
        _graphics.ApplyChanges();
        Window.Title = "Cat Royale";
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        //jeuChat = new Jeu(10, 5, "Alice", "Bob");
        jeuChat= new Jeu("autosave.xml");
    }
    
    public void DrawCarte(Rectangle dest, Color tint, String image, int degrees, SpriteEffects spriteEffect)
    {
        
        Texture2D texture = Content.Load<Texture2D>(image);
        _spriteBatch.Draw(
            texture,
            dest,
            null,
            tint,
            MathHelper.ToRadians(degrees),
            Vector2.Zero,
            spriteEffect,
            0f
        );
    }
    
    private void DrawAllCarte(
        Joueur joueur,
        int x,          // point de départ en X
        int y,           // Y normal des cartes
        int espaceCarte,     // écart entre les cartes
        int selectionY,       // Y quand la carte est "levée"
        int direction        // 1 = vers la droite, -1 = vers la gauche
    )
    {
        for (int cartenum = 0; cartenum < joueur.getNbCartesInMain(); cartenum++)
        {
            int cardX = x + direction * cartenum * espaceCarte;

            Rectangle dest = new Rectangle(cardX, y, 70, 100);

            if (joueur == jeuChat.JoueurActuel)
            {
                Carte carteAt = joueur.getCarteInMainAt(cartenum,jeuChat.CartesExistantes);
                string image = carteAt.Image;

                if (jeuChat.CarteI == cartenum)
                {
                    dest = new Rectangle(cardX, selectionY, 70, 100);
                    _spriteBatch.DrawString(_font, carteAt.ToString(), new Vector2(20, 20), Color.Black);
                }

                SpriteEffects spriteEffects = (joueur == jeuChat.Joueur2)?SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically:SpriteEffects.None;
                DrawCarte(dest, Color.White, image, 0,spriteEffects);
            }
            else
            {
                DrawCarte(dest, Color.White, _carteBaseImage, 0, SpriteEffects.None);
            }

            
        }
    }
    
    public void DrawVie(Invocation invocation, int x, int y)
    {
        int vie = invocation.Vie;
        int maxVie = invocation.MaxVie;

        int largeur= 30;
        int hauteur= 5;
        
        Rectangle backRect = new Rectangle(x, y-5,largeur,hauteur);
        
        Color c1, c2;
        if (invocation.PseudoInvocateur==jeuChat.Joueur1.Pseudo)
        {
            c1 = Color.DarkBlue;
            c2 = Color.CornflowerBlue;
        }
        else
        {
            c1 = Color.Red;
            c2 = Color.LightSalmon;
        }
        _spriteBatch.DrawString(_font, invocation.Vie+"/"+invocation.MaxVie, new Vector2(x-25, y-35), Color.Black);
        _spriteBatch.Draw(jauge, backRect, c1);
        
        int manaH = (int)(largeur * ((float)vie/maxVie));
        Rectangle manaRect = new Rectangle(x, y-5, manaH, hauteur);
        _spriteBatch.Draw(jauge, manaRect, c2);
    }
    
    public void DrawMana(Joueur joueur, int x, int y)
    {
        int mana = joueur.Jauge;
        int maxMana = Joueur.MAXJAUGE;

        int largeur= 200;
        int hauteur= 15;
        
        Rectangle backRect = new Rectangle(x+100, y,largeur,hauteur);
        
        Color c1, c2;
        if (joueur==jeuChat.Joueur1)
        {
            c1 = Color.DarkBlue;
            c2 = Color.CornflowerBlue;
        }
        else
        {
            c1 = Color.Red;
            c2 = Color.LightSalmon;
        }
        _spriteBatch.Draw(jauge, backRect, c1);
        
        int manaH = (int)(largeur * ((float)mana/maxMana));
  
        Rectangle manaRect = new Rectangle(x+100, y, manaH, hauteur);
        _spriteBatch.Draw(jauge, manaRect, c2);
    }
    
    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _background = Content.Load<Texture2D>("textures/map/background");
        _case = Content.Load<Texture2D>("textures/map/case");
        _carteBaseImage = "textures/cards/carte_base";
        _font = Content.Load<SpriteFont>("font");

        jauge = new Texture2D(GraphicsDevice, 1, 1);
        jauge.SetData(new[]
        {
            Color.White
        });
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState keyboardState = Keyboard.GetState();
        if (keyboardState.IsKeyDown(Keys.Escape))
        {
            jeuChat.SaveGame("autosave.xml");
            Exit();
        }
        
        
        Console.WriteLine("joueurActuel : " + ((jeuChat.JoueurActuel == jeuChat.Joueur1)?"joueur1":"joueur2") + "\n" +
                          "mana : " + jeuChat.JoueurActuel.Jauge + "\n" +
                          "phase : " + jeuChat.Phase + "\n" +
                          "main : " + jeuChat.JoueurActuel.getNbCartesInMain() + "\n" +
                          "carteI : " + jeuChat.CarteI + "\n" +
                          "caseI : " + jeuChat.CaseI + "\n" +
                          "caseJ : " + jeuChat.CaseJ + "\n" +
                          "prevI : " + jeuChat.LastCaseI + "\n" +
                          "prevJ : " + jeuChat.LastCaseJ + "\n");
        
        //on gère les inputs
        jeuChat.transition(keyboardState,_previousKeyboardState);
        //puis, si victoire
        if (jeuChat.victory())
        {
            //alors finir la partie
            jeuChat.EndGame();
            Console.WriteLine("-------------------------GAGNÉ--------------------------------");
        }
        
        
        _previousKeyboardState = keyboardState;
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        
        int offsetX = (GraphicsDevice.Viewport.Width - 512) / 2;
        int offsetY = (GraphicsDevice.Viewport.Height - 320) / 2;

        _spriteBatch.Begin();
        
        Rectangle destbackground = new Rectangle(0, 0, GraphicsDevice.Viewport.Width , GraphicsDevice.Viewport.Height);
        _spriteBatch.Draw(_background, destbackground, Color.White);


        //draw du plateau

        int taillel  = jeuChat.Longueur() * taillecase;
        int tailleh = jeuChat.Largeur() * taillecase;


        int plateauX = (GraphicsDevice.Viewport.Width  - taillel)  / 2;
        int plateauY = (GraphicsDevice.Viewport.Height - tailleh) / 2;

        for (int j = 0; j < jeuChat.Largeur(); j++)     
        {
            for (int i = 0; i < jeuChat.Longueur(); i++) 
            {
                int caseX = plateauX + i * taillecase;
                int caseY = plateauY + j * taillecase;

                Rectangle destcase = new Rectangle(caseX, caseY, taillecase, taillecase);
                
                bool selection =
                    (i == jeuChat.CaseI && j == jeuChat.CaseJ) ||
                    (i == jeuChat.LastCaseI && j == jeuChat.LastCaseJ);

                Color tint = selection ? Color.Cyan : Color.White;
                if (selection)
                {
                    switch (jeuChat.Phase)
                    {
                        case EtatAutomate.SELECTION_CASE_CARTE:
                            tint = Color.Yellow;
                            break;
                        case EtatAutomate.SELECTION_CASE_SOURCE:
                            tint = Color.DeepSkyBlue;
                            break;
                        case EtatAutomate.SELECTION_CASE_CIBLE:
                            tint = Color.Cyan;
                            break;
                    }
                }
                
                _spriteBatch.Draw(_case, destcase, tint);

                // draw du pion
                Rectangle destpion = new Rectangle(caseX+2, caseY-20, 50, 70);

                if (!jeuChat.Plateau.isEmpty(j, i))
                {
                    Invocation entite= jeuChat.Plateau.getEntityAt(j, i);
                    
                    String ImageEntite= entite.Image;
                    Color c;
                    if (entite.PseudoInvocateur==jeuChat.Joueur1.Pseudo)
                    {
                        c= Color.LightSkyBlue;
                    }
                    else
                    {
                        c= Color.PaleVioletRed;
                    }

                    DrawVie(entite, caseX, caseY);
                    SpriteEffects spriteEffect =(!jeuChat.Plateau.isTower(entite) && entite.PseudoInvocateur == jeuChat.Joueur2.Pseudo) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                    DrawCarte(destpion, c, ImageEntite, 0,spriteEffect);
                }
            }
        }

        
        //afficher cartes joueurs
       
        DrawAllCarte(
            jeuChat.Joueur1,             
            plateauX,                         
            GraphicsDevice.Viewport.Height - 30, 
            70,                                 
            520,                                
            1                                   
        );

        
        DrawAllCarte(
            jeuChat.Joueur2,    
            plateauX + 9 * taillecase, 
            -60,                    
            70,                       
            40,                       
            -1                       
        );
        
        DrawMana(jeuChat.Joueur1, GraphicsDevice.Viewport.Width/2-200, GraphicsDevice.Viewport.Height - manaY);
        DrawMana(jeuChat.Joueur2, GraphicsDevice.Viewport.Width/2-200, manaY);
        
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}