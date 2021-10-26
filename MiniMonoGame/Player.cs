﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace MiniMonoGame
{
    public class Player : IPlayer
    {
        public bool Dead { get; private set; }
        public IBullet[] Bullets { get; private set; }
        public int Score { get; private set; }
        public float ExplosionTimer { get; private set; }
        public Texture2D Texture { get; private set; }
        public Vector2 Position { get; private set; }
        public Vector2 Scale { get; private set; }

        private Vector2 direction;
        private float rotation;
        private float speed;
        private float rotationSpeed;
        private float movementTolerance;
        private float rotationDestination;
        private bool move;
        private bool shoot;
        private bool stopShoot;
        private float rotationAlpha;
        private Vector2 destination;
        private Vector2 shootDirection;
        private Vector2 forwardDirection;
        private Vector2 rightDirection;
        private bool increaseScore;
        private Texture2D explosionTexture;
        private float defaultSpeed;
        private float speedBoost;

        public void Init(Vector2 position, Vector2 scale, float rotation = 0.0f, float speed = 100.0f, float rotationSpeed = 1.0f, float movementTolerance = 1.0f, int numberOfBullets = 100)
        {
            this.speed = speed;
            this.rotationSpeed = rotationSpeed;
            this.movementTolerance = movementTolerance;
            Position = position;
            this.rotation = rotation;
            Scale = scale;
            direction = Vector2.Zero;
            defaultSpeed = speed;
            speedBoost = 2.0f;
            move = false;
            Dead = false;
            ExplosionTimer = 0.5f;
            Score = 0;
            increaseScore = false;
            forwardDirection = new Vector2(0.0f, -1.0f);
            rightDirection = new Vector2(1.0f, 0.0f);
            Bullets = new IBullet[numberOfBullets];
            for (int i = 0; i < numberOfBullets; i++)
            {
                IBullet bullet = new Bullet();
                bullet.Init(position, new Vector2(0.2f, 0.2f), true, 0.0f, 600.0f);
                Bullets[i] = bullet;
            }
        }

        public void LoadContent(Texture2D playerTexture, Texture2D explosionTexture, Texture2D bulletTexture)
        {
            Texture = playerTexture;
            this.explosionTexture = explosionTexture;
            foreach (IBullet bullet in Bullets)
            {
                bullet.LoadContent(bulletTexture, explosionTexture);
            }
        }

        public void Update(float deltaTime)
        {
            if (Dead)
            {
                if (ExplosionTimer >= 0.0f)
                {
                    ExplosionTimer -= deltaTime;
                }
                return;
            }

            // Mouse Player movement
            MouseState mouseState = Mouse.GetState();
            if (mouseState.RightButton == ButtonState.Pressed && mouseState.Position.ToVector2() != Position)
            {
                destination = mouseState.Position.ToVector2();
                direction = Position - destination;
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
                Position -= direction * speed * deltaTime;
                Vector2 difference = Position - destination;
                if (Math.Abs(difference.X) < movementTolerance && Math.Abs(difference.Y) < movementTolerance)
                {
                    Position = destination;
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
                Position += speed * deltaTime * forwardDirection;
                move = false;
            }
            if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S) || gamePadState.IsButtonDown(Buttons.LeftTrigger))
            {
                Position -= speed * deltaTime * forwardDirection;
                move = false;
            }
            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A) || gamePadState.IsButtonDown(Buttons.LeftThumbstickLeft))
            {
                Position -= speed * deltaTime * rightDirection;
                move = false;
            }
            if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D) || gamePadState.IsButtonDown(Buttons.LeftThumbstickRight))
            {
                Position += speed * deltaTime * rightDirection;
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
                shootDirection = mouseState.Position.ToVector2() - Position;
                shootDirection.Normalize();
                shoot = true;
            }
            if (keyboardState.IsKeyDown(Keys.Space) || gamePadState.IsButtonDown(Buttons.LeftShoulder))
            {
                shootDirection = forwardDirection;
                shoot = true;
            }

            foreach (IBullet bullet in Bullets)
            {
                bullet.Update(deltaTime, shoot, out stopShoot, Position, shootDirection, out increaseScore);
                if (stopShoot)
                {
                    shoot = false;
                }
                if (increaseScore)
                {
                    Score++;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!Dead)
            {
                spriteBatch.Draw(Texture, Position, null, Color.White, rotation, new Vector2(Texture.Width * 0.5f, Texture.Height * 0.5f), Scale, SpriteEffects.None, 0.0f);
            }
            else
            {
                spriteBatch.Draw(explosionTexture, Position, null, Color.White, rotation, new Vector2(Texture.Width * 0.5f, Texture.Height * 0.5f), Scale, SpriteEffects.None, 0.0f);
            }

            foreach (IBullet bullet in Bullets)
            {
                bullet.Draw(spriteBatch);
            }
        }

        public void Die()
        {
            Dead = true;
        }

        private void RestrictToScreen()
        {
            if (Position.X > GAME.ScreenWidth - Texture.Width * 0.5f)
            {
                Position = new Vector2(GAME.ScreenWidth - Texture.Width * 0.5f, Position.Y);
                move = false;
            }
            else if (Position.X < Texture.Width * 0.5f)
            {
                Position = new Vector2(Texture.Width * 0.5f, Position.Y);
                move = false;
            }
            if (Position.Y > GAME.ScreenHeight - Texture.Height * 0.5f)
            {
                Position = new Vector2(Position.X, GAME.ScreenHeight - Texture.Height * 0.5f);
                move = false;
            }
            else if (Position.Y < Texture.Height * 0.5f)
            {
                Position = new Vector2(Position.X, Texture.Height * 0.5f);
                move = false;
            }
        }

        public void Respawn(Vector2 position)
        {
            Position = position;
            Dead = false;
            Score = 0;
            ExplosionTimer = 0.5f;
            rotation = 0.0f;
        }
    }
}
