using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace MiniMonoGame
{
    public class Player : IPlayer
    {
        public IBullet[] Bullets { get; private set; }
        public Texture2D Texture { get; private set; }
        public Vector2 Position { get; private set; }
        public Vector2 Scale { get; private set; }
        public float ExplosionTimer { get; private set; }
        public int Score { get; private set; }
        public int CurrentHealth { get; private set; }
        public int BaseHealth { get; private set; }
        public float CurrentEnergy { get; private set; }
        public float BaseEnergy { get; private set; }
        public int Index { get; private set; }
        public bool Dead { get; private set; }

        private Texture2D explosionTexture;
        private Texture2D shieldTexture;
        private Vector2 direction;
        private Vector2 destination;
        private Vector2 shootDirection;
        private Vector2 forwardDirection;
        private Vector2 rightDirection;
        private float rotation;
        private float speed;
        private float rotationSpeed;
        private float movementTolerance;
        private float rotationDestination;
        private float rotationAlpha;
        private float defaultSpeed;
        private readonly float speedBoost;
        private bool move;
        private bool shoot;
        private bool stopShoot;
        private bool increaseScore;
        private bool usingShield;

        public Player(int index)
        {
            Index = index;
            direction = Vector2.Zero;
            speedBoost = 2.0f;
            move = false;
            Dead = false;
            ExplosionTimer = 0.5f;
            Score = 0;
            increaseScore = false;
            forwardDirection = new Vector2(0.0f, -1.0f);
            rightDirection = new Vector2(1.0f, 0.0f);
        }

        public void Init(Vector2 position, Vector2 scale, float rotation = 0.0f, float speed = 200.0f, float rotationSpeed = 2.0f, float movementTolerance = 1.0f, int numberOfBullets = 100, int health = 10, float energy = 10.0f, int bulletDamage = 1)
        {
            BaseEnergy = energy;
            CurrentEnergy = BaseEnergy;
            BaseHealth = health;
            CurrentHealth = health;
            defaultSpeed = speed;
            this.speed = speed;
            this.rotationSpeed = rotationSpeed;
            this.movementTolerance = movementTolerance;
            Position = position;
            this.rotation = rotation;
            Scale = scale;
            Bullets = new IBullet[numberOfBullets];
            for (int i = 0; i < numberOfBullets; i++)
            {
                IBullet bullet = new Bullet();
                bullet.Init(position, new Vector2(0.2f, 0.2f), true, 0.0f, 600.0f, bulletDamage);
                Bullets[i] = bullet;
            }
        }

        public void LoadContent(Texture2D playerTexture, Texture2D explosionTexture, Texture2D bulletTexture, Texture2D shieldTexture)
        {
            Texture = playerTexture;
            this.explosionTexture = explosionTexture;
            this.shieldTexture = shieldTexture;
            foreach (IBullet bullet in Bullets)
            {
                bullet.LoadContent(bulletTexture, explosionTexture);
            }
        }

        public void Update(float deltaTime)
        {
            if (Dead)
            {
                if (ExplosionTimer > 0.0f)
                {
                    ExplosionTimer -= deltaTime;
                }
                return;
            }

            // Mouse Player movement
            MouseState mouseState = Mouse.GetState();
            if (mouseState.RightButton == ButtonState.Pressed && Index == 0 && mouseState.Position.ToVector2() != Position)
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
            if ((keyboardState.IsKeyDown(Keys.LeftShift) && Index == 1 || mouseState.XButton1 == ButtonState.Pressed && Index == 0 ||  gamePadState.IsButtonDown(Buttons.RightShoulder)) && !(move && 
                (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W) && Index == 1 || gamePadState.IsButtonDown(Buttons.RightTrigger) ||
                keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S) && Index == 1 || gamePadState.IsButtonDown(Buttons.LeftTrigger) ||
                keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A) && Index == 1 || gamePadState.IsButtonDown(Buttons.LeftThumbstickLeft) ||
                keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D) && Index == 1 || gamePadState.IsButtonDown(Buttons.LeftThumbstickRight))))
            {
                speed = defaultSpeed * speedBoost;
            }
            else
            {
                speed = defaultSpeed;
            }
            if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W) && Index == 1 || gamePadState.IsButtonDown(Buttons.RightTrigger))
            {
                Position += speed * deltaTime * forwardDirection;
                move = false;
            }
            if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S) && Index == 1 || gamePadState.IsButtonDown(Buttons.LeftTrigger))
            {
                Position -= speed * deltaTime * forwardDirection;
                move = false;
            }
            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A) && Index == 1 || gamePadState.IsButtonDown(Buttons.LeftThumbstickLeft))
            {
                Position -= speed * deltaTime * rightDirection;
                move = false;
            }
            if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D) && Index == 1 || gamePadState.IsButtonDown(Buttons.LeftThumbstickRight))
            {
                Position += speed * deltaTime * rightDirection;
                move = false;
            }

            if (keyboardState.IsKeyDown(Keys.Q) && Index == 1 || gamePadState.IsButtonDown(Buttons.RightThumbstickLeft))
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
            if (keyboardState.IsKeyDown(Keys.E) && Index == 1 || gamePadState.IsButtonDown(Buttons.RightThumbstickRight))
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
            if (mouseState.LeftButton == ButtonState.Pressed && Index == 0)
            {
                shootDirection = mouseState.Position.ToVector2() - Position;
                shootDirection.Normalize();
                shoot = true;
            }
            if (keyboardState.IsKeyDown(Keys.Space) && Index == 1 || gamePadState.IsButtonDown(Buttons.LeftShoulder))
            {
                shootDirection = forwardDirection;
                shoot = true;
            }

            //Use Shield
            if ((keyboardState.IsKeyDown(Keys.LeftControl) && Index == 1 || mouseState.XButton2 == ButtonState.Pressed && Index == 0) && CurrentEnergy > 0.0f)
            {
                usingShield = true;
            }
            else
            {
                usingShield = false;
            }

            if (usingShield)
            {
                CurrentEnergy -= deltaTime;
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
            if (Dead && ExplosionTimer <= 0.0f)
            {
                return;
            }

            if (!Dead)
            {
                spriteBatch.Draw(Texture, Position, null, Color.White, rotation, new Vector2(Texture.Width * 0.5f, Texture.Height * 0.5f), Scale, SpriteEffects.None, 0.0f);
            }
            else
            {
                spriteBatch.Draw(explosionTexture, Position, null, Color.White, rotation, new Vector2(Texture.Width * 0.5f, Texture.Height * 0.5f), Scale, SpriteEffects.None, 0.0f);
            }

            if (usingShield)
            {
                spriteBatch.Draw(shieldTexture, Position, null, Color.White, rotation, new Vector2(shieldTexture.Width * 0.5f, shieldTexture.Height * 0.5f), Scale, SpriteEffects.None, 0.0f);
            }

            foreach (IBullet bullet in Bullets)
            {
                bullet.Draw(spriteBatch);
            }
        }

        public void DecreaseHealth(int amount = 1)
        {
            if (!usingShield)
            {
                CurrentHealth -= amount;
                if (CurrentHealth <= 0)
                {
                    Die();
                }
            }
        }

        public void IncreaseHealth(int amount = 1)
        {
            CurrentHealth += amount;
            if (CurrentHealth > BaseHealth)
            {
                CurrentHealth = BaseHealth;
            }
        }

        public void IncreaseEnergy(int amount = 1)
        {
            CurrentEnergy += amount;
            if (CurrentEnergy > BaseEnergy)
            {
                CurrentEnergy = BaseEnergy;
            }
        }

        private void Die()
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
            forwardDirection = new Vector2(0.0f, -1.0f);
            rightDirection = new Vector2(1.0f, 0.0f);
            CurrentHealth = BaseHealth;
            CurrentEnergy = BaseEnergy;
            Position = position;
            Dead = false;
            Score = 0;
            ExplosionTimer = 0.5f;
            rotation = 0.0f;
            foreach (IBullet bullet in Bullets)
            {
                bullet.Respawn();
            }
        }
    }
}
