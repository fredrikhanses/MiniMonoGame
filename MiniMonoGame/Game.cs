using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace MiniMonoGame
{
    public class GAME : Game
    {
        public static ContentManager Loader { get; private set; }
        public static IPlayer Player { get; private set; }
        public static List<IEnemy> Enemies { get; private set; }
        public static IPlanet[] Planets { get; private set; }
        public static IBoss Boss { get; private set; }
        public static IEnemy BossEnemy { get; private set; }
        public static List<IHealthDrop> HealthDrops { get; private set; }
        public static Random RNG { get; private set; }
        public static int ScreenWidth { get; private set; }
        public static int ScreenHeight { get; private set; }

        private readonly GraphicsDeviceManager graphics;
        private Texture2D portal;
        private SpriteBatch spriteBatch;
        private SpriteFont spriteFont;
        private Vector2 playerStart;
        private Vector2 enemyStart;
        private Vector2 pauseFontSize;
        private Vector2 deadFontSize;
        private Vector2 respawnFontSize;
        private Vector2 scoreFontSize;
        private Vector2 healthFontSize;
        private Vector2 bossHealthFontSize;
        private Vector2 enemiesLeftFontSize;
        private Vector2 levelFontSize;
        private const string pausedText = "Paused";
        private const string deadText = "You Died";
        private const string respawnText = "Press R or Start to Respawn";
        private const string scoreText = "Score ";
        private const string healthText = "Health ";
        private const string bossHealthText = "Boss ";
        private const string enemiesLeftText = "Enemies ";
        private const string levelText = "Level ";
        private const int numberOfEnemies = 100;
        private const int numberOfPlanets = 2;
        private float respawnTimer;
        private int enemiesLeft;
        private int currentLevel;

        public GAME()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.ApplyChanges();
            graphics.PreferredBackBufferWidth = GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            graphics.ApplyChanges();
            graphics.ToggleFullScreen();
            graphics.SynchronizeWithVerticalRetrace = false;
            Loader = Content;
            Loader.RootDirectory = "Content";
            IsMouseVisible = true;

            ScreenWidth = graphics.PreferredBackBufferWidth;
            ScreenHeight = graphics.PreferredBackBufferHeight;
            respawnTimer = 1.0f;
            currentLevel = 1;
            RNG = new Random(DateTime.Now.Second);

            Player = new Player();
            playerStart = new Vector2(ScreenWidth * 0.5f, ScreenHeight * 0.75f);

            HealthDrops = new List<IHealthDrop>();
            HealthDrops.Add(new HealthDrop());

            enemyStart = new Vector2(ScreenWidth * 0.5f, ScreenHeight * 0.03f);
            Enemies = new List<IEnemy>(numberOfEnemies);
            for (int i = 0; i < numberOfEnemies; i++)
            {
                Enemies.Add(new Enemy());
                if (i % 5 == 0)
                {
                    Enemies[i].TexturePath = "enemy";
                }
                else if (i % 3 == 0)
                {
                    Enemies[i].TexturePath = "spaceship";
                }
                else
                {
                    Enemies[i].TexturePath = "scout-ship";
                }
            }
            Boss = new Boss();
            BossEnemy = Boss as IEnemy;

            Planets = new IPlanet[numberOfPlanets];
            for (int i = 0; i < numberOfPlanets; i++)
            {
                Planets[i] = new Planet();
                if(i % 2 == 0)
                {
                    Planets[i].TexturePath = "world";
                }
                else
                {
                    Planets[i].TexturePath = "ringed-planet";
                }
            }
        }

        protected override void Initialize()
        {
            Player.Init(playerStart, Vector2.One);

            foreach (IHealthDrop healthdrop in HealthDrops)
            {
                healthdrop.Init(new Vector2(RNG.Next(ScreenWidth), RNG.Next(ScreenHeight)), Vector2.One);
            }

            foreach (IEnemy enemy in Enemies)
            {
                enemy.Init(enemyStart, Vector2.One);
            }
            Boss.Init(enemyStart, Vector2.One, 0.0f, 50.0f, 450.0f, 0.0f, 2.0f, 1, 1000);

            foreach (IPlanet planet in Planets)
            {
                planet.Init(new Vector2(RNG.Next(ScreenWidth), RNG.Next(ScreenHeight)), Vector2.One);
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Player.LoadContent(Loader.Load<Texture2D>("ship"), Loader.Load<Texture2D>("explosion"), Loader.Load<Texture2D>("bullet"));

            foreach (IHealthDrop healthdrop in HealthDrops)
            {
                healthdrop.LoadContent(Loader.Load<Texture2D>("healthDrop"));
            }

            foreach (IEnemy enemy in Enemies)
            {
                enemy.LoadContent(Loader.Load<Texture2D>(enemy.TexturePath), Loader.Load<Texture2D>("explosion"), Loader.Load<Texture2D>("bullet"));
            }

            Boss.LoadContent(Loader.Load<Texture2D>("death-star"), Loader.Load<Texture2D>("explosion-big"), Loader.Load<Texture2D>("rocket"));

            foreach (IPlanet planet in Planets)
            {
                planet.LoadContent(Loader.Load<Texture2D>(planet.TexturePath));
            }

            portal = Loader.Load<Texture2D>("portal");

            Mouse.SetCursor(MouseCursor.FromTexture2D(Loader.Load<Texture2D>("cursor"), 0, 0));

            Song song = Loader.Load<Song>("sci-fi_theme");
            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = true;

            spriteFont = Loader.Load<SpriteFont>("Pause");
            pauseFontSize = spriteFont.MeasureString(pausedText);
            deadFontSize = spriteFont.MeasureString(deadText);
            respawnFontSize = spriteFont.MeasureString(respawnText);
            scoreFontSize = spriteFont.MeasureString(scoreText);
            healthFontSize = spriteFont.MeasureString(healthText);
            bossHealthFontSize = spriteFont.MeasureString(bossHealthText);
            enemiesLeftFontSize = spriteFont.MeasureString(enemiesLeftText);
            levelFontSize = spriteFont.MeasureString(levelText);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            if (!Player.Dead)
            {
                Player.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            else
            {
                KeyboardState keyboardState = Keyboard.GetState();
                GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
                if (keyboardState.IsKeyDown(Keys.R) || gamePadState.IsButtonDown(Buttons.Start))
                {
                    Player.Respawn(playerStart);
                    foreach (IHealthDrop healthdrop in HealthDrops)
                    {
                        healthdrop.Respawn();
                    }
                    foreach (IBullet bullet in Player.Bullets)
                    {
                        bullet.Respawn();
                    }
                    foreach (IEnemy enemy in Enemies)
                    {
                        enemy.Respawn(enemyStart);
                    }
                    Boss.Respawn(enemyStart);
                }
                else
                {
                    Player.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
                    return;
                }
            }

            foreach (IHealthDrop healthdrop in HealthDrops)
            {
                healthdrop.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            enemiesLeft = 0;
            foreach (IEnemy enemy in Enemies)
            {
                if (!enemy.Dead && enemy.ExplosionTimer != 0.0f)
                {
                    enemiesLeft++;
                }
                enemy.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            if (enemiesLeft == 0 || BossEnemy.CurrentHealth <= BossEnemy.BaseHealth / 2)
            {
                Boss.StartToSpin();
            }
            Boss.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            if (!BossEnemy.Dead)
            {
                enemiesLeft++;
            }
            if (enemiesLeft == 0)
            {
                respawnTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (respawnTimer <= 0.0f)
                {
                    currentLevel++;
                    Enemies.Capacity = Enemies.Capacity + numberOfEnemies;
                    for (int i = 0; i < numberOfEnemies; i++)
                    {
                        Enemies.Add(new Enemy());
                        Enemies[Enemies.Count - 1].Init(enemyStart, Vector2.One);
                        Enemies[Enemies.Count - 1].LoadContent(Enemies[i].Texture, Enemies[i].ExplosionTexture, Enemies[i].Bullets[0].Texture);
                    }
                    foreach (IEnemy enemy in Enemies)
                    {
                        enemy.Respawn(enemyStart);
                    }
                    Boss.Respawn(enemyStart);
                    respawnTimer = 1.0f;
                }
            }

            foreach (IPlanet planet in Planets)
            {
                planet.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            foreach (IPlanet planet in Planets)
            {
                planet.Draw(spriteBatch);
            }

            spriteBatch.Draw(portal, enemyStart, null, Color.White, 0.0f, new Vector2(portal.Width * 0.5f, portal.Height * 0.5f), new Vector2(0.5f, 0.5f), SpriteEffects.None, 0.0f);

            Boss.Draw(spriteBatch);

            foreach (IHealthDrop healthdrop in HealthDrops)
            {
                healthdrop.Draw(spriteBatch);
            }

            foreach (IEnemy enemy in Enemies)
            {
                enemy.Draw(spriteBatch);
            }

            Player.Draw(spriteBatch);

            DrawScoreText();
            DrawHealthText();
            DrawEnemiesLeftText();
            DrawBossHealthText();
            DrawLevelText();

            if (Player.Dead && Player.ExplosionTimer <= 0.0f)
            {
                DrawPauseText();
                DrawDeadText();
                DrawRespawnText();
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public static void AddHealthDrop(Vector2 position)
        {
            foreach (IHealthDrop healthdrop in HealthDrops)
            {
                if (healthdrop.Used)
                {
                    healthdrop.Spawn(position);
                    return;
                }
            }
            HealthDrops.Add(new HealthDrop());
            HealthDrops[HealthDrops.Count - 1].Init(position, Vector2.One);
            HealthDrops[HealthDrops.Count - 1].LoadContent(HealthDrops[0].Texture);
        }

        private void DrawPauseText()
        {
            spriteBatch.DrawString(spriteFont, pausedText, new Vector2(ScreenWidth * 0.5f - pauseFontSize.X * 0.5f, ScreenHeight * 0.25f - pauseFontSize.Y * 0.5f), Color.Red);
        }

        private void DrawDeadText()
        {
            spriteBatch.DrawString(spriteFont, deadText, new Vector2(ScreenWidth * 0.5f - deadFontSize.X * 0.5f, ScreenHeight * 0.5f - deadFontSize.Y * 0.5f), Color.Red);
        }

        private void DrawRespawnText()
        {
            spriteBatch.DrawString(spriteFont, respawnText, new Vector2(ScreenWidth * 0.5f - respawnFontSize.X * 0.5f, ScreenHeight * 0.75f - respawnFontSize.Y * 0.5f), Color.Red);
        }

        private void DrawScoreText()
        {
            spriteBatch.DrawString(spriteFont, scoreText + Player.Score, new Vector2(ScreenWidth * 0.1f - scoreFontSize.X * 0.5f, ScreenHeight * 0.15f - scoreFontSize.Y * 0.5f), Color.Cyan);
        }

        private void DrawHealthText()
        {
            spriteBatch.DrawString(spriteFont, healthText + Player.CurrentHealth, new Vector2(ScreenWidth * 0.8f - healthFontSize.X * 0.5f, ScreenHeight * 0.05f - healthFontSize.Y * 0.5f), Color.Red);
        }

        private void DrawEnemiesLeftText()
        {
            spriteBatch.DrawString(spriteFont, enemiesLeftText + enemiesLeft, new Vector2(ScreenWidth * 0.1f - enemiesLeftFontSize.X * 0.5f, ScreenHeight * 0.25f - enemiesLeftFontSize.Y * 0.5f), Color.Yellow);
        }

        private void DrawBossHealthText()
        {
            spriteBatch.DrawString(spriteFont, bossHealthText + BossEnemy.CurrentHealth, new Vector2(ScreenWidth * 0.5f - bossHealthFontSize.X * 0.5f, ScreenHeight * 0.05f - bossHealthFontSize.Y * 0.5f), Color.Yellow);
        }

        private void DrawLevelText()
        {
            spriteBatch.DrawString(spriteFont, levelText + currentLevel, new Vector2(ScreenWidth * 0.1f - levelFontSize.X * 0.5f, ScreenHeight * 0.05f - levelFontSize.Y * 0.5f), Color.Cyan);
        }
    }
}
