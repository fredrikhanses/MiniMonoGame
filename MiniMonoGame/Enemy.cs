using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MiniMonoGame
{
    public class Enemy : IEnemy
    {
        public string TexturePath { get; set; }
        public IBullet[] Bullets { get; private set; }
        public Texture2D Texture { get; private set; }
        public Texture2D ExplosionTexture { get; private set; }
        public Vector2 Position { get; private set; }
        public Vector2 Scale { get; private set; }
        public float ExplosionTimer { get; private set; }
        public float Rotation { get; set; }
        public float ChaseRadiusSquared { get; set; }
        public int BaseHealth { get; private set; }
        public int CurrentHealth { get; private set; }
        public bool Dead { get; private set; }

        private Vector2 direction;
        private Vector2 destination;
        private Vector2 forwardDirection;
        private Vector2 rightDirection;
        private float rotationSpeed;
        private float rotationDestination;
        private float rotationAlpha;
        private float speed;
        private float movementTolerance;
        private const int dropChance = 1;
        private bool move;
        private IPlayer chasingPlayer;
        private bool stopShoot;
        private bool _increaseScore;

        public virtual void Init(Vector2 position, Vector2 scale, float rotation = 0.0f, float speed = 100.0f, float chaseRadius = 450.0f, float rotationSpeed = 1.0f, float movementTolerance = 2.0f, int numberOfBullets = 1, int health = 1, int bulletDamage = 1)
        {
            this.speed = speed;
            this.movementTolerance = movementTolerance;
            this.rotationSpeed = rotationSpeed;
            CurrentHealth = health;
            Position = position;
            Rotation = rotation;
            Scale = scale;
            direction = Vector2.Zero;
            BaseHealth = health;
            move = false;
            Dead = false;
            chasingPlayer = null;
            ChaseRadiusSquared = chaseRadius * chaseRadius;
            ExplosionTimer = 0.5f;
            forwardDirection = new Vector2(0.0f, -1.0f);
            rightDirection = new Vector2(1.0f, 0.0f);
            Bullets = new IBullet[numberOfBullets];
            for (int i = 0; i < numberOfBullets; i++)
            {
                IBullet bullet = new Bullet();
                bullet.Init(position, new Vector2(0.2f, 0.2f), false, 0.0f, 400.0f, bulletDamage);
                Bullets[i] = bullet;
            }
        }

        public virtual void LoadContent(Texture2D enemyTexture, Texture2D explosionTexture, Texture2D bulletTexture = null)
        {
            Texture = enemyTexture;
            ExplosionTexture = explosionTexture;
            foreach (IBullet bullet in Bullets)
            {
                bullet.LoadContent(bulletTexture, explosionTexture);
            }
        }

        public virtual void Update(float deltaTime)
        {
            foreach (IBullet bullet in Bullets)
            {
                if (stopShoot || (this is IBoss && !(this as IBoss).Spinning))
                {
                    chasingPlayer = null;
                }
                bullet.Update(deltaTime, chasingPlayer != null, out stopShoot, Position, forwardDirection, out _increaseScore);
            }

            if (Dead)
            {
                if (ExplosionTimer > 0.0f)
                {
                    ExplosionTimer -= deltaTime;
                }
                return;
            }

            // Enemy movement
            foreach (IPlayer player in GAME.Players)
            {
                if (!player.Dead && player.Position != Position)
                {
                    float distanceSquared = (player.Position - Position).LengthSquared();
                    if (distanceSquared < ChaseRadiusSquared)
                    {
                        if (chasingPlayer == null)
                        {
                            chasingPlayer = player;
                        }
                        else if (distanceSquared < (chasingPlayer.Position - Position).LengthSquared())
                        {
                            chasingPlayer = player;
                        }
                    }
                }
            }
            if (chasingPlayer != null)
            {
                destination = chasingPlayer.Position;
                direction = Position - destination;
                direction.Normalize();
                rotationDestination = (float)Math.Atan2(direction.Y * -forwardDirection.X - direction.X * -forwardDirection.Y, direction.X * -forwardDirection.X + direction.Y * -forwardDirection.Y);
                move = true;
                rotationAlpha = 0.0f;
            }
            if (!move)
            {
                Random random = new Random();
                destination = new Vector2(random.Next(0, GAME.ScreenWidth), random.Next(0, GAME.ScreenHeight));
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
                        Rotation += rotationDestination * rotationSpeed * deltaTime;
                        rotationAlpha += rotationSpeed * deltaTime;
                    }
                    else if (rotationDestination > (float)Math.PI)
                    {
                        Rotation -= rotationDestination * rotationSpeed * deltaTime;
                        rotationAlpha += rotationSpeed * deltaTime;
                    }
                    if (Rotation < -2.0f * (float)Math.PI)
                    {
                        Rotation += 2.0f * (float)Math.PI;
                    }
                    if (Rotation > 2.0f * (float)Math.PI)
                    {
                        Rotation -= 2.0f * (float)Math.PI;
                    }
                    rightDirection = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));
                    rightDirection.Normalize();
                    forwardDirection = -new Vector2((float)Math.Cos(Rotation + (float)Math.PI * 0.5f), (float)Math.Sin(Rotation + (float)Math.PI * 0.5f));
                    forwardDirection.Normalize();
                }
                Position -= direction * speed * deltaTime;
                Vector2 difference = Position - destination;
                if (Math.Abs(difference.X) < movementTolerance && Math.Abs(difference.Y) < movementTolerance)
                {
                    Position = destination;
                    move = false;
                }

                //Enemy-Player Collision
                foreach (IPlayer player in GAME.Players)
                {
                    if (!player.Dead && CheckCollision(player))
                    {
                        player.DecreaseHealth(BaseHealth);
                        DecreaseHealth(BaseHealth);
                    }
                }
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (!Dead)
            {
                spriteBatch.Draw(Texture, Position, null, Color.White, Rotation, new Vector2(Texture.Width * 0.5f, Texture.Height * 0.5f), Scale, SpriteEffects.None, 0.0f);
            }
            else if(ExplosionTimer >= 0.0f)
            {
                spriteBatch.Draw(ExplosionTexture, Position, null, Color.White, Rotation, new Vector2(ExplosionTexture.Width * 0.5f, ExplosionTexture.Height * 0.5f), Scale, SpriteEffects.None, 0.0f);
            }
            foreach (IBullet bullet in Bullets)
            {
                bullet.Draw(spriteBatch);
            }
        }

        public virtual void Respawn(Vector2 position)
        {
            move = false;
            ExplosionTimer = 0.5f;
            Position = position;
            Dead = false;
            CurrentHealth = BaseHealth;
            chasingPlayer = null;
            foreach (IBullet bullet in Bullets)
            {
                bullet.Respawn();
            }
        }

        public bool DecreaseHealth(int amount = 1)
        {
            CurrentHealth -= amount;
            if (CurrentHealth <= 0)
            {
                Die();
                return true;
            }
            return false;
        }

        private void Die()
        {
            Dead = true;
            move = false;
            chasingPlayer = null;
            stopShoot = true;
            CurrentHealth = BaseHealth;
            int randomNumber = GAME.RNG.Next(100);
            if (randomNumber < dropChance)
            {
                GAME.AddPickup(Position);
            }
        }

        private Rectangle GetBounds(Vector2 position, Texture2D texture, Vector2 scale)
        {
            return new Rectangle((position - texture.Bounds.Size.ToVector2() * scale * 0.5f).ToPoint(), (texture.Bounds.Size.ToVector2() * scale).ToPoint());
        }

        private bool CheckCollision(IPlayer player)
        {
            return GetBounds(player.Position, player.Texture, player.Scale).Intersects(GetBounds(Position, Texture, Scale));
        }
    }
}
