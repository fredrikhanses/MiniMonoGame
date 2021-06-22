using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace MiniMonoGame
{
    public class Game1 : Game
    {
        private readonly int numberOfEnemies = 100;
        private readonly int numberOfBullets = 100;
        private readonly Player player;
        private readonly List<Bullet> bullets;
        private readonly List<Enemy> enemies;
        private readonly Planet planet;
        private readonly Planet ringPlanet;
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
            bullets = new List<Bullet>();
            for (int i = 0; i < numberOfBullets; i++)
            {
                bullets.Add(new Bullet());
            }
            enemies = new List<Enemy>();
            for (int i = 0; i < numberOfEnemies; i++)
            {
                enemies.Add(new Enemy());
            }
            planet = new Planet();
            ringPlanet = new Planet();
        }

        protected override void Initialize()
        {
            player.Init(new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2), Vector2.One, 0.0f, 200.0f, 5.0f, 1.0f);
            foreach (Enemy enemy in enemies)
            {
                enemy.Init(new Vector2(graphics.PreferredBackBufferWidth / 2, 32.0f), Vector2.One, 0.0f, 100.0f, 1.0f, 2.0f);
            }
            foreach (Bullet bullet in bullets)
            {
                bullet.Init(player.position, new Vector2(0.2f, 0.2f), 0.0f, 600.0f);
            }
            planet.Init(new Vector2(graphics.PreferredBackBufferWidth / 2, 0.0f), Vector2.One, 0.0f, 10.0f, 0.01f, 1.0f);
            ringPlanet.Init(new Vector2(0.0f, graphics.PreferredBackBufferHeight / 2), Vector2.One, 0.0f, 10.0f, 0.01f, 1.0f);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            player.texture = Content.Load<Texture2D>("ship");
            foreach (Bullet bullet in bullets)
            {
                bullet.texture = Content.Load<Texture2D>("bullet");
            }
            foreach (Enemy enemy in enemies)
            {
                enemy.texture = Content.Load<Texture2D>("enemy");
            }
            planet.texture = Content.Load<Texture2D>("world");
            ringPlanet.texture = Content.Load<Texture2D>("ringed-planet");

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

            player.Update((float)gameTime.ElapsedGameTime.TotalSeconds, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            foreach (Enemy enemy in enemies)
            {
                enemy.Update((float)gameTime.ElapsedGameTime.TotalSeconds, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, player);
            }
            foreach (Bullet bullet in bullets)
            {
                if (player.stopShoot)
                {
                    player.shoot = false;
                }
                bullet.Update((float)gameTime.ElapsedGameTime.TotalSeconds, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, player.shoot, out player.stopShoot, player.position, player.shootDirection, enemies);
            }
            planet.Update((float)gameTime.ElapsedGameTime.TotalSeconds, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            ringPlanet.Update((float)gameTime.ElapsedGameTime.TotalSeconds, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            if (player.dead)
            {
                Exit();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            spriteBatch.Draw(planet.texture, planet.position, null, Color.White, planet.rotation, new Vector2(planet.texture.Width / 2, planet.texture.Height / 2), Vector2.One, SpriteEffects.None, 0.0f);
            spriteBatch.Draw(ringPlanet.texture, ringPlanet.position, null, Color.White, ringPlanet.rotation, new Vector2(ringPlanet.texture.Width / 2, ringPlanet.texture.Height / 2), Vector2.One, SpriteEffects.None, 0.0f);

            foreach (Enemy enemy in enemies)
            {
                //Debug Collision
                //Texture2D rect = new Texture2D(graphics.GraphicsDevice, enemy.texture.Width, enemy.texture.Height);
                //Color[] data = new Color[enemy.texture.Width * enemy.texture.Height];
                //for (int i = 0; i < data.Length; ++i)
                //{
                //    data[i] = Color.Chocolate;
                //}
                //rect.SetData(data);
                //spriteBatch.Draw(rect, enemy.position - enemy.texture.Bounds.Size.ToVector2() * 0.5f, null, Color.White, 0.0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.0f);

                spriteBatch.Draw(enemy.texture, enemy.position, null, Color.White, enemy.rotation, new Vector2(enemy.texture.Width / 2, enemy.texture.Height / 2), Vector2.One, SpriteEffects.None, 0.0f);
            }

            spriteBatch.Draw(player.texture, player.position, null, Color.White, player.rotation, new Vector2(player.texture.Width / 2, player.texture.Height / 2), Vector2.One, SpriteEffects.None, 0.0f);
            foreach (Bullet bullet in bullets)
            {
                if (bullet.move)
                {
                    spriteBatch.Draw(bullet.texture, bullet.position, null, Color.White, bullet.rotation, new Vector2(bullet.texture.Width / 2, bullet.texture.Height / 2), bullet.scale, SpriteEffects.None, 0.0f);
                }
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
