using System;
using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TestProjet.Scripts;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using TestProjet.Scripts;

namespace TestProjet;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _background, _case, _carteBase, _carteLui, _lui;
    private int _carteI, _caseI, _caseJ, _prevI, _prevJ;
    private String _carteBaseImage;
    private bool _tour; //true = joueur sud
    private int _phase; //0:choisi carte, 1:choisi case, 2:choisi invoc, 3:choisi cible
    private KeyboardState _previousKeyboardState;
    
    private Texture2D jauge;

    
    private Jeu jeuChat;
    private Joueur joueur1;
    private Joueur joueur2;
    
    private SpriteFont _font;
    private MouseState _previousMouseState;
    
    private static int taillecase = 55;

    private static int manaY = 130;  

    public Game1()
    {
        jeuChat = new Jeu(6, 10, "Joueur1", "Joueur2");
        joueur1 = jeuChat.GetJoueur1();
        joueur2 = jeuChat.GetJoueur2();

        Carte TitouChat = new Carte(10, 5, 2, "TitouChat", "textures/cards/carte_lui", TypeDeCarte.COMBATTANT, TypeRarete.COMMUNE,
            "textures/mobs/lui");
        Carte MagiChat = new Carte(25, 8, 4, "MagiChat", "textures/cards/magichat", TypeDeCarte.COMBATTANT, TypeRarete.RARE,
            "textures/mobs/magichat");
        
        Carte Chatiment = new Carte(35, 13, 6, "Chatiment", "textures/cards/chatiment", TypeDeCarte.COMBATTANT, TypeRarete.EPIQUE,
            "textures/mobs/chatiment");
        
        Carte Soin = new Carte(-1, 5, 5, "Soin", "textures/cards/soin", TypeDeCarte.SORT, TypeRarete.COMMUNE,
            "textures/cards/soin");
        
        joueur1.addCarteInMain(TitouChat);
        joueur1.addCarteInMain(Soin);
        joueur1.addCarteInMain(TitouChat);
        joueur1.addCarteInMain(TitouChat);
        joueur1.addCarteInMain(MagiChat);
        joueur1.addCarteInMain(Chatiment);
        
        joueur2.addCarteInMain(TitouChat);
        joueur2.addCarteInMain(TitouChat);
        joueur2.addCarteInMain(TitouChat);
        joueur2.addCarteInMain(MagiChat);
        joueur2.addCarteInMain(Chatiment);
    
        
        _graphics = new GraphicsDeviceManager(this);
        
        _graphics.PreferredBackBufferWidth = 1024;
        _graphics.PreferredBackBufferHeight = 640;
        _graphics.ApplyChanges();
        Window.Title = "Chat jeu";
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _carteI = 0;
        _caseI = -1;
        _caseJ = -1;
        _prevI = -1;
        _prevJ = -1;


        
    }
    public void DrawCarte(Rectangle dest, Color tint, String image, int degrees)
    {
        
        Texture2D texture = Content.Load<Texture2D>(image);
        _spriteBatch.Draw(
            texture,
            dest,
            null,
            tint,
            MathHelper.ToRadians(degrees),
            Vector2.Zero,
            SpriteEffects.None,
            0f
        );
    }
    
    private void DrawAllCarte(
        Joueur joueur,
        bool isTour,   // tour du joueur
        int x,          // point de départ en X
        int y,           // Y normal des cartes
        int espaceCarte,     // écart entre les cartes
        int selectionY,       // Y quand la carte est "levée"
        int direction        // 1 = vers la droite, -1 = vers la gauche
    )
    {
        for (int cartenum = 0; cartenum < joueur.nbCarteInMain(); cartenum++)
        {
            int cardX = x + direction * cartenum * espaceCarte;

            Rectangle dest = new Rectangle(cardX, y, 70, 100);

            if (isTour)
            {
                Carte carteAt = joueur.getCarteInMainAt(cartenum);
                string image = carteAt.getImage();

                if (_carteI == cartenum)
                {
                    dest = new Rectangle(cardX, selectionY, 70, 100);
                    _spriteBatch.DrawString(_font, carteAt.ToString(), new Vector2(20, 20), Color.Black);
                }

                DrawCarte(dest, Color.White, image, 0);
            }
            else
            {
                DrawCarte(dest, Color.White, _carteBaseImage, 0);
            }

            
        }
    }
    
    public void DrawVie(Invocation invocation, int x, int y)
    {
        int vie = invocation.getVie();
        int maxVie = invocation.getMaxVie();

        int largeur= 30;
        int hauteur= 5;
        
        Rectangle backRect = new Rectangle(x, y-5,largeur,hauteur);
        
        Color c1, c2;
        if (invocation.getInvocateur()==joueur1)
        {
            c1 = Color.DarkBlue;
            c2 = Color.CornflowerBlue;
        }
        else
        {
            c1 = Color.Red;
            c2 = Color.LightSalmon;
        }
        _spriteBatch.DrawString(_font, invocation.getVie()+"/"+invocation.getMaxVie(), new Vector2(x-25, y-35), Color.Black);
        _spriteBatch.Draw(jauge, backRect, c1);
        
        int manaH = (int)(largeur * ((float)vie/maxVie));
        Rectangle manaRect = new Rectangle(x, y-5, manaH, hauteur);
        _spriteBatch.Draw(jauge, manaRect, c2);
    }
    
    public void DrawMana(Joueur joueur, int x, int y)
    {
        int mana = joueur.getJauge();
        int maxMana = Joueur.MAXJAUGE;

        int largeur= 200;
        int hauteur= 15;
        
        Rectangle backRect = new Rectangle(x+100, y,largeur,hauteur);
        
        Color c1, c2;
        if (joueur==joueur1)
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
        _carteBase = Content.Load<Texture2D>("textures/cards/carte_base");
        _carteBaseImage = "textures/cards/carte_base";
        _carteLui = Content.Load<Texture2D>("textures/cards/carte_lui");
        _lui = Content.Load<Texture2D>("textures/mobs/lui");
        _font = Content.Load<SpriteFont>("font");

        jauge = new Texture2D(GraphicsDevice, 1, 1);
        jauge.SetData(new[]
        {
            Color.White
        });
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState _keyboardState = Keyboard.GetState();
        MouseState mouseState = Mouse.GetState();
        if (_keyboardState.IsKeyDown(Keys.Escape))
            Exit();

        bool ClicGauche = mouseState.LeftButton == ButtonState.Pressed &&
                         _previousMouseState.LeftButton == ButtonState.Released;
        

        int taillel  = jeuChat.getLargeur() * taillecase;
        int tailleh = jeuChat.getLongueur() * taillecase;

        int plateauX = (GraphicsDevice.Viewport.Width  - taillel)  / 2;
        int plateauY = (GraphicsDevice.Viewport.Height - tailleh) / 2;

        bool ClicSurPlateau = false;
        int ClicI = -1;
        int ClicJ = -1;

        if (ClicGauche)
        {
            int mx = mouseState.X;
            int my = mouseState.Y;

            if (mx >= plateauX && mx < plateauX + taillel &&
                my >= plateauY && my < plateauY + tailleh)
            {
                ClicI = (mx - plateauX) / taillecase;
                ClicJ = (my - plateauY) / taillecase;
                ClicSurPlateau = true;
            }
        }

        int ClicCarteJ1 = -1;
        int ClicCarteJ2 = -1;

        if (ClicGauche)
        {
            int mx = mouseState.X;
            int my = mouseState.Y;
            int c;
            
            for (c = 0; c < joueur1.nbCarteInMain(); c++)
            {
                int cardX = c * 70 + plateauX;
                int cardY = GraphicsDevice.Viewport.Height - 30;
                if (jeuChat.getTour() && _carteI == c)
                {
                    cardY = 520;
                }
                   

                Rectangle cardRect = new Rectangle(cardX, cardY, 70, 100);
                if (cardRect.Contains(mx, my))
                {
                    ClicCarteJ1 = c;
                    break;
                }
            }

            for (c = 0; c < joueur2.nbCarteInMain(); c++)
            {
                int cardX = (plateauX + 9 * taillecase) - c * 70;
                int cardY = -60;
                if (!jeuChat.getTour() && _carteI == c)
                    cardY = 40;

                Rectangle cardRect = new Rectangle(cardX, cardY, 70, 100);
                if (cardRect.Contains(mx, my))
                {
                    ClicCarteJ2 = c;
                    break;
                }
            }
        }

        switch (_phase)
        {
            case 0:
                int nbCartes = jeuChat.getTourJoueur().nbCarteInMain() - 1;
                if (nbCartes < 0)
                {
                    _phase = 2;
                    _carteI = -1;
                    _caseI = 0;
                    _caseJ = 0;
                    break;
                }
                if (_keyboardState.IsKeyDown(Keys.Q) && !_previousKeyboardState.IsKeyDown(Keys.Q)
                    || _keyboardState.IsKeyDown(Keys.Left) && !_previousKeyboardState.IsKeyDown(Keys.Left))
                    _carteI = (_carteI > 0) ? _carteI - 1 : nbCartes;
                if (_keyboardState.IsKeyDown(Keys.D) && !_previousKeyboardState.IsKeyDown(Keys.D)
                    || _keyboardState.IsKeyDown(Keys.Right) && !_previousKeyboardState.IsKeyDown(Keys.Right))
                {
                    _carteI = (_carteI < nbCartes) ? _carteI + 1 : 0;
                    
                }
                    
                if (_keyboardState.IsKeyDown(Keys.E) && !_previousKeyboardState.IsKeyDown(Keys.E)
                    || _keyboardState.IsKeyDown(Keys.Enter) && !_previousKeyboardState.IsKeyDown(Keys.Enter))
                {
                    _phase = 1;
                    _caseI = 0;
                    _caseJ = 0;
                }

                if (ClicGauche)
                {
                    if (jeuChat.getTour() && ClicCarteJ1 >= 0)
                    {
                        _carteI = ClicCarteJ1;
                    }
                    else if (!jeuChat.getTour() && ClicCarteJ2 >= 0)
                    {
                        _carteI = ClicCarteJ2;
                    }

                    if (ClicSurPlateau && _carteI >= 0)
                    {
                        _phase = 1;
                        _caseI = ClicI;
                        _caseJ = ClicJ;
                    }
                }

                break;
            case 1:
                if (_keyboardState.IsKeyDown(Keys.Q) && !_previousKeyboardState.IsKeyDown(Keys.Q)
                    || _keyboardState.IsKeyDown(Keys.Left) && !_previousKeyboardState.IsKeyDown(Keys.Left))
                    _caseI = (_caseI > 0) ? _caseI - 1 : _caseI;
                if (_keyboardState.IsKeyDown(Keys.D) && !_previousKeyboardState.IsKeyDown(Keys.D)
                    || _keyboardState.IsKeyDown(Keys.Right) && !_previousKeyboardState.IsKeyDown(Keys.Right))
                    _caseI = (_caseI < 9) ? _caseI + 1 : _caseI;
                if (_keyboardState.IsKeyDown(Keys.S) && !_previousKeyboardState.IsKeyDown(Keys.S)
                    || _keyboardState.IsKeyDown(Keys.Down) && !_previousKeyboardState.IsKeyDown(Keys.Down))
                    _caseJ = (_caseJ < 5) ? _caseJ + 1 : _caseJ;
                if (_keyboardState.IsKeyDown(Keys.Z) && !_previousKeyboardState.IsKeyDown(Keys.Z)
                    || _keyboardState.IsKeyDown(Keys.Up) && !_previousKeyboardState.IsKeyDown(Keys.Up))
                    _caseJ = (_caseJ > 0) ? _caseJ - 1 : _caseJ;
                if ((_keyboardState.IsKeyDown(Keys.E) && !_previousKeyboardState.IsKeyDown(Keys.E)
                    || _keyboardState.IsKeyDown(Keys.Enter) && !_previousKeyboardState.IsKeyDown(Keys.Enter)))
                {
                    
                    
                    Carte carteInvocation;
                    Joueur joueurCourant = jeuChat.getTourJoueur();
                  
                    carteInvocation= joueurCourant.getCarteInMainAt(_carteI);


                    if (jeuChat.invoke(jeuChat.getTourJoueur(), carteInvocation, _caseJ, _caseI))
                    {
                        _phase = 2;
                        _carteI = -1;
                        joueurCourant.deleteCarteInMain(carteInvocation);
                    };
                   
                    
                }

                if (ClicGauche && ClicSurPlateau)
                {
                    _caseI = ClicI;
                    _caseJ = ClicJ;
                    
                        Carte carteInvocation;
                        Joueur joueurCourant = jeuChat.getTourJoueur();
                        carteInvocation = joueurCourant.getCarteInMainAt(_carteI);

                    if (jeuChat.invoke(jeuChat.getTourJoueur(), carteInvocation, _caseJ, _caseI))
                    {
                        _phase = 2;
                        _carteI = -1;
                        joueurCourant.deleteCarteInMain(carteInvocation);
                    }
                    
                }

                if (_keyboardState.IsKeyDown(Keys.A) && !_previousKeyboardState.IsKeyDown(Keys.A)
                    || _keyboardState.IsKeyDown(Keys.Back) && !_previousKeyboardState.IsKeyDown(Keys.Back))
                {
                    _phase = 0;
                    _caseI = -1;
                    _caseJ = -1;
                }
                break;
            case 2:
                if (_keyboardState.IsKeyDown(Keys.Q) && !_previousKeyboardState.IsKeyDown(Keys.Q)
                    || _keyboardState.IsKeyDown(Keys.Left) && !_previousKeyboardState.IsKeyDown(Keys.Left))
                    _caseI = (_caseI > 0) ? _caseI - 1 : _caseI;
                if (_keyboardState.IsKeyDown(Keys.D) && !_previousKeyboardState.IsKeyDown(Keys.D)
                    || _keyboardState.IsKeyDown(Keys.Right) && !_previousKeyboardState.IsKeyDown(Keys.Right))
                    _caseI = (_caseI < 9) ? _caseI + 1 : _caseI;
                if (_keyboardState.IsKeyDown(Keys.S) && !_previousKeyboardState.IsKeyDown(Keys.S)
                    || _keyboardState.IsKeyDown(Keys.Down) && !_previousKeyboardState.IsKeyDown(Keys.Down))
                    _caseJ = (_caseJ < 5) ? _caseJ + 1 : _caseJ;
                if (_keyboardState.IsKeyDown(Keys.Z) && !_previousKeyboardState.IsKeyDown(Keys.Z)
                    || _keyboardState.IsKeyDown(Keys.Up) && !_previousKeyboardState.IsKeyDown(Keys.Up))
                    _caseJ = (_caseJ > 0) ? _caseJ - 1 : _caseJ;
                Invocation entite = jeuChat.getEntityAt(_caseJ, _caseI);

                if ((_keyboardState.IsKeyDown(Keys.E) && !_previousKeyboardState.IsKeyDown(Keys.E)
                    || _keyboardState.IsKeyDown(Keys.Enter) && !_previousKeyboardState.IsKeyDown(Keys.Enter))
                    && entite != null && entite.getInvocateur() == jeuChat.getTourJoueur())
                {
                    Console.WriteLine(entite.getInvocateur().getPseudo());
                    Console.WriteLine(jeuChat.getTourJoueur().getPseudo());
                    _phase = 3;
                    _prevI = _caseI;
                    _prevJ = _caseJ;
                    
                    
                    
                }

                if (ClicGauche && ClicSurPlateau)
                {
                    _caseI = ClicI;
                    _caseJ = ClicJ;
                    Invocation entiteClick = jeuChat.getEntityAt(_caseJ, _caseI);
                    if (entiteClick != null && entiteClick.getInvocateur() == jeuChat.getTourJoueur())
                    {
                        Console.WriteLine(entiteClick.getInvocateur().getPseudo());
                        Console.WriteLine(jeuChat.getTourJoueur().getPseudo());
                        _phase = 3;
                        _prevI = _caseI;
                        _prevJ = _caseJ;
                    }
                }

                break;
            case 3:
                if (_keyboardState.IsKeyDown(Keys.Q) && !_previousKeyboardState.IsKeyDown(Keys.Q)
                    || _keyboardState.IsKeyDown(Keys.Left) && !_previousKeyboardState.IsKeyDown(Keys.Left))
                    _caseI = (_caseI > 0) ? _caseI - 1 : _caseI;
                if (_keyboardState.IsKeyDown(Keys.D) && !_previousKeyboardState.IsKeyDown(Keys.D)
                    || _keyboardState.IsKeyDown(Keys.Right) && !_previousKeyboardState.IsKeyDown(Keys.Right))
                    _caseI = (_caseI < 9) ? _caseI + 1 : _caseI;
                if (_keyboardState.IsKeyDown(Keys.S) && !_previousKeyboardState.IsKeyDown(Keys.S)
                    || _keyboardState.IsKeyDown(Keys.Down) && !_previousKeyboardState.IsKeyDown(Keys.Down))
                    _caseJ = (_caseJ < 5) ? _caseJ + 1 : _caseJ;
                if (_keyboardState.IsKeyDown(Keys.Z) && !_previousKeyboardState.IsKeyDown(Keys.Z)
                    || _keyboardState.IsKeyDown(Keys.Up) && !_previousKeyboardState.IsKeyDown(Keys.Up))
                    _caseJ = (_caseJ > 0) ? _caseJ - 1 : _caseJ;
                if (_keyboardState.IsKeyDown(Keys.E) && !_previousKeyboardState.IsKeyDown(Keys.E)
                    || _keyboardState.IsKeyDown(Keys.Enter) && !_previousKeyboardState.IsKeyDown(Keys.Enter))
                {

                    if (jeuChat.move(jeuChat.getTourJoueur(), _prevJ, _prevI, _caseJ, _caseI))
                    {
                        _phase = 0;
                        _carteI = 0;
                        _caseI = -1;
                        _caseJ = -1;
                        _prevI = -1;
                        _prevJ = -1;
                    
                        jeuChat.nextTour();
                    }
                    
                    
                    
                }

                if (ClicGauche && ClicSurPlateau)
                {
                    _caseI = ClicI;
                    _caseJ = ClicJ;
                    if (jeuChat.move(jeuChat.getTourJoueur(), _prevJ, _prevI, _caseJ, _caseI))
                    {
                        _phase = 0;
                        _carteI = 0;
                        _caseI = -1;
                        _caseJ = -1;
                        _prevI = -1;
                        _prevJ = -1;
                        jeuChat.nextTour();
                    }
                }

                if (_keyboardState.IsKeyDown(Keys.A) && !_previousKeyboardState.IsKeyDown(Keys.A)
                    || _keyboardState.IsKeyDown(Keys.Back) && !_previousKeyboardState.IsKeyDown(Keys.Back))
                {
                    _phase = 2;
                    _prevI = -1;
                    _prevJ = -1;
                }
                break;
        }
        _previousKeyboardState = _keyboardState;
        _previousMouseState = mouseState;
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

        int taillel  = jeuChat.getLargeur() * taillecase;
        int tailleh = jeuChat.getLongueur() * taillecase;


        int plateauX = (GraphicsDevice.Viewport.Width  - taillel)  / 2;
        int plateauY = (GraphicsDevice.Viewport.Height - tailleh) / 2;

        for (int j = 0; j < jeuChat.getLongueur(); j++)     
        {
            for (int i = 0; i < jeuChat.getLargeur(); i++) 
            {
                int caseX = plateauX + i * taillecase;
                int caseY = plateauY + j * taillecase;

                Rectangle destcase = new Rectangle(caseX, caseY, taillecase, taillecase);
                
                bool selection =
                    (i == _caseI && j == _caseJ) ||
                    (i == _prevI && j == _prevJ);

                Color tint = selection ? Color.Cyan : Color.White;
                if (selection)
                {
                    if (_phase == 1)
                    {
                        tint = Color.Yellow;
                    }
                    else if (_phase == 2)
                    { 
                        tint = Color.Cyan;
                    }else if (_phase == 3)
                    {
                        tint = Color.DeepSkyBlue;
                    }
                }
                else
                {
                    tint = Color.White;
                }
                
                _spriteBatch.Draw(_case, destcase, tint);

                // draw du pion
                Rectangle destpion = new Rectangle(caseX+2, caseY-20, 50, 70);

                if (!jeuChat.isEmpty(j, i))
                {
                    Invocation entite= jeuChat.getEntityAt(j, i);
                    
                    String ImageEntite= entite.getImage();
                    Color c;
                    if (jeuChat.getEntityAt(j, i).getInvocateur()==joueur1)
                    {
                        c= Color.LightSkyBlue;
                    }
                    else
                    {
                        c= Color.PaleVioletRed;
                    }

                    DrawVie(entite, caseX, caseY);
                    DrawCarte(destpion, c, ImageEntite, 0);
                }
            }
        }

        
        //afficher cartes joueurs
       
        DrawAllCarte(
            joueur1,
            jeuChat.getTour(),                  
            plateauX,                         
            GraphicsDevice.Viewport.Height - 30, 
            70,                                 
            520,                                
            1                                   
        );

        
        DrawAllCarte(
            joueur2,
            !jeuChat.getTour(),       
            plateauX + 9 * taillecase, 
            -60,                    
            70,                       
            40,                       
            -1                       
        );
        
        DrawMana(joueur1, GraphicsDevice.Viewport.Width/2-200, GraphicsDevice.Viewport.Height - manaY);
        DrawMana(joueur2, GraphicsDevice.Viewport.Width/2-200, manaY);
        
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    

}
