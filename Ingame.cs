using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using TestProjet.Scripts;
using Microsoft.Xna.Framework.Content;

namespace TestProjet;

public class InGame
{
    private List<CarteCliquable> _zonesCartes = new List<CarteCliquable>();
    private List<CaseCliquable> _zonesCase = new List<CaseCliquable>();
    private int phase= -2;

    private Texture2D _background, _case, cadre, cadre2;
    private Texture2D jauge, Contourbarmana;

    private KeyboardState _previousKeyboardState;
    private MouseState _previousMouseState;

    private String _carteBaseImage;


    private EtatMenu _menu = EtatMenu.MENUMAIN;

    public Jeu jeuChat;
    private MainMenu menu;

    private SpriteFont _font;
    private static int taillecase = 55;
    private static int manaY = 280;
    
    
    //Pour stats de fin
    public Joueur joueurWin; 

    public InGame()
    {
        jeuChat = new Jeu(10, 5, "Alice", "Bob");
    }

    private class CarteCliquable
    {
        public Rectangle zone;
        public int i;

        public CarteCliquable(Rectangle zone, int i)
        {
            this.zone = zone;
            this.i = i;
        }
    }

    private class CaseCliquable
    {
        public Rectangle zone;
        public int i, j;

        public CaseCliquable(Rectangle zone, int i, int j)
        {
            this.zone = zone;
            this.i = i;
            this.j = j;
        }
    }

    public void DrawCarte(Rectangle dest, Color tint, String image, int degrees,
        SpriteEffects spriteEffect, ContentManager content, SpriteBatch spriteBatch)
    {

        Texture2D texture = content.Load<Texture2D>(image);

        spriteBatch.Draw(
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
    
    public void DrawInfoGene(SpriteBatch spriteBatch, String text)
    {
        
        Rectangle cadreRect = new Rectangle(760, 480, 260, 150);
        spriteBatch.Draw(cadre2, cadreRect, Color.White);
        spriteBatch.DrawString(_font, text, new Vector2(790, 520), Color.Red);
        
    }

    private void DrawAllCarte(
        Joueur joueur,
        int x, // point de départ en X
        int y, // Y normal des cartes
        int espaceCarte, // écart entre les cartes
        int selectionY, // Y quand la carte est "levée"
        int direction, // 1 = vers la droite, -1 = vers la gauche
        ContentManager content,
        SpriteBatch spriteBatch
    )
    {
        for (int cartenum = 0; cartenum < joueur.nbCarteInMain(); cartenum++)
        {
            int cardX = x + direction * cartenum * espaceCarte;

            Rectangle dest = new Rectangle(cardX, y, 105, 150);
            Color tint = Color.White;
            if (joueur == jeuChat.joueurActuel())
            {
                Carte carteAt = joueur.getCarteInMainAt(cartenum);
                string image = carteAt.getImage();

                if (jeuChat.carteI() == cartenum)
                {
                    DrawInfoCarte(carteAt, spriteBatch);
                    
                    dest = new Rectangle(cardX, selectionY, 105, 150);

                    if (jeuChat.getPhase() == EtatAutomate.SELECTION_CASE_CARTE)
                    {
                        tint = Color.Yellow;
                    }

                    if (!jeuChat.peutSelectionnerCarte(cartenum))
                    {
                        tint = Color.Red;
                        DrawInfoGene(spriteBatch, "Mana trop faible !");
          
                    }
                    
                    
                }

                _zonesCartes.Add(new CarteCliquable(dest, cartenum));
                SpriteEffects spriteEffects = (joueur == jeuChat.joueur2())
                    ? SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically
                    : SpriteEffects.None;

                DrawCarte(dest, tint, image, 0, spriteEffects, content, spriteBatch);
            }
            else
            {
                SpriteEffects spriteEffects = (joueur == jeuChat.joueur2())
                    ? SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically
                    : SpriteEffects.None;
                DrawCarte(dest, Color.White, _carteBaseImage, 0, SpriteEffects, content, spriteBatch);
            }


        }
    }

    public void DrawInfoCarte(Carte carte, SpriteBatch spriteBatch)
    {
        if (carte != null)
        {
            Rectangle cadreRect = new Rectangle(-10, -10, 380, 260);
            spriteBatch.Draw(cadre, cadreRect, Color.White);
            spriteBatch.DrawString(_font, carte.ToString(), new Vector2(50, 40), Color.Black);
        }

    }

    public void DrawVie(Invocation invocation, int x, int y, SpriteBatch spriteBatch)
    {
        int vie = invocation.getVie();
        int maxVie = invocation.getMaxVie();

        int largeur = 50;
        int hauteur = 17;

        Rectangle backRect = new Rectangle(x-10, y, largeur, hauteur);

        Color c1, c2;
        if (invocation.getInvocateur() == jeuChat.joueur1())
        {
            c1 = Color.DarkBlue;
            c2 = Color.CornflowerBlue;
     
        }
        else
        {
            c1 = Color.Red;
            c2 = Color.LightSalmon;

        }

        
        spriteBatch.Draw(jauge, backRect, c1);

        int manaH = (int)(largeur * ((float)vie / maxVie));
        Rectangle manaRect = new Rectangle(x-10, y, manaH, hauteur);
        spriteBatch.Draw(jauge, manaRect, c2);
        
        spriteBatch.DrawString(_font, invocation.getVie() + "" , new Vector2(x+2, y-5),
            Color.Black);
    }

    public void DrawMana(Joueur joueur, int x, int y, SpriteBatch spriteBatch)
    {
        int mana = joueur.getJauge();
        int maxMana = Joueur.MAXJAUGE;

        int largeur = 50;
        int hauteur = 170;

        Rectangle backRect = new Rectangle(x+10, y+25, largeur-20, hauteur-50);

        Color c1, c2;
  
        if (joueur == jeuChat.joueur1())
        {
            c1 = Color.DarkBlue;
            c2 = Color.CornflowerBlue;

        }
        else
        {
            c1 = Color.Maroon;
            c2 = Color.Red;
        }
        
        
        spriteBatch.Draw(jauge, backRect, c1);

        // IA generative //
        int manaH = (int)(backRect.Height * (float)mana / maxMana);

        Rectangle manaRect = new Rectangle(
            backRect.X,
            backRect.Y + backRect.Height - manaH,
            backRect.Width,
            manaH
        );
        // IA generative //
        
        spriteBatch.Draw(jauge, manaRect, c2);
        
        spriteBatch.DrawString(_font, joueur.getJauge() + "/" +Joueur.MAXJAUGE , new Vector2(x, y-25),
            Color.Black);
        
        Rectangle ContourmanaRect = new Rectangle(x, y, largeur, hauteur);
        spriteBatch.Draw(Contourbarmana, ContourmanaRect, Color.White);
        
    }


    public void LoadContent(ContentManager content, GraphicsDevice graphics)
    {

        _background = content.Load<Texture2D>("textures/map/background");
        cadre = content.Load<Texture2D>("textures/map/cadre");
        cadre2 = content.Load<Texture2D>("textures/map/cadre2");
        _case = content.Load<Texture2D>("textures/map/case");
        _carteBaseImage = "textures/cards/carte_base";
        _font = content.Load<SpriteFont>("font");
        Contourbarmana = content.Load<Texture2D>("textures/map/barmana");

        jauge = new Texture2D(graphics, 1, 1);
        jauge.SetData(new[]
        {
            Color.White
        });
    }

    public void Update(GameTime gameTime, GraphicsDeviceManager graphics)
    {

        KeyboardState keyboardState = Keyboard.GetState();
        if (keyboardState.IsKeyDown(Keys.Escape))
        {
            CatRoyal.Quitter();
        }
            

        //Console.WriteLine("joueurActuel : " + ((jeuChat.joueurActuel() == jeuChat.joueur1()) ? "joueur1" : "joueur2") +
        //                  "\n" +
        //                  "mana : " + jeuChat.joueurActuel().getJauge() + "\n" +
        //                "phase : " + jeuChat.phase() + "\n" +
        //              "main : " + jeuChat.joueurActuel().getNbCartesInMain() + "\n" +
        //              "carteI : " + jeuChat.carteI() + "\n" +
        //              "caseI : " + jeuChat.caseI() + "\n" +
        //             "caseJ : " + jeuChat.caseJ() + "\n" +
        //              "prevI : " + jeuChat.lastCaseI() + "\n" +
        //             "prevJ : " + jeuChat.lastCaseJ() + "\n");

        //on gère les inputs
        Clic();
        jeuChat.transition(keyboardState, _previousKeyboardState, phase);
        phase = -2;
        
        //puis, si victoire
        
        if (jeuChat.victory())
        {
            //alors finir la partie
            joueurWin = jeuChat.joueurActuel();
            jeuChat.EndGame();
            
            CatRoyal.setMenu(EtatMenu.ENDGAME);
            Console.WriteLine("-------------------------GAGNÉ--------------------------------");
            
        }


        _previousKeyboardState = keyboardState;
    }

    public void Draw(GameTime gameTime, GraphicsDevice graphics, ContentManager content, SpriteBatch spriteBatch)
    {

        graphics.Clear(Color.CornflowerBlue);
        _zonesCartes.Clear();
        _zonesCase.Clear();
        

        Rectangle destbackground = new Rectangle(0, 0, graphics.Viewport.Width, graphics.Viewport.Height);
        spriteBatch.Draw(_background, destbackground, Color.White);


        //draw du plateau
        int taillel = jeuChat.getLongueur() * taillecase;
        int tailleh = jeuChat.getLargeur() * taillecase;

        int plateauX = (graphics.Viewport.Width - taillel) / 2;
        int plateauY = (graphics.Viewport.Height - tailleh) / 2;

        for (int j = 0; j < jeuChat.getLargeur(); j++)
        {
            for (int i = 0; i < jeuChat.getLongueur(); i++)
            {
                int caseX = plateauX + i * taillecase;
                int caseY = plateauY + j * taillecase;

                Rectangle destcase = new Rectangle(caseX, caseY, taillecase, taillecase);
                _zonesCase.Add(new CaseCliquable(destcase, i, j));
                bool selection =
                    (i == jeuChat.caseI() && j == jeuChat.caseJ()) ||
                    (i == jeuChat.lastCaseI() && j == jeuChat.lastCaseJ());

                Color tint = selection ? Color.Cyan : Color.White;
                if (selection)
                {
                    switch (jeuChat.phase())
                    {
                        case EtatAutomate.SELECTION_CASE_CARTE:
                            tint = Color.Red;
                            if (jeuChat.isEmpty(j, i))
                            {
                                tint = Color.Yellow;
                            }
                            else
                            {
                                tint = Color.Red;
                                DrawInfoGene(spriteBatch, "Impossible \n d'invoquer ici !");
                            }
                            break;
                        case EtatAutomate.SELECTION_CASE_SOURCE:
                            
                            tint = Color.Red;
                            Invocation invocAt = null;
                            
                            //si il y a une invocation afficher du bleu
                            if (!jeuChat.isEmpty(j, i))
                            {
                                invocAt = jeuChat.getEntityAt(j, i);
                                tint = Color.DeepSkyBlue;
                            }
                            
                            
                            if (invocAt != null)
                            {
                                if (!invocAt.getPeutBouger())
                                {
                                    DrawInfoGene(spriteBatch, "Impossible de\n bouger cette\ninvocation !");
                                }
                                
                                if (!invocAt.getPeutAttaquer())
                                {
                                    DrawInfoGene(spriteBatch, "Impossible \nd'attaquer avec\ncette invocation !");
                                }
                                
                                //si l invocation peut attaquer ou bouger et appartient au joueur rester en affichage bleu
                                if ((!invocAt.getPeutBouger() && !invocAt.getPeutAttaquer()) ||
                                    !(invocAt.getInvocateur() == jeuChat.joueurActuel()))  
                                {
                                    DrawInfoGene(spriteBatch, "Impossible de\nbouger/attaquer\navec cette invocation");
                                    tint = Color.Red;
                                }
                            }
                            
                            //afficher info carte d une invocation sur le plateau
                            if (invocAt != null)
                            {
                                DrawInfoCarte(invocAt.getCarte(), spriteBatch);
                            }

                            break;
                        case EtatAutomate.SELECTION_CASE_CIBLE:
                 
                            
                            if (j == jeuChat.getLastCaseJ() && i == jeuChat.getLastCaseI())
                            {
                                tint = Color.MediumSeaGreen;
                            }
                            else
                            {
                                tint = Color.Red;
                            }
                            //Console.WriteLine(j+" "+i);
                            //Console.WriteLine(jeuChat.getLastCaseJ()+" "+jeuChat.getLastCaseI());
                            
                            bool go = jeuChat.peutAttaquerOuDeplacer(jeuChat.getLastCaseJ(), jeuChat.getLastCaseI(), j, i);
                            if (go)
                            {
                                tint = Color.DeepSkyBlue;
                                if (!jeuChat.isEmpty(j, i))
                                {
                                    tint = Color.Orange;
                                }
                            }

                            if (tint == Color.Red)
                            {
                                DrawInfoGene(spriteBatch, "Impossible de cibler\n cette case");
                            }
                            
                            break;
                    }
                }

                spriteBatch.Draw(_case, destcase, tint);

                // draw des invocations
                Rectangle destpion = new Rectangle(caseX + 2, caseY - 20, 50, 70);

                if (!jeuChat.isEmpty(j, i))
                {
                    Invocation entite = jeuChat.getEntityAt(j, i);

                    String ImageEntite = entite.getImage();
                    Color c= Color.White;
                    
                    SpriteEffects spriteEffect =
                        (!jeuChat.plateau().isTower(entite) && entite.getInvocateur() == jeuChat.joueur2())
                            ? SpriteEffects.FlipHorizontally
                            : SpriteEffects.None;

                    DrawCarte(destpion, c, ImageEntite, 0, spriteEffect, content, spriteBatch);
                    DrawVie(entite, caseX+13, caseY+30, spriteBatch);
                }
            }
            


        }
        
        //afficher cartes joueurs

        DrawAllCarte(
            jeuChat.joueur1(),
            plateauX,
            graphics.Viewport.Height - 100,
            50,
            450,
            1,
            content,
            spriteBatch
        );


        DrawAllCarte(
            jeuChat.joueur2(),
            plateauX + 9 * taillecase,
            -50,
            50,
            50,
            -1,
            content,
            spriteBatch
        );
    
        //Afficher la jauge de mana
        DrawMana(jeuChat.joueur1(), 180, manaY, spriteBatch);
        DrawMana(jeuChat.joueur2(), 795, manaY, spriteBatch);
    }

    public void Clic()
    {
        MouseState ms = Mouse.GetState();
        
        bool estClique =
            _previousMouseState.LeftButton == ButtonState.Released &&
            ms.LeftButton == ButtonState.Pressed;
        
        phase = -2;

        if (!estClique)
        {
            _previousMouseState = ms;
            return;
        }

        EtatAutomate phaseCourante = jeuChat.getPhase();

        // ici on gere le clique sur carte
        foreach (CarteCliquable r in _zonesCartes)
        {
            if (r.zone.Contains(ms.Position))
            {
                int oldCarte = jeuChat.carteI();

                //Console.WriteLine("Clic carte" + phaseCourante);

                if (phaseCourante == EtatAutomate.SELECTION_CARTE)
                {
                    //valide le clique
                    if (oldCarte == r.i)
                    {
                        phase = 0; 
                    }
                    
                    jeuChat.setCarteI(r.i);
                }
                else
                {
                    jeuChat.setCarteI(r.i);
                    jeuChat.setPhase(EtatAutomate.SELECTION_CARTE);
                    jeuChat.setCaseIJ(-1, -1);
                }
                
                _previousMouseState = ms;
                return;
            }
        }

        // gerer un clique sur plateau 
        foreach (CaseCliquable r in _zonesCase)
        {
            if (r.zone.Contains(ms.Position))
            {
                int oldI = jeuChat.caseI();
                int oldJ = jeuChat.caseJ();
                
                jeuChat.setCaseIJ(r.i, r.j);

                //Console.WriteLine("Clic plateau:" + phaseCourante);

                switch (phaseCourante)
                {
                    case EtatAutomate.SELECTION_CARTE:
                        phase = 1; 
                        break;

                    case EtatAutomate.SELECTION_CASE_CARTE:
                        if (oldI == r.i && oldJ == r.j)
                        {
                            phase = 2; 
                        }
                        break;

                    case EtatAutomate.SELECTION_CASE_SOURCE:
                      
                        if (oldI == r.i && oldJ == r.j)
                        {
                            phase = 3; 
                        }
                        break;

                    case EtatAutomate.SELECTION_CASE_CIBLE:
                        if (oldI == r.i && oldJ == r.j)
                        {
                            phase = 4; 
                        }
                        break;
                }

                _previousMouseState = ms;
                return;
            }
        }

        _previousMouseState = ms;
    }


}