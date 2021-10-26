using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;

namespace MiniMonoGame
{
    public class GAME : Game
    {
        public static ContentManager Loader { get; private set; }
        public static IPlayer Player { get; private set; }
        public static IEnemy[] Enemies { get; private set; }
        public static IPlanet[] Planets { get; private set; }
        public static IBoss Boss { get; private set; }
        public static IEnemy BossEnemy { get; private set; }
        public static int ScreenWidth { get; private set; }
        public static int ScreenHeight { get; private set; }

        private const int numberOfEnemies = 100;
        private const int numberOfPlanets = 2;
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

            Player = new Player();

            Enemies = new IEnemy[numberOfEnemies];
            for (int i = 0; i < numberOfEnemies; i++)
            {
                Enemies[i] = new Enemy();
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
            Player.Init(new Vector2(ScreenWidth * 0.5f, ScreenHeight * 0.75f), Vector2.One, 0.0f, 200.0f, 5.0f, 1.0f, 100);
            
            foreach (IEnemy enemy in Enemies)
            {
                enemy.Init(new Vector2(ScreenWidth * 0.5f, 32.0f), Vector2.One, 0.0f, 100.0f, 450.0f, 1.0f, 2.0f, 1, 1);
            }
            Boss.Init(new Vector2(ScreenWidth * 0.5f, 32.0f), Vector2.One, 0.0f, 50.0f, 450.0f, 0.0f, 2.0f, 1, 1000);

            Random random = new Random();
            foreach (IPlanet planet in Planets)
            {
                planet.Init(new Vector2(random.Next(ScreenWidth), random.Next(ScreenHeight)), Vector2.One, 0.0f, 10.0f, 0.01f, 1.0f);
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Player.LoadContent(Loader.Load<Texture2D>("ship"), Loader.Load<Texture2D>("explosion"), Loader.Load<Texture2D>("bullet"));

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
                    Player.Respawn(new Vector2(ScreenWidth * 0.5f, ScreenHeight * 0.75f));
                    foreach (IBullet bullet in Player.Bullets)
                    {
                        bullet.Respawn();
                    }
                    foreach (IEnemy enemy in Enemies)
                    {
                        enemy.Respawn(new Vector2(ScreenWidth * 0.5f, 32.0f));
                    }
                    Boss.Respawn(new Vector2(ScreenWidth * 0.5f, 32.0f));
                }
                else
                {
                    Player.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
                    return;
                }
            }

            bool aliveEnemies = false;
            foreach (IEnemy enemy in Enemies)
            {
                if (!enemy.Dead && enemy.ExplosionTimer != 0.0f)
                {
                    aliveEnemies = true;
                }
                enemy.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            Boss.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            if (!BossEnemy.Dead)
            {
                aliveEnemies = true;
            }
            if (!aliveEnemies)
            {
                respawnTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (respawnTimer <= 0.0f)
                {
                    foreach (IEnemy enemy in Enemies)
                    {
                        enemy.Respawn(new Vector2(ScreenWidth * 0.5f, 32.0f));
                    }
                    Boss.Respawn(new Vector2(ScreenWidth * 0.5f, 32.0f));
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

            spriteBatch.Draw(portal, new Vector2(ScreenWidth * 0.5f, 32.0f), null, Color.White, 0.0f, new Vector2(portal.Width * 0.5f, portal.Height * 0.5f), new Vector2(0.5f, 0.5f), SpriteEffects.None, 0.0f);

            Boss.Draw(spriteBatch);

            foreach (IEnemy enemy in Enemies)
            {
                enemy.Draw(spriteBatch);
            }

            Player.Draw(spriteBatch);

            DrawScoreText();

            if (Player.Dead && Player.ExplosionTimer <= 0.0f)
            {
                DrawPauseText();
                DrawDeadText();
                DrawRespawnText();
            }

            spriteBatch.End();

            base.Draw(gameTime);
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
            spriteBatch.DrawString(spriteFont, scoreText + Player.Score, new Vector2(ScreenWidth * 0.1f - scoreFontSize.X * 0.5f, ScreenHeight * 0.1f - scoreFontSize.Y * 0.5f), Color.Cyan);
        }
    }
}
