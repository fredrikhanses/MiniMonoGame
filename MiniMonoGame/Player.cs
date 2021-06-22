using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace MiniMonoGame
{
    public class Player : Entity
    {
        public float speed;
        public float rotationSpeed;
        public float movementTolerance;
        public float rotationDestination;
        public bool move;
        public bool shoot;
        public bool stopShoot;
        public bool dead;
        public float rotationAlpha;
        public Vector2 destination;
        public Vector2 shootDirection;
        public Vector2 forwardDirection;
        public Vector2 rightDirection;

        public void Init(Vector2 position, Vector2 scale, float rotation = 0.0f, float speed = 100.0f, float rotationSpeed = 1.0f, float movementTolerance = 1.0f)
        {
            InitEntity(position, scale, rotation);
            this.speed = speed;
            this.rotationSpeed = rotationSpeed;
            this.movementTolerance = movementTolerance;
            move = false;
            dead = false;
            forwardDirection = new Vector2(0.0f, -1.0f);
            rightDirection = new Vector2(1.0f, 0.0f);
        }

        public void Update(float deltaTime, int screenWidth, int screenHeight)
        {
            UpdateEntity(deltaTime);

            // Mouse Player movement
            MouseState mouseState = Mouse.GetState();
            if (mouseState.RightButton == ButtonState.Pressed && mouseState.Position.ToVector2() != position)// && !move)
            {
                destination = mouseState.Position.ToVector2();
                direction = position - destination;
                direction.Normalize();
                rotationDestination = (float)Math.Atan2(direction.Y * -forwardDirection.X - direction.X * -forwardDirection.Y, direction.X * -forwardDirection.X + direction.Y * -forwardDirection.Y);
                move = true;
                rotationAlpha = 0.0f;
            }
            if (move)
            {
                if (rotationAlpha <= 1.0f)
                {
                    if (rotationDestination <= (float)Math.PI)
                    {
                        rotation += rotationDestination * rotationSpeed * deltaTime;
                        rotationAlpha += rotationSpeed * deltaTime;
                    }
                    else if (rotationDestination > (float)Math.PI)
                    {
                        rotation -= rotationDestination * rotationSpeed * deltaTime;
                        rotationAlpha += rotationSpeed * deltaTime;
                    }
                    if (rotation < -2.0f * (float)Math.PI)
                    {
                        rotation += 2.0f * (float)Math.PI;
                    }
                    if (rotation > 2.0f * (float)Math.PI)
                    {
                        rotation -= 2.0f * (float)Math.PI;
                    }
                    rightDirection = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
                    rightDirection.Normalize();
                    forwardDirection = -new Vector2((float)Math.Cos(rotation + (float)Math.PI * 0.5f), (float)Math.Sin(rotation + (float)Math.PI * 0.5f));
                    forwardDirection.Normalize();
                }
                position.Y -= speed * deltaTime * direction.Y;
                position.X -= speed * deltaTime * direction.X;
                Vector2 difference = position - destination;
                if (Math.Abs(difference.X) < movementTolerance && Math.Abs(difference.Y) < movementTolerance)
                {
                    position = destination;
                    move = false;
                }
            }

            // Keyboard/Game pad Player movement
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W) || gamePadState.ThumbSticks.Right.Y > 0.0f)
            {

                position += speed * deltaTime * forwardDirection;
                move = false;
            }
            if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S) || gamePadState.ThumbSticks.Right.Y < 0.0f)
            {
                position -= speed * deltaTime * forwardDirection;
                move = false;
            }
            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A) || gamePadState.ThumbSticks.Right.X < 0.0f)
            {
                position -= speed * deltaTime * rightDirection;
                move = false;
            }
            if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D) || gamePadState.ThumbSticks.Right.X > 0.0f)
            {
                position += speed * deltaTime * rightDirection;
                move = false;
            }

            if (keyboardState.IsKeyDown(Keys.Q))
            {
                rotation -= rotationSpeed * deltaTime;
                if (rotation < -2.0f * (float)Math.PI)
                {
                    rotation += 2.0f * (float)Math.PI;
                }
                rightDirection = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
                rightDirection.Normalize();
                forwardDirection = -new Vector2((float)Math.Cos(rotation + (float)Math.PI * 0.5f), (float)Math.Sin(rotation + (float)Math.PI * 0.5f));
                forwardDirection.Normalize();
            }
            if (keyboardState.IsKeyDown(Keys.E))
            {
                rotation += rotationSpeed * deltaTime;
                if (rotation > 2.0f * (float)Math.PI)
                {
                    rotation -= 2.0f * (float)Math.PI;
                }
                rightDirection = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
                rightDirection.Normalize();
                forwardDirection = -new Vector2((float)Math.Cos(rotation + (float)Math.PI * 0.5f), (float)Math.Sin(rotation + (float)Math.PI * 0.5f));
                forwardDirection.Normalize();
            }

            if (position.X > screenWidth - texture.Width / 2)
            {
                position.X = screenWidth - texture.Width / 2;
                move = false;
            }
            else if (position.X < texture.Width / 2)
            {
                position.X = texture.Width / 2;
                move = false;
            }
            if (position.Y > screenHeight - texture.Height / 2)
            {
                position.Y = screenHeight - texture.Height / 2;
                move = false;
            }
            else if (position.Y < texture.Height / 2)
            {
                position.Y = texture.Height / 2;
                move = false;
            }

            //Shoot bullet
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                shootDirection = mouseState.Position.ToVector2() - position;
                shootDirection.Normalize();
                shoot = true;
            }
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                shootDirection = forwardDirection;
                shoot = true;
            }
        }
    }
}
