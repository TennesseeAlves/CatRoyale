using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;
namespace TestProjet.Scripts;

public class MainMenu
{
    private GraphicsDeviceManager _graphics;
    private Texture2D _background, boutonjouer, boutonjouer2, boutoncharger,
        boutoncharger2, boutonquitter2, boutonquitter, cadrestat;
    private SpriteBatch _spriteBatch;
    private SpriteFont font;
    private const int boutonLargeur = 390;
    private const int boutonHauteur = 160;
    private const int x = (1024  - boutonLargeur) / 2;
    private const int y = (640 - boutonHauteur) / 2;
    private const int ecart = 4;
    private const int offsety = 55;
    private Rectangle boutonStart = new Rectangle(x, y+offsety-(boutonHauteur+ecart), boutonLargeur, boutonHauteur);
    private Rectangle boutonCharger = new Rectangle(x, y+offsety, boutonLargeur, boutonHauteur);
    private Rectangle boutonQuitter;
    
    
    private MouseState EtatActuelSouris;
    private MouseState EtatPrecSouris;

    private int select;
    private int selectFile;
    
    private bool charger = false;
    private List<string> fileSave = new List<string>();
    private List<Rectangle> rectSave = new List<Rectangle>();
    
    private void ChargerListe()
    {
        fileSave.Clear();
        rectSave.Clear();
        
        //ajoute les fichiers sauvegardes dans Filesave
        //doc ici https://learn.microsoft.com/fr-fr/dotnet/api/system.io.directory.getfiles?view=net-8.0
        // utilisation de l'ia generative + la doc pour la comprehension/utilisation de System.IO
        fileSave.AddRange(Directory.GetFiles(CatRoyal.savePath));
        //
        int Y = 250;
        int num = 0;

        foreach (string fichier in fileSave)
        {
            Rectangle zone = new Rectangle(300, Y+num*40, 400, 35);
            rectSave.Add(zone);
            num++;
        }
    }

    private void ClicBouton()
    {
        bool ClicGauche = EtatActuelSouris.LeftButton == ButtonState.Pressed &&
                          EtatPrecSouris.LeftButton == ButtonState.Released;
        //Console.WriteLine(EtatActuelSouris);
        int mx = EtatActuelSouris.X;
        int my = EtatActuelSouris.Y;
        select = -1;
        selectFile = -1;
        if (charger)
        {
            int i;
            for (i=0; i < rectSave.Count; i++)
            {
                if (rectSave[i].Contains(mx, my) && ClicGauche)
                {
                    // utilisation de l'ia generative + la doc pour la comprehension/utilisation de System.IO
                    String nom = Path.GetFileName(fileSave[i]);
                    // 
                    
                    Console.WriteLine("Chargement de " + nom);

                    CatRoyal.LoadGame(nom);
                    CatRoyal.SetMenu(EtatMenu.INGAME);
                    return; 
                }
                else
                {
                    if (rectSave[i].Contains(mx, my))
                    {
                        selectFile = i;
                    }
                }
            }

            
        }
        else
        {
            if (boutonStart.Contains(mx, my))
            {
                select = 0;
            }
        
            if (boutonCharger.Contains(mx, my))
            {
           
                select = 1;
            }
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
                    CatRoyal.SetMenu(EtatMenu.INGAME);
                    
                    break;
                case 1:
                    Console.WriteLine("CHARGER");
                    ChargerListe();
                    charger=true;
                    
                    //CatRoyal.LoadGame(CatRoyal.autoSaveFileName);
                    //CatRoyal.setMenu(EtatMenu.INGAME);
                    break;
                case 2:
                    if (!charger)
                    {
                        Console.WriteLine("Quitter");
                        CatRoyal.Quitter();  
                    }
                    else
                    {
                        charger=false; 
                    }
                    
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
        cadrestat= content.Load<Texture2D>("textures/map/cadrestat");
        
        font = content.Load<SpriteFont>("font");
    }

    public void Update()
    {
        EtatActuelSouris = Mouse.GetState();
        ClicBouton();
        EtatPrecSouris = EtatActuelSouris;

    }

    public void Draw(GraphicsDevice graphics, SpriteBatch spriteBatch)
    {
        
        Rectangle destbackground = new Rectangle(0, 0, graphics.Viewport.Width , graphics.Viewport.Height);
        spriteBatch.Draw(_background, destbackground, Color.White);
        
        //affiche tout les noms
        if (charger)
        {
            
            Rectangle cadrestatRect = new Rectangle(graphics.Viewport.Width/2-300,  graphics.Viewport.Height/2-200, 600, 400);
            
            spriteBatch.Draw(cadrestat, cadrestatRect, Color.White);
            spriteBatch.DrawString(font,  "-- Parties sauvegardées --" , new Vector2(cadrestatRect.X+180, cadrestatRect.Y+30), Color.White);
            int i;
            for (i = 0; i<fileSave.Count; i++)
            {
                String nom = Path.GetFileName(fileSave[i]);
                spriteBatch.DrawString(
                    font,
                    nom,
                    new Vector2(rectSave[i].X, rectSave[i].Y),
                    selectFile == i ? Color.Blue:Color.Black 
                );
            }
            boutonQuitter = new Rectangle(graphics.Viewport.Width/2-80, y+ 180, 160, boutonHauteur);
            spriteBatch.Draw(select == 2 ?boutonquitter2:boutonquitter, boutonQuitter, Color.White);
        }
        else
        {
            boutonQuitter = new Rectangle(x + 115, y + boutonHauteur + ecart + offsety, 160, boutonHauteur);
            spriteBatch.Draw(select == 0 ? boutonjouer2:boutonjouer, boutonStart, Color.White );
            spriteBatch.Draw(select == 1 ? boutoncharger2:boutoncharger, boutonCharger, Color.White);
            spriteBatch.Draw(select == 2 ?boutonquitter2:boutonquitter, boutonQuitter, Color.White);
        }
       
        
    }
    
}