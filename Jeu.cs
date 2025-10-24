using System;
using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace TestProjet;

public class Jeu : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _background, _case, _carteBase, _carteLui, _lui;
    private int _carteI, _caseI, _caseJ, _prevI, _prevJ;
    private bool _tour; //true = joueur sud
    private int _phase; //0:choisi carte, 1:choisi case, 2:choisi invoc, 3:choisi cible
    private int[,] _map; //0:rien, 1:lui de J1, 2:lui de J2
    private KeyboardState _previousKeyboardState;

    public Jeu()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 512;
        _graphics.PreferredBackBufferHeight = 320;
        _graphics.ApplyChanges();
        Window.Title = "Card Game";
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _carteI = 0;
        _caseI = -1;
        _caseJ = -1;
        _prevI = -1;
        _prevJ = -1;
        _tour = true;
        _phase = 0;
        _map = new int[10,6];
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
        KeyboardState _keyboardState = Keyboard.GetState();
        if (_keyboardState.IsKeyDown(Keys.Escape))
            Exit();

        switch (_phase)
        {
            case 0:
                if (_keyboardState.IsKeyDown(Keys.Q) && !_previousKeyboardState.IsKeyDown(Keys.Q) 
                    || _keyboardState.IsKeyDown(Keys.Left) && !_previousKeyboardState.IsKeyDown(Keys.Left))
                    _carteI = (_carteI > 0) ? _carteI - 1 : _carteI;
                if (_keyboardState.IsKeyDown(Keys.D) && !_previousKeyboardState.IsKeyDown(Keys.D) 
                    || _keyboardState.IsKeyDown(Keys.Right) && !_previousKeyboardState.IsKeyDown(Keys.Right))
                    _carteI = (_carteI < 5) ? _carteI + 1 : _carteI;
                if (_keyboardState.IsKeyDown(Keys.E) && !_previousKeyboardState.IsKeyDown(Keys.E)
                    || _keyboardState.IsKeyDown(Keys.Enter) && !_previousKeyboardState.IsKeyDown(Keys.Enter))
                {
                    _phase = 1;
                    _caseI = 0;
                    _caseJ = 0;
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
                    || _keyboardState.IsKeyDown(Keys.Enter) && !_previousKeyboardState.IsKeyDown(Keys.Enter))
                    && _map[_caseI,_caseJ] == 0)
                {
                    _phase = 2;
                    _map[_caseI, _caseJ] = (_tour) ? 1 : 2;
                    _carteI = -1;
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
                if ((_keyboardState.IsKeyDown(Keys.E) && !_previousKeyboardState.IsKeyDown(Keys.E)
                    || _keyboardState.IsKeyDown(Keys.Enter) && !_previousKeyboardState.IsKeyDown(Keys.Enter))
                    && _map[_caseI,_caseJ] == ((_tour)?1:2))
                {
                    _phase = 3;
                    _prevI = _caseI;
                    _prevJ = _caseJ;
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
                    _phase = 0;
                    _map[_caseI, _caseJ] = _map[_prevI, _prevJ];
                    _map[_prevI, _prevJ] = 0;
                    _tour = !_tour;
                    _carteI = 0;
                    _caseI = -1;
                    _caseJ = -1;
                    _prevI = -1;
                    _prevJ = -1;
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
                if (colone == _caseI+3 && ligne == _caseJ+2
                    || colone == _prevI+3 && ligne == _prevJ+2)
                {
                    _spriteBatch.Draw(_case,new Vector2(colone*32 + _case.Width*0.25f, ligne*32 + _case.Height*0.25f), null, Color.White, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.FlipVertically, 0.0f);
                }
                if (_map[colone-3, ligne-2] == 1)
                {
                    _spriteBatch.Draw(_lui, new Vector2(colone*32, ligne*32), Color.Red);
                }
                else if (_map[colone-3, ligne-2] == 2)
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
            if (colone == _carteI+5 && !_tour)
            {
                _spriteBatch.Draw(_carteBase,new Vector2(colone*32, 1*32), null, Color.White, MathHelper.ToRadians(180), new Vector2(_carteBase.Width,_carteBase.Height), 1.0f, SpriteEffects.None, 0.0f);
            }
            else
            {
                _spriteBatch.Draw(_carteBase,new Vector2(colone*32, 0*32), null, Color.White, MathHelper.ToRadians(180), new Vector2(_carteBase.Width,_carteBase.Height), 1.0f, SpriteEffects.None, 0.0f);
            }
            if (colone == _carteI+5 && _tour)
            {
                _spriteBatch.Draw(_carteLui, new Vector2(colone*32, 8*32), Color.White);
            }
            else
            {
                _spriteBatch.Draw(_carteLui, new Vector2(colone*32, 9*32), Color.White);
            }
        }
        //tests variables
        /*
        if (_tour)
        {
            _spriteBatch.Draw(_carteLui, new Vector2(0, 0), Color.White);
        }
        else
        {
            _spriteBatch.Draw(_carteLui, new Vector2(0, 0), Color.White);
        }
        _spriteBatch.Draw(_lui, new Vector2(_phase*32, 32), Color.White);
        _spriteBatch.Draw(_lui, new Vector2((_carteI+5)*32, 7*32), Color.White);
        */
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}