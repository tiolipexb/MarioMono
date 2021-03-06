﻿using Game1.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Game1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Rectangle life;
        string hud;
        SoundEffect jump_sound, kick_sound, mario_theme, life_sound, finish_sound, mariodie_sound;
        SpriteFont Font;
        Texture2D background;
        private Mario Mario;
        private Goomba Goomba;
        private Goomba Goomba2;
        GameState currentState = GameState.Menu;
        private Random random;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            random = new Random();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Mario = new Mario();
            Goomba = new Goomba(random.Next(100, 800));
            Goomba2 = new Goomba(random.Next(100,800));
            life = new Rectangle(300, 230, 48, 48);      
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Font = Content.Load<SpriteFont>("Font");
            background = this.Content.Load<Texture2D>(@"background");

            jump_sound = this.Content.Load<SoundEffect>("jump");
            kick_sound = this.Content.Load<SoundEffect>("kick");
            life_sound = this.Content.Load<SoundEffect>("1up");
            mariodie_sound = this.Content.Load<SoundEffect>("mariodie");
            finish_sound = this.Content.Load<SoundEffect>("courseclear");

            mario_theme = this.Content.Load<SoundEffect>("mario theme");            

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            hud = $"Life {Mario.Life}";

            Window.Title = "X:" + Mario.Person.X + " Y:" + Mario.Person.Y;

            if(currentState == GameState.Menu)
            {
                if(Keyboard.GetState().GetPressedKeys().Length > 0)
                {
                    currentState = GameState.Playing;
                    mario_theme.Play();
                }
            }

            if (currentState == GameState.Playing)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                {
                    Mario.Direction = Side.Right;
                    Mario.Move();
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                {
                    Mario.Direction = Side.Left;
                    Mario.Move();
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    if (Mario.Person.Y == Mario.MARIO_INITIAL_Y_POSITION)
                    {
                        Mario.IsJump = true;
                        jump_sound.Play();
                    }
                }

                if (Mario.IsJump)
                {
                    Mario.Jump();
                }

                if (Mario.Person.Intersects(life))
                {
                    life_sound.Play();
                    Mario.Life++;
                    life.Y = 999;
                }

                MarioIntersects(ref Goomba.Person);
                MarioIntersects(ref Goomba2.Person);

                //Goomba walk
                Goomba.Walk();
                Goomba2.Walk();                
            }

            //Game Over
            if (Mario.Life == 2 && Mario.Person.X == 750)
            {
                if (currentState == GameState.Playing)
                    finish_sound.Play();

                currentState = GameState.GameOver;                
            }

            if(currentState == GameState.GameOver)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    RestartGame();
                }
            }

            base.Update(gameTime);
        }

        private void MarioIntersects(ref Rectangle target)
        {
            if (Mario.Person.Intersects(target))
            {
                if (Mario.IsJump)
                {
                    target.Y = 999;
                    kick_sound.Play();
                }
                else
                {
                    Mario.Life--;
                }
            }
        }

        private void RestartGame()
        {
            finish_sound.Dispose();
            currentState = GameState.Menu;
            Mario = new Mario();
            Goomba = new Goomba(random.Next(100,600));
            Goomba2 = new Goomba(random.Next(300, 800));
            life = new Rectangle(300, 230, 48, 48);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            var center = graphics.GraphicsDevice.Viewport.Bounds.Center.ToVector2();

            spriteBatch.Begin();

            this.spriteBatch.Draw(background, new Rectangle(0, 0, 800, 500), Color.White);

            if(currentState == GameState.Menu)
            {
                var v = new Vector2(Font.MeasureString("Mario Mono!").X / 2, 0);
                spriteBatch.DrawString(Font, "Mario Mono!", center - v, Color.Black);
            }


            if (currentState == GameState.GameOver)
            {
                // Measure the text so we can center it correctly
                var v = new Vector2(Font.MeasureString("Game Over").X / 2, 0);
                spriteBatch.DrawString(Font, "GAME OVER", center - v, Color.Black);
                mario_theme.Dispose();
            }



            if (currentState == GameState.Playing)
            {
                if (Mario.Direction == Side.Right)
                {
                    spriteBatch.Draw(Content.Load<Texture2D>("mario"), Mario.Person, Color.White);
                }
                else if (Mario.Direction == Side.Left)
                {
                    spriteBatch.Draw(Content.Load<Texture2D>("mario2"), Mario.Person, Color.White);
                }

                spriteBatch.Draw(Content.Load<Texture2D>("life"), life, Color.White);

                spriteBatch.Draw(Content.Load<Texture2D>("goomba"), Goomba.Person, Color.White);

                spriteBatch.Draw(Content.Load<Texture2D>("goomba"), Goomba2.Person, Color.White);

                spriteBatch.DrawString(Font, hud, Vector2.Zero, Color.Black);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
