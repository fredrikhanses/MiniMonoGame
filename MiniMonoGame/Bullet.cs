using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MiniMonoGame
{
    public class Bullet : Entity 
    {
        public float speed;
        public bool shot;

        public void Init(Vector2 position, Vector2 scale, float speed)
        {
            InitEntity(position, scale);
            this.speed = speed;
            shot = false;
        }

        public void Update(float deltaTime, int screenWidth, int screenHeight, Vector2 initPosition)
        {
            UpdateEntity(deltaTime);

            // Shoot Bullet
            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed && !shot)
            {
                position = initPosition;
                direction = position - mouseState.Position.ToVector2();
                direction.Normalize();
                shot = true;
            }
            if (shot)
            {
                position.Y -= speed * deltaTime * direction.Y;
                position.X -= speed * deltaTime * direction.X;
            }
            if (position.X > screenWidth - (texture.Width / 2) * scale.X ||
                position.Y > screenHeight - (texture.Height / 2) * scale.Y ||
                position.X < (texture.Width / 2) * scale.X || position.Y < (texture.Height / 2) * scale.Y)
            {
                shot = false;
            }
        }
    }
}
