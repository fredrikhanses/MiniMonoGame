using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;

namespace MiniMonoGame
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        public static ContentManager Loader { get; private set; }
        private const int numberOfEnemies = 100;
        private const int numberOfPlanets = 2;
        private readonly Player player;
        private readonly Enemy[] enemies;
        private readonly Planet[] planets;
        private readonly Boss boss;
        private Texture2D portal;
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SpriteFont spriteFont;
        private const string pausedText = "Paused";
        private Vector2 pauseFontSize;
        private const string deadText = "You Died";
        private Vector2 deadFontSize;
        private const string respawnText = "Press R to Respawn";
        private Vector2 respawnFontSize;
        private const string scoreText = "Score ";
        private Vector2 scoreFontSize;
        private float respawnTimer;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.ApplyChanges();
            graphics.PreferredBackBufferWidth = GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            graphics.ApplyChanges();
            //graphics.ToggleFullScreen();
            graphics.SynchronizeWithVerticalRetrace = false;
            Loader = Content;
            Loader.RootDirectory = "Content";
            IsMouseVisible = true;

            respawnTimer = 1.0f;

            player = new Player();

            enemies = new Enemy[numberOfEnemies];
            for (int i = 0; i < numberOfEnemies; i++)
            {
                enemies[i] = new Enemy();
                if (i % 5 == 0)
                {
                    enemies[i].texturePath = "enemy";
                }
                else if (i % 3 == 0)
                {
                    enemies[i].texturePath = "spaceship";
                }
                else
                {
                    enemies[i].texturePath = "scout-ship";
                }
            }
            boss = new Boss();

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
            player.Init(new Vector2(graphics.PreferredBackBufferWidth * 0.5f, graphics.PreferredBackBufferHeight * 0.75f), Vector2.One, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, 0.0f, 200.0f, 5.0f, 1.0f, 100, enemies, boss);
            
            foreach (Enemy enemy in enemies)
            {
                enemy.Init(new Vector2(graphics.PreferredBackBufferWidth * 0.5f, 32.0f), Vector2.One, player, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, 0.0f, 100.0f, 450.0f, 1.0f, 2.0f, 1, 1);
            }
            boss.Init(new Vector2(graphics.PreferredBackBufferWidth * 0.5f, 32.0f), Vector2.One, player, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, 0.0f, 50.0f, 450.0f, 0.0f, 2.0f, 1, 100);

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

            player.LoadContent(Content.Load<Texture2D>("ship"), Content.Load<Texture2D>("bullet"), Content.Load<Texture2D>("explosion"));

            foreach (Enemy enemy in enemies)
            {
                enemy.LoadContent(Content.Load<Texture2D>(enemy.texturePath), Content.Load<Texture2D>("bullet"), Content.Load<Texture2D>("explosion"));
            }

            boss.LoadContent(Content.Load<Texture2D>("death-star"), Content.Load<Texture2D>("rocket"), Content.Load<Texture2D>("explosion-big"));

            foreach (Planet planet in planets)
            {
                planet.LoadContent(Content.Load<Texture2D>(planet.texturePath));
            }

            portal = Content.Load<Texture2D>("portal");

            Mouse.SetCursor(MouseCursor.FromTexture2D(Content.Load<Texture2D>("cursor"), 0, 0));

            Song song = Content.Load<Song>("sci-fi_theme");
            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = true;

            spriteFont = Content.Load<SpriteFont>("Pause");
            pauseFontSize = spriteFont.MeasureString(pausedText);
            deadFontSize = spriteFont.MeasureString(deadText);
            respawnFontSize = spriteFont.MeasureString(respawnText);
            scoreFontSize = spriteFont.MeasureString(scoreText);
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
                    player.position = new Vector2(graphics.PreferredBackBufferWidth * 0.5f, graphics.PreferredBackBufferHeight * 0.75f);
                    player.dead = false;
                    player.score = 0;
                    player.explosionTimer = 0.5f;
                    player.rotation = 0.0f;
                    foreach (Bullet bullet in player.bullets)
                    {
                        bullet.move = false;
                    }
                    foreach (Enemy enemy in enemies)
                    {
                        enemy.Respawn();
                    }
                    boss.Respawn();
                }
                else
                {
                    player.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
                    return;
                }
            }

            if (!player.dead)
            {
                player.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            bool aliveEnemies = false;
            foreach (Enemy enemy in enemies)
            {
                if (!enemy.dead && enemy.explosionTimer != 0.0f)
                {
                    aliveEnemies = true;
                }
                enemy.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            boss.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            if (!boss.dead)
            {
                aliveEnemies = true;
            }
            if (!aliveEnemies)
            {
                respawnTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (respawnTimer <= 0.0f)
                {
                    foreach (Enemy enemy in enemies)
                    {
                        enemy.Respawn();
                    }
                    boss.Respawn();
                    respawnTimer = 1.0f;
                }
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

            boss.Draw(spriteBatch);

            foreach (Enemy enemy in enemies)
            {
                enemy.Draw(spriteBatch);
            }

            player.Draw(spriteBatch);

            DrawScoreText();

            if (player.dead && player.explosionTimer <= 0.0f)
            {
                DrawPauseText();
                DrawDeadText();
                DrawRespawnText();
            }

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

        private void DrawPauseText()
        {
            spriteBatch.DrawString(spriteFont, pausedText, new Vector2(graphics.PreferredBackBufferWidth * 0.5f - pauseFontSize.X * 0.5f, graphics.PreferredBackBufferHeight * 0.25f - pauseFontSize.Y * 0.5f), Color.Red);
        }

        private void DrawDeadText()
        {
            spriteBatch.DrawString(spriteFont, deadText, new Vector2(graphics.PreferredBackBufferWidth * 0.5f - deadFontSize.X * 0.5f, graphics.PreferredBackBufferHeight * 0.5f - deadFontSize.Y * 0.5f), Color.Red);
        }

        private void DrawRespawnText()
        {
            spriteBatch.DrawString(spriteFont, respawnText, new Vector2(graphics.PreferredBackBufferWidth * 0.5f - respawnFontSize.X * 0.5f, graphics.PreferredBackBufferHeight * 0.75f - respawnFontSize.Y * 0.5f), Color.Red);
        }

        private void DrawScoreText()
        {
            spriteBatch.DrawString(spriteFont, scoreText + player.score, new Vector2(graphics.PreferredBackBufferWidth * 0.1f - scoreFontSize.X * 0.5f, graphics.PreferredBackBufferHeight * 0.1f - scoreFontSize.Y * 0.5f), Color.Cyan);
        }
    }
}
