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
        public Vector2 Position { get; private set; }
        public Vector2 Scale { get; private set; }
        public float ExplosionTimer { get; private set; }
        public bool Dead { get; private set; }

        private Texture2D explosionTexture;
        private Vector2 direction;
        private Vector2 destination;
        private Vector2 forwardDirection;
        private Vector2 rightDirection;
        private float rotation;
        private float rotationSpeed;
        private float rotationDestination;
        private float rotationAlpha;
        private float speed;
        private float movementTolerance;
        private float chaseRadiusSquared;
        private int health;
        private int baseHealth;
        private bool move;
        private bool chasingPlayer;
        private bool stopShoot;
        private bool _increaseScore;

        public virtual void Init(Vector2 position, Vector2 scale, float rotation = 0.0f, float speed = 100.0f, float chaseRadius = 450.0f, float rotationSpeed = 1.0f, float movementTolerance = 1.0f, int numberOfBullets = 5, int health = 1)
        {
            this.speed = speed;
            this.movementTolerance = movementTolerance;
            this.rotationSpeed = rotationSpeed;
            this.health = health;
            Position = position;
            this.rotation = rotation;
            Scale = scale;
            direction = Vector2.Zero;
            baseHealth = health;
            move = false;
            Dead = false;
            chasingPlayer = false;
            chaseRadiusSquared = chaseRadius * chaseRadius;
            ExplosionTimer = 0.5f;
            forwardDirection = new Vector2(0.0f, -1.0f);
            rightDirection = new Vector2(1.0f, 0.0f);
            Bullets = new IBullet[numberOfBullets];
            for (int i = 0; i < numberOfBullets; i++)
            {
                IBullet bullet = new Bullet();
                bullet.Init(position, new Vector2(0.2f, 0.2f));
                Bullets[i] = bullet;
            }
        }

        public virtual void LoadContent(Texture2D enemyTexture, Texture2D explosionTexture, Texture2D bulletTexture = null)
        {
            Texture = enemyTexture;
            this.explosionTexture = explosionTexture;
            foreach (IBullet bullet in Bullets)
            {
                bullet.LoadContent(bulletTexture, explosionTexture);
            }
        }

        public virtual void Update(float deltaTime)
        {
            foreach (IBullet bullet in Bullets)
            {
                bullet.Update(deltaTime, chasingPlayer, out stopShoot, Position, forwardDirection, out _increaseScore);
                if (stopShoot)
                {
                    chasingPlayer = false;
                }
            }

            if (Dead)
            {
                if (ExplosionTimer >= 0.0f)
                {
                    ExplosionTimer -= deltaTime;
                }
                return;
            }

            // Enemy movement
            if ((GAME.Player.Position - Position).LengthSquared() < chaseRadiusSquared && GAME.Player.Position != Position)
            {
                destination = GAME.Player.Position;
                direction = Position - destination;
                direction.Normalize();
                rotationDestination = (float)Math.Atan2(direction.Y * -forwardDirection.X - direction.X * -forwardDirection.Y, direction.X * -forwardDirection.X + direction.Y * -forwardDirection.Y);
                move = true;
                chasingPlayer = true;
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

                //Enemy-Player Collision
                if (CheckCollision(GAME.Player))
                {
                    GAME.Player.Die();
                    Dead = true;
                }
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (!Dead)
            {
                spriteBatch.Draw(Texture, Position, null, Color.White, rotation, new Vector2(Texture.Width * 0.5f, Texture.Height * 0.5f), Scale, SpriteEffects.None, 0.0f);
            }
            else if(ExplosionTimer >= 0.0f)
            {
                spriteBatch.Draw(explosionTexture, Position, null, Color.White, rotation, new Vector2(explosionTexture.Width * 0.5f, explosionTexture.Height * 0.5f), Scale, SpriteEffects.None, 0.0f);
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
            health = baseHealth;
            chasingPlayer = false;
            foreach (IBullet bullet in Bullets)
            {
                bullet.Respawn();
            }
        }

        public bool DecreaseHealth()
        {
            health--;
            if (health <= 0)
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
            chasingPlayer = false;
            stopShoot = true;
            health = baseHealth;
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
