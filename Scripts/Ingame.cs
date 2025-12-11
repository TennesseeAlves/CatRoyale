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

    private Texture2D _background, _case, cadre, cadre2, boutonquitter, boutonquitter2;
    private Texture2D jauge, contourBarMana;
    

    private KeyboardState _previousKeyboardState;
    private MouseState _previousMouseState;

    private String _carteBaseImage;

    private SpriteFont _font;
    private static int taillecase = 55;
    private static int manaY = 280;
    
    private static bool survolQuitter = false;
    private Rectangle boutonQuitter = new Rectangle(20, 500, 120, 120);
    
    public Joueur joueurWin;

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
        for (int cartenum = 0; cartenum < joueur.getNbCartesInMain(); cartenum++)
        {
            int cardX = x + direction * cartenum * espaceCarte;

            Rectangle dest = new Rectangle(cardX, y, 105, 150);
            Color tint = Color.White;
            if (joueur == CatRoyal.jeuChat.JoueurActuel)
            {
                Carte carteAt = joueur.getCarteInMainAt(cartenum,CatRoyal.jeuChat.CartesExistantes);
                string image = carteAt.Image;

                if (CatRoyal.jeuChat.CarteI == cartenum)
                {
                    DrawInfoCarte(carteAt, spriteBatch);
                    
                    dest = new Rectangle(cardX, selectionY, 105, 150);

                    if (CatRoyal.jeuChat.Phase == EtatAutomate.SELECTION_CASE_CARTE)
                    {
                        tint = Color.Yellow;
                    }

                    if (!CatRoyal.jeuChat.peutSelectionnerCarte(cartenum))
                    {
                        tint = Color.Red;
                        DrawInfoGene(spriteBatch, "Mana trop faible !");
          
                    }
                    
                    
                }

                _zonesCartes.Add(new CarteCliquable(dest, cartenum));
                SpriteEffects spriteEffects = (joueur == CatRoyal.jeuChat.Joueur2)
                    ? SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically
                    : SpriteEffects.None;

                DrawCarte(dest, tint, image, 0, spriteEffects, content, spriteBatch);
            }
            else
            {
                SpriteEffects spriteEffects = (joueur == CatRoyal.jeuChat.Joueur2)
                    ? SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically
                    : SpriteEffects.None;
                DrawCarte(dest, Color.White, _carteBaseImage, 0, spriteEffects, content, spriteBatch);
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
        int vie = invocation.Vie;
        int maxVie = invocation.MaxVie;

        int largeur = 50;
        int hauteur = 17;

        Rectangle backRect = new Rectangle(x-10, y, largeur, hauteur);

        Color c1, c2;
        if (invocation.PseudoInvocateur == CatRoyal.jeuChat.Joueur1.Pseudo)
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
        
        spriteBatch.DrawString(_font, invocation.Vie + "" , new Vector2(x+2, y-5),
            Color.Black);
    }

    public void DrawMana(Joueur joueur, int x, int y, SpriteBatch spriteBatch)
    {
        int mana = joueur.Jauge;
        int maxMana = Joueur.MAXJAUGE;

        int largeur = 50;
        int hauteur = 170;

        Rectangle backRect = new Rectangle(x+10, y+25, largeur-20, hauteur-50);

        Color c1, c2;
  
        if (joueur == CatRoyal.jeuChat.Joueur1)
        {
            c1 = Color.DarkBlue;
            c2 = Color.CornflowerBlue;

        }
        else
        {
            c1 = Color.Maroon;
            c2 = Color.IndianRed;
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
        
        spriteBatch.DrawString(_font, joueur.Jauge + "/" +Joueur.MAXJAUGE , new Vector2(x, y-25),
            Color.Black);
        
        Rectangle contourManaRect = new Rectangle(x, y, largeur, hauteur);
        spriteBatch.Draw(contourBarMana, contourManaRect, Color.White);
        
    }


    public void LoadContent(ContentManager content, GraphicsDevice graphics)
    {

        _background = content.Load<Texture2D>("textures/map/background");
        cadre = content.Load<Texture2D>("textures/map/cadre");
        cadre2 = content.Load<Texture2D>("textures/map/cadre2");
        _case = content.Load<Texture2D>("textures/map/case");
        _carteBaseImage = "textures/cards/carte_base";
        _font = content.Load<SpriteFont>("font");
        contourBarMana = content.Load<Texture2D>("textures/map/barmana");
        boutonquitter= content.Load<Texture2D>("textures/map/boutonquitter");
        boutonquitter2= content.Load<Texture2D>("textures/map/boutonquitter2");

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
            CatRoyal.SaveGame(CatRoyal.autoSaveFileName);
            CatRoyal.Quitter();
        }
            

        //Console.WriteLine("joueurActuel : " + ((CatRoyal.jeuChat.joueurActuel() == CatRoyal.jeuChat.joueur1()) ? "joueur1" : "joueur2") +
        //                  "\n" +
        //                  "mana : " + CatRoyal.jeuChat.joueurActuel().getJauge() + "\n" +
        //                "phase : " + CatRoyal.jeuChat.phase() + "\n" +
        //              "main : " + CatRoyal.jeuChat.joueurActuel().getNbCartesInMain() + "\n" +
        //              "carteI : " + CatRoyal.jeuChat.carteI() + "\n" +
        //              "caseI : " + CatRoyal.jeuChat.caseI() + "\n" +
        //             "caseJ : " + CatRoyal.jeuChat.caseJ() + "\n" +
        //              "prevI : " + CatRoyal.jeuChat.lastCaseI() + "\n" +
        //             "prevJ : " + CatRoyal.jeuChat.lastCaseJ() + "\n");

        //on gère les inputs
        Clic();
        CatRoyal.jeuChat.transition(keyboardState, _previousKeyboardState, phase);
        phase = -2;
        
        //puis, si victoire
        
        if (CatRoyal.jeuChat.victory())
        {
            //alors finir la partie
            joueurWin = CatRoyal.jeuChat.JoueurActuel;
            joueurWin.WinStreak += 1;
            
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
        Color colorInvocation= Color.White;

        Rectangle destbackground = new Rectangle(0, 0, graphics.Viewport.Width, graphics.Viewport.Height);
        spriteBatch.Draw(_background, destbackground, Color.White);
        
       

        //draw du plateau
        int taillel = CatRoyal.jeuChat.Longueur() * taillecase;
        int tailleh = CatRoyal.jeuChat.Largeur() * taillecase;

        int plateauX = (graphics.Viewport.Width - taillel) / 2;
        int plateauY = (graphics.Viewport.Height - tailleh) / 2;

        for (int j = 0; j < CatRoyal.jeuChat.Largeur(); j++)
        {
            for (int i = 0; i < CatRoyal.jeuChat.Longueur(); i++)
            {
                int caseX = plateauX + i * taillecase;
                int caseY = plateauY + j * taillecase;
                colorInvocation= Color.White;
                
                Rectangle destcase = new Rectangle(caseX, caseY, taillecase, taillecase);
                _zonesCase.Add(new CaseCliquable(destcase, i, j));
                bool selection =
                    (i == CatRoyal.jeuChat.CaseI && j == CatRoyal.jeuChat.CaseJ) ||
                    (i == CatRoyal.jeuChat.LastCaseI && j == CatRoyal.jeuChat.LastCaseJ);

                Color tint = selection ? Color.Cyan : Color.White;
                //Console.WriteLine();
                
                if (CatRoyal.jeuChat.Phase == EtatAutomate.SELECTION_CASE_CARTE)
                {
                    if (!CatRoyal.jeuChat.peutInvoquer(CatRoyal.jeuChat.CarteI, j, i))
                    {
                        //Console.WriteLine(j+" "+i);
                        //Console.WriteLine(CatRoyal.jeuChat.CarteI);
                        tint = Color.Gray;
                    }
                }
                
                if (selection)
                {
                    switch (CatRoyal.jeuChat.Phase)
                    {
                        case EtatAutomate.SELECTION_CASE_CARTE:
                            if (CatRoyal.jeuChat.peutInvoquer(CatRoyal.jeuChat.CarteI, j, i))
                            {
                                tint = Color.Yellow;
                            }
                            else
                            {
                                tint = Color.Gray;
                                DrawInfoGene(spriteBatch, "Impossible \n d'invoquer ici !");
                            }
                            break;
                        case EtatAutomate.SELECTION_CASE_SOURCE:
                            
                            tint = Color.Red;
                            Invocation invocAt = null;
                            
                            //si il y a une invocation afficher du bleu
                            if (!CatRoyal.jeuChat.Plateau.isEmpty(j, i))
                            {
                                invocAt = CatRoyal.jeuChat.Plateau.getEntityAt(j, i);
                                tint = Color.DeepSkyBlue;
                            }
                            
                            
                            if (invocAt != null)
                            {
                                if (!invocAt.PeutBouger)
                                {
                                    DrawInfoGene(spriteBatch, "Impossible de\n bouger cette\ninvocation !");
                                }
                                
                                if (!invocAt.PeutAttaquer)
                                {
                                    DrawInfoGene(spriteBatch, "Impossible \nd'attaquer avec\ncette invocation !");
                                }
                                
                                //si l invocation peut attaquer ou bouger et appartient au joueur rester en affichage bleu
                                if ((!invocAt.PeutBouger && !invocAt.PeutAttaquer) ||
                                    invocAt.PseudoInvocateur != CatRoyal.jeuChat.JoueurActuel.Pseudo)  
                                {
                                    DrawInfoGene(spriteBatch, "Impossible de\nbouger/attaquer\navec cette invocation");
                                    tint = Color.Red;
                                }
                            }
                            
                            //afficher info carte d une invocation sur le plateau
                            if (invocAt != null)
                            {
                                //DrawInfoCarte(invocAt.Carte, spriteBatch);
                            }

                            break;
                        case EtatAutomate.SELECTION_CASE_CIBLE:
                 
                            
                            if (j == CatRoyal.jeuChat.LastCaseJ && i == CatRoyal.jeuChat.LastCaseI)
                            {
                                tint = Color.MediumSeaGreen;
                                colorInvocation = Color.MediumSeaGreen;
                            }
                            else
                            {
                                tint = Color.Red;
                            }
                            //Console.WriteLine(j+" "+i);
                            //Console.WriteLine(CatRoyal.jeuChat.getLastCaseJ()+" "+CatRoyal.jeuChat.getLastCaseI());
                            
                            bool go = CatRoyal.jeuChat.peutAttaquerOuDeplacer(CatRoyal.jeuChat.LastCaseJ, CatRoyal.jeuChat.LastCaseI, j, i);
                            if (go)
                            {
                                tint = Color.DeepSkyBlue;
                                if (!CatRoyal.jeuChat.Plateau.isEmpty(j, i))
                                {
                                    tint = Color.Orange;
                                    colorInvocation = Color.Orange;
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
                Rectangle destpion = new Rectangle(caseX, caseY - 18, 55, 70);

                if (!CatRoyal.jeuChat.Plateau.isEmpty(j, i))
                {
                    Invocation entite = CatRoyal.jeuChat.Plateau.getEntityAt(j, i);

                    String ImageEntite = entite.Image;
                    
                    
                    SpriteEffects spriteEffect =
                        (!CatRoyal.jeuChat.Plateau.isTower(entite) && entite.PseudoInvocateur == CatRoyal.jeuChat.Joueur2.Pseudo)
                            ? SpriteEffects.FlipHorizontally
                            : SpriteEffects.None;

                    DrawCarte(destpion, colorInvocation, ImageEntite, 0, spriteEffect, content, spriteBatch);
                    DrawVie(entite, caseX+13, caseY+30, spriteBatch);
                }
            }
            


        }
        
        //afficher cartes joueurs

        DrawAllCarte(
            CatRoyal.jeuChat.Joueur1,
            plateauX,
            graphics.Viewport.Height - 100,
            50,
            450,
            1,
            content,
            spriteBatch
        );


        DrawAllCarte(
            CatRoyal.jeuChat.Joueur2,
            plateauX + 9 * taillecase,
            -50,
            50,
            50,
            -1,
            content,
            spriteBatch
        );
    
        //Afficher la jauge de mana
        DrawMana(CatRoyal.jeuChat.Joueur1, 180, manaY, spriteBatch);
        DrawMana(CatRoyal.jeuChat.Joueur2, 795, manaY, spriteBatch);
        
        //afficher quitter 
        spriteBatch.Draw(survolQuitter? boutonquitter2 : boutonquitter, boutonQuitter, Color.White);
        
    }

    public void Clic()
    {
        MouseState ms = Mouse.GetState();
        
        bool estClique =
            _previousMouseState.LeftButton == ButtonState.Released &&
            ms.LeftButton == ButtonState.Pressed;
        
        phase = -2;
        
        survolQuitter = false;
        if (boutonQuitter.Contains(ms.Position))
        {
            survolQuitter = true;
            if (estClique)
            {
                CatRoyal.setMenu(EtatMenu.MENUMAIN);
                return;
            }
            
        }
        
        if (!estClique)
        {
            _previousMouseState = ms;
            return;
        }
        
       
      
        
        EtatAutomate phaseCourante = CatRoyal.jeuChat.Phase;

        // ici on gere le clique sur carte
        //Console.WriteLine("");
        
        //gerer le chevauchement en partant du haut
        int i;
        for (i = _zonesCartes.Count - 1; i >= 0; i--)
        {
            CarteCliquable r = _zonesCartes[i];
            if (r.zone.Contains(ms.Position))
            {
                int oldCarte = CatRoyal.jeuChat.CarteI;

                //Console.WriteLine("Clic carte" + phaseCourante);

                if (phaseCourante == EtatAutomate.SELECTION_CARTE)
                {
                    //valide le clique
                    //Console.WriteLine(r.i);
                    if (oldCarte == r.i)
                    {
                        //Console.WriteLine("valide");
                        phase = 0; 
                    }
                    
                    CatRoyal.jeuChat.CarteI = r.i;
                }
                else
                {
                    CatRoyal.jeuChat.CarteI = r.i;
                    CatRoyal.jeuChat.Phase = EtatAutomate.SELECTION_CARTE;
                    CatRoyal.jeuChat.CaseI = -1;
                    CatRoyal.jeuChat.CaseJ = -1;
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
                int oldI = CatRoyal.jeuChat.CaseI;
                int oldJ = CatRoyal.jeuChat.CaseJ;
                
                CatRoyal.jeuChat.CaseI = r.i;
                CatRoyal.jeuChat.CaseJ = r.j;

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
                            if (CatRoyal.jeuChat.LastCaseI == CatRoyal.jeuChat.CaseI &&
                                CatRoyal.jeuChat.LastCaseJ == CatRoyal.jeuChat.CaseJ)
                            {
                                phase = -1; 
                            }
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