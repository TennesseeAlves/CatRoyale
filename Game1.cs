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
    private Texture2D _background, _case, _carteBase, _carteLui, _lui;
    private KeyboardState previousKeyboardState;
    private Jeu jeu;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 512;
        _graphics.PreferredBackBufferHeight = 320;
        _graphics.ApplyChanges();
        Window.Title = "Card Game";
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        jeu = new Jeu(10, 6, "Alice", "Bob");
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
        _carteLui = Content.Load<Texture2D>("textures/cards/carte_lui");
        _lui = Content.Load<Texture2D>("textures/mobs/lui");
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState keyboardState = Keyboard.GetState();
        if (keyboardState.IsKeyDown(Keys.Escape))
            Exit();
        
        
        Console.WriteLine("joueurActuel : " + ((jeu.joueurActuel() == jeu.joueur1())?"joueur1":"joueur2") + "\n" +
                          "mana : " + jeu.joueurActuel().getJauge() + "\n" +
                          "phase : " + jeu.phase() + "\n" +
                          "main : " + jeu.joueurActuel().getNbCartesInMain() + "\n" +
                          "carteI : " + jeu.carteI() + "\n" +
                          "caseI : " + jeu.caseI() + "\n" +
                          "caseJ : " + jeu.caseJ() + "\n" +
                          "prevI : " + jeu.lastCaseI() + "\n" +
                          "prevJ : " + jeu.lastCaseJ() + "\n");
        
        //on gère les inputs
        jeu.transition(keyboardState,previousKeyboardState);
        //puis, si victoire
        if (jeu.victory())
        {
            //alors finir la partie
            jeu.EndGame();
        }
        
        
        previousKeyboardState = keyboardState;
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();
        _spriteBatch.Draw(_background, Vector2.Zero, Color.White);
        for (int ligne = 2; ligne < 8; ligne++)
        {
            for (int colone = 3; colone < 13; colone++)
            {
                if (colone == jeu.caseI()+3 && ligne == jeu.caseJ()+2
                    || colone == jeu.lastCaseI()+3 && ligne == jeu.lastCaseJ()+2)
                {
                    _spriteBatch.Draw(_case,new Vector2(colone*32 + _case.Width*0.25f, ligne*32 + _case.Height*0.25f), null, Color.White, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.FlipVertically, 0.0f);
                }
                if (!jeu.plateau().isEmpty(ligne-2,colone-3) && jeu.plateau().getEntityAt(ligne-2,colone-3).getInvocateur() == jeu.joueur1())
                {
                    _spriteBatch.Draw(_lui, new Vector2(colone*32, ligne*32), Color.Red);
                }
                else if (!jeu.plateau().isEmpty(ligne-2,colone-3) && jeu.plateau().getEntityAt(ligne-2,colone-3).getInvocateur() == jeu.joueur2())
                {
                    _spriteBatch.Draw(_lui,new Vector2(colone*32, ligne*32), null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.FlipHorizontally, 0.0f);
                }
                else
                {
                    _spriteBatch.Draw(_case, new Vector2(colone*32, ligne*32), Color.White);
                }
            }
        }
        for (int colone = 5; colone < 11; colone++)
        {
            if (colone == jeu.carteI()+5 && jeu.joueurActuel() == jeu.joueur2())
            {
                _spriteBatch.Draw(_carteBase,new Vector2(colone*32, 1*32), null, Color.White, MathHelper.ToRadians(180), new Vector2(_carteBase.Width,_carteBase.Height), 1.0f, SpriteEffects.None, 0.0f);
            }
            else if (colone < jeu.joueur2().getNbCartesInMain()+5)
            {
                _spriteBatch.Draw(_carteBase,new Vector2(colone*32, 0*32), null, Color.White, MathHelper.ToRadians(180), new Vector2(_carteBase.Width,_carteBase.Height), 1.0f, SpriteEffects.None, 0.0f);
            }
            if (colone == jeu.carteI()+5 && jeu.joueurActuel() == jeu.joueur1())
            {
                _spriteBatch.Draw(_carteLui, new Vector2(colone*32, 8*32), Color.White);
            }
            else if (colone < jeu.joueur1().getNbCartesInMain()+5)
            {
                _spriteBatch.Draw(_carteLui, new Vector2(colone*32, 9*32), Color.White);
            }
        }
        
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}