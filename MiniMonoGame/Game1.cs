﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;

namespace MiniMonoGame
{
    public class Game1 : Game
    {
        private static readonly int numberOfEnemies = 100;
        private static readonly int numberOfPlanets = 2;
        private readonly Player player;
        private readonly Enemy[] enemies;
        private readonly Planet[] planets;
        private Texture2D portal;
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.ApplyChanges();
            graphics.PreferredBackBufferWidth = GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            graphics.ApplyChanges();
            //graphics.ToggleFullScreen();
            graphics.SynchronizeWithVerticalRetrace = false;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            player = new Player();

            enemies = new Enemy[numberOfEnemies];
            for (int i = 0; i < numberOfEnemies; i++)
            {
                enemies[i] = new Enemy();
            }

            planets = new Planet[numberOfPlanets];
            for (int i = 0; i < numberOfPlanets; i++)
            {
                planets[i] = new Planet();
                if(i % 2 == 0)
                {
                    planets[i].texturePath = "world";
                }
                else
                {
                    planets[i].texturePath = "ringed-planet";
                }
            }
        }

        protected override void Initialize()
        {
            player.Init(new Vector2(graphics.PreferredBackBufferWidth * 0.5f, graphics.PreferredBackBufferHeight * 0.5f), Vector2.One, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, 0.0f, 200.0f, 5.0f, 1.0f, 100, enemies);
            
            foreach (Enemy enemy in enemies)
            {
                enemy.Init(new Vector2(graphics.PreferredBackBufferWidth * 0.5f, 32.0f), Vector2.One, player, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, 0.0f, 100.0f, 450.0f, 1.0f, 2.0f, 1);
            }

            Random random = new Random();
            foreach (Planet planet in planets)
            {
                planet.Init(new Vector2(random.Next(graphics.PreferredBackBufferWidth), random.Next(graphics.PreferredBackBufferHeight)), Vector2.One, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight,  0.0f, 10.0f, 0.01f, 1.0f);
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            player.LoadContent(Content.Load<Texture2D>("ship"), Content.Load<Texture2D>("bullet"));

            foreach (Enemy enemy in enemies)
            {
                enemy.LoadContent(Content.Load<Texture2D>("enemy"), Content.Load<Texture2D>("bullet"));
            }

            foreach (Planet planet in planets)
            {
                planet.LoadContent(Content.Load<Texture2D>(planet.texturePath));
            }

            portal = Content.Load<Texture2D>("portal");

            Mouse.SetCursor(MouseCursor.FromTexture2D(Content.Load<Texture2D>("cursor"), 0, 0));

            Song song = Content.Load<Song>("sci-fi_theme");
            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = true;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            if (player.dead)
            {
                KeyboardState keyboardState = Keyboard.GetState();
                if (keyboardState.IsKeyDown(Keys.R))
                {
                    player.position = new Vector2(graphics.PreferredBackBufferWidth * 0.5f, graphics.PreferredBackBufferHeight * 0.5f);
                    player.dead = false;
                }
                else
                {
                    return;
                }
            }

            if (!player.dead)
            {
                player.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            foreach (Enemy enemy in enemies)
            {
                enemy.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            foreach (Planet planet in planets)
            {
                planet.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            foreach (Planet planet in planets)
            {
                planet.Draw(spriteBatch);
            }

            spriteBatch.Draw(portal, new Vector2(graphics.PreferredBackBufferWidth * 0.5f, 32.0f), null, Color.White, 0.0f, new Vector2(portal.Width * 0.5f, portal.Height * 0.5f), new Vector2(0.5f, 0.5f), SpriteEffects.None, 0.0f);
            
            foreach (Enemy enemy in enemies)
            {
                enemy.Draw(spriteBatch);
            }

            player.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawDebugCollision(Entity entity)
        {
            Texture2D rect = new Texture2D(graphics.GraphicsDevice, entity.texture.Width, entity.texture.Height);
            Color[] data = new Color[entity.texture.Width * entity.texture.Height];
            for (int i = 0; i < data.Length; ++i)
            {
                data[i] = Color.Chocolate;
            }
            rect.SetData(data);
            spriteBatch.Draw(rect, entity.position - entity.texture.Bounds.Size.ToVector2() * 0.5f, null, Color.White, 0.0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.0f);
        }
    }
}
