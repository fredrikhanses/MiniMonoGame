using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace MiniMonoGame
{
    public class Player : Entity
    {
        public float speed;
        public float movementTolerance;
        public bool moving;
        public Vector2 destination;

        public void Init(Vector2 position, Vector2 scale, float speed, float movementTolerance)
        {
            InitEntity(position, scale);
            this.speed = speed;
            this.movementTolerance = movementTolerance;
            moving = false;
        }

        public void Update(float deltaTime, int screenWidth, int screenHeight)
        {
            UpdateEntity(deltaTime);

            // Mouse Player movement
            MouseState mouseState = Mouse.GetState();
            if (mouseState.RightButton == ButtonState.Pressed)
            {
                destination = mouseState.Position.ToVector2();
                direction = position - destination;
                direction.Normalize();
                moving = true;
            }
            if (moving)
            {
                position.Y -= speed * deltaTime * direction.Y;
                position.X -= speed * deltaTime * direction.X;
                Vector2 difference = position - destination;
                if (Math.Abs(difference.X) < movementTolerance && Math.Abs(difference.Y) < movementTolerance)
                {
                    position = destination;
                    moving = false;
                }
            }

            // Keyboard/Gamepad Player movement
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W) || gamePadState.ThumbSticks.Right.Y > 0.0f)
            {
                position.Y -= speed * deltaTime;
                moving = false;
            }
            if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S) || gamePadState.ThumbSticks.Right.Y < 0.0f)
            {
                position.Y += speed * deltaTime;
                moving = false;
            }
            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A) || gamePadState.ThumbSticks.Right.X < 0.0f)
            {
                position.X -= speed * deltaTime;
                moving = false;
            }
            if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D) || gamePadState.ThumbSticks.Right.X > 0.0f)
            {
                position.X += speed * deltaTime;
                moving = false;
            }

            if (position.X > screenWidth - texture.Width / 2)
            {
                position.X = screenWidth - texture.Width / 2;
                moving = false;
            }
            else if (position.X < texture.Width / 2)
            {
                position.X = texture.Width / 2;
                moving = false;
            }
            if (position.Y > screenHeight - texture.Height / 2)
            {
                position.Y = screenHeight - texture.Height / 2;
                moving = false;
            }
            else if (position.Y < texture.Height / 2)
            {
                position.Y = texture.Height / 2;
                moving = false;
            }
        }
    }
}
