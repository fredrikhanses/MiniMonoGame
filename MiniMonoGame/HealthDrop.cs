using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniMonoGame
{
    public class HealthDrop : IHealthDrop
    {
        public Texture2D Texture { get; private set; }
        public bool Used { get; private set; }

        private Vector2 position;
        private Vector2 scale;
        private Vector2 scaleDirection;
        private float rotation;
        private float rotationSpeed;
        private int value;

        public void Init(Vector2 position, Vector2 scale, float rotation = 0.0f, float rotationSpeed = 1.0f, int value = 1)
        {
            this.position = position;
            this.scale = scale;
            this.rotation = rotation;
            this.rotationSpeed = rotationSpeed;
            this.value = value;
            scaleDirection = Vector2.One;
        }

        public void LoadContent(Texture2D texture)
        {
            this.Texture = texture;
        }

        public void Update(float deltaTime)
        {
            if (Used)
            {
                return;
            }

            rotation += deltaTime * rotationSpeed;
            scale += scaleDirection * deltaTime * rotationSpeed;
            if (scale.X > (Vector2.One).X || scale.X < (Vector2.One * 0.5f).X)
            {
                ToggleScaleDirection();
            }

            if (CheckCollision(GAME.Player))
            {
                if (GAME.Player.CurrentHealth < GAME.Player.BaseHealth)
                {
                    GAME.Player.IncreaseHealth(value);
                    Used = true;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Used)
            {
                return;
            }

            spriteBatch.Draw(Texture, position, null, Color.White, rotation, new Vector2(Texture.Width * 0.5f, Texture.Height * 0.5f), scale, SpriteEffects.None, 0.0f);
        }

        public void Spawn(Vector2 position)
        {
            this.position = position;
            Used = false;
        }

        public void Respawn()
        {
            int randomNumber = GAME.RNG.Next(10);
            if (randomNumber == 0)
            {
                position = new Vector2(GAME.RNG.Next(GAME.ScreenWidth), GAME.RNG.Next(GAME.ScreenHeight));
                Used = false;
            }
            else
            {
                Used = true;
            }
        }

        private void ToggleScaleDirection()
        {
            scaleDirection *= -1;
        }

        private Rectangle GetBounds(Vector2 position, Texture2D texture, Vector2 scale)
        {
            return new Rectangle((position - texture.Bounds.Size.ToVector2() * scale * 0.5f).ToPoint(), (texture.Bounds.Size.ToVector2() * scale).ToPoint());
        }

        private bool CheckCollision(IPlayer player)
        {
            return GetBounds(player.Position, player.Texture, player.Scale).Intersects(GetBounds(position, Texture, scale));
        }
    }
}
