using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        public Bullet[] bullets;
        private int screenWidth;
        private int screenHeight;
        public int score;
        private bool increaseScore;
        private Texture2D explosionTexture;
        public float explosionTimer;
        private float defaultSpeed;
        private float speedBoost;

        public void Init(Vector2 position, Vector2 scale, int screenWidth, int screenHeight, float rotation = 0.0f, float speed = 100.0f, float rotationSpeed = 1.0f, float movementTolerance = 1.0f, int numberOfBullets = 100, Enemy[] enemies = null, Boss boss = null)
        {
            Init(position, scale, rotation);
            this.speed = speed;
            this.rotationSpeed = rotationSpeed;
            this.movementTolerance = movementTolerance;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            defaultSpeed = speed;
            speedBoost = 2.0f;
            move = false;
            dead = false;
            explosionTimer = 0.5f;
            score = 0;
            increaseScore = false;
            forwardDirection = new Vector2(0.0f, -1.0f);
            rightDirection = new Vector2(1.0f, 0.0f);
            bullets = new Bullet[numberOfBullets];
            for (int i = 0; i < numberOfBullets; i++)
            {
                Bullet bullet = new Bullet();
                bullet.Init(position, new Vector2(0.2f, 0.2f), screenWidth, screenHeight, 0.0f, 600.0f, enemies, null, boss);
                bullets[i] = bullet;
            }
        }

        public void LoadContent(Texture2D playerTexture, Texture2D bulletTexture, Texture2D explosionTexture)
        {
            texture = playerTexture;
            this.explosionTexture = explosionTexture;
            foreach (Bullet bullet in bullets)
            {
                bullet.texture = bulletTexture;
                bullet.explosionTexture = explosionTexture;
            }
        }

        public void Update(float deltaTime)
        {
            if (dead)
            {
                if (explosionTimer >= 0.0f)
                {
                    explosionTimer -= deltaTime;
                }
                return;
            }

            // Mouse Player movement
            MouseState mouseState = Mouse.GetState();
            if (mouseState.RightButton == ButtonState.Pressed && mouseState.Position.ToVector2() != position)
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
            if ((keyboardState.IsKeyDown(Keys.LeftShift) || gamePadState.IsButtonDown(Buttons.RightShoulder)) && !(move && 
                (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W) || gamePadState.IsButtonDown(Buttons.RightTrigger) ||
                keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S) || gamePadState.IsButtonDown(Buttons.LeftTrigger) ||
                keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A) || gamePadState.IsButtonDown(Buttons.LeftThumbstickLeft) ||
                keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D) || gamePadState.IsButtonDown(Buttons.LeftThumbstickRight))))
            {
                speed = defaultSpeed * speedBoost;
            }
            else
            {
                speed = defaultSpeed;
            }
            if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W) || gamePadState.IsButtonDown(Buttons.RightTrigger))
            {
                position += speed * deltaTime * forwardDirection;
                move = false;
            }
            if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S) || gamePadState.IsButtonDown(Buttons.LeftTrigger))
            {
                position -= speed * deltaTime * forwardDirection;
                move = false;
            }
            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A) || gamePadState.IsButtonDown(Buttons.LeftThumbstickLeft))
            {
                position -= speed * deltaTime * rightDirection;
                move = false;
            }
            if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D) || gamePadState.IsButtonDown(Buttons.LeftThumbstickRight))
            {
                position += speed * deltaTime * rightDirection;
                move = false;
            }

            if (keyboardState.IsKeyDown(Keys.Q) || gamePadState.IsButtonDown(Buttons.RightThumbstickLeft))
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
            if (keyboardState.IsKeyDown(Keys.E) || gamePadState.IsButtonDown(Buttons.RightThumbstickRight))
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

            RestrictToScreen();

            //Shoot bullet
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                shootDirection = mouseState.Position.ToVector2() - position;
                shootDirection.Normalize();
                shoot = true;
            }
            if (keyboardState.IsKeyDown(Keys.Space) || gamePadState.IsButtonDown(Buttons.LeftShoulder))
            {
                shootDirection = forwardDirection;
                shoot = true;
            }

            foreach (Bullet bullet in bullets)
            {
                bullet.Update(deltaTime, shoot, out stopShoot, position, shootDirection, out increaseScore);
                if (stopShoot)
                {
                    shoot = false;
                }
                if (increaseScore)
                {
                    score++;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!dead)
            {
                spriteBatch.Draw(texture, position, null, Color.White, rotation, new Vector2(texture.Width * 0.5f, texture.Height * 0.5f), scale, SpriteEffects.None, 0.0f);
            }
            else
            {
                spriteBatch.Draw(explosionTexture, position, null, Color.White, rotation, new Vector2(texture.Width * 0.5f, texture.Height * 0.5f), scale, SpriteEffects.None, 0.0f);
            }

            foreach (Bullet bullet in bullets)
            {
                bullet.Draw(spriteBatch);
            }
        }

        private void RestrictToScreen()
        {
            if (position.X > screenWidth - texture.Width * 0.5f)
            {
                position.X = screenWidth - texture.Width * 0.5f;
                move = false;
            }
            else if (position.X < texture.Width * 0.5f)
            {
                position.X = texture.Width * 0.5f;
                move = false;
            }
            if (position.Y > screenHeight - texture.Height * 0.5f)
            {
                position.Y = screenHeight - texture.Height * 0.5f;
                move = false;
            }
            else if (position.Y < texture.Height * 0.5f)
            {
                position.Y = texture.Height * 0.5f;
                move = false;
            }
        }
    }
}
