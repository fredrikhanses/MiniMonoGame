using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MiniMonoGame
{
    public class Game1 : Game
    {
        private readonly Player player = new Player();
        private readonly Bullet bullet = new Bullet();
        private readonly Enemy enemy = new Enemy();

        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            player.Init(new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2), Vector2.One, 100.0f, 1.0f);
            enemy.Init(new Vector2(_graphics.PreferredBackBufferWidth / 2, 0),Vector2.One, 50.0f);
            bullet.Init(player.position, new Vector2(0.2f, 0.2f), 200.0f);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            player.texture = Content.Load<Texture2D>("ball");
            bullet.texture = player.texture;
            enemy.texture = player.texture;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            player.Update((float)gameTime.ElapsedGameTime.TotalSeconds, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            bullet.Update((float)gameTime.ElapsedGameTime.TotalSeconds, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight, player.position);
            enemy.Update((float)gameTime.ElapsedGameTime.TotalSeconds, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight, (int)gameTime.TotalGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _spriteBatch.Draw(player.texture, player.position, null, Color.White, 0.0f, new Vector2(player.texture.Width / 2, player.texture.Height / 2), Vector2.One, SpriteEffects.None, 0.0f);
            if (bullet.shot)
            {
                _spriteBatch.Draw(bullet.texture, bullet.position, null, Color.White, 0.0f, new Vector2(bullet.texture.Width / 2, bullet.texture.Height / 2), bullet.scale, SpriteEffects.None, 0.0f);
            }
            _spriteBatch.Draw(enemy.texture, enemy.position, null, Color.White, 0.0f, new Vector2(enemy.texture.Width / 2, enemy.texture.Height / 2), Vector2.One, SpriteEffects.None, 0.0f);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
