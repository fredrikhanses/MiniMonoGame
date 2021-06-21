using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace MiniMonoGame
{
    public class Bullet : Entity 
    {
        public float speed;
        public bool shot;

        public void Init(Vector2 position, Vector2 scale, float rotation = 0.0f, float speed = 400.0f)
        {
            InitEntity(position, scale, rotation);
            this.speed = speed;
            shot = false;
        }

        public void Update(float deltaTime, int screenWidth, int screenHeight, Vector2 initPosition, Vector2 initDirecton, Vector2 enemyPosition, Point enemySize)
        {
            UpdateEntity(deltaTime);

            // Shoot Bullet
            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed && !shot)
            {
                position = initPosition;
                direction = mouseState.Position.ToVector2() - position;
                direction.Normalize();
                shot = true;
            }
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Space) && !shot)
            {
                position = initPosition;
                direction = initDirecton;
                shot = true;
            }
            if (shot)
            {
                position.Y += speed * deltaTime * direction.Y;
                position.X += speed * deltaTime * direction.X;
            }
            Rectangle enemyBounds = new Rectangle((enemyPosition - enemySize.ToVector2() * 0.5f).ToPoint(), enemySize);
            if (enemyBounds.Intersects(new Rectangle((position - texture.Bounds.Size.ToVector2() * scale * 0.5f).ToPoint(), (texture.Bounds.Size.ToVector2() * scale).ToPoint())) ||
                position.X > screenWidth - (texture.Width / 2) * scale.X ||
                position.Y > screenHeight - (texture.Height / 2) * scale.Y ||
                position.X < (texture.Width / 2) * scale.X || position.Y < (texture.Height / 2) * scale.Y)
            {
                shot = false;
            }
        }
    }
}
