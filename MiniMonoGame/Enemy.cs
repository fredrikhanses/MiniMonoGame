using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MiniMonoGame
{
    public class Enemy : Entity
    {
        public float speed;
        public bool move;
        public bool dead;
        public bool chasingPlayer;
        public bool stopShoot;
        public float rotationSpeed;
        public float movementTolerance;
        public float rotationDestination;
        public float rotationAlpha;
        private float chaseRadiusSquared;
        public Vector2 destination;
        public Vector2 forwardDirection;
        public Vector2 rightDirection;
        public Bullet[] bullets;
        private Player player;
        private int screenWidth;
        private int screenHeight;

        public void Init(Vector2 position, Vector2 scale, Player player, int screenWidth, int screenHeight, float rotation = 0.0f, float speed = 100.0f, float chaseRadius = 450.0f, float rotationSpeed = 1.0f, float movementTolerance = 1.0f, int numberOfBullets = 5)
        {
            InitEntity(position, scale, rotation);
            this.speed = speed;
            this.movementTolerance = movementTolerance;
            this.rotationSpeed = rotationSpeed;
            this.player = player;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            move = false;
            dead = false;
            chasingPlayer = false;
            chaseRadiusSquared = chaseRadius * chaseRadius;
            forwardDirection = new Vector2(0.0f, -1.0f);
            rightDirection = new Vector2(1.0f, 0.0f);
            bullets = new Bullet[numberOfBullets];
            for (int i = 0; i < numberOfBullets; i++)
            {
                Bullet bullet = new Bullet();
                bullet.Init(position, new Vector2(0.2f, 0.2f), screenWidth, screenHeight, 0.0f, 400.0f, null, player);
                bullets[i] = bullet;
            }
        }

        public void LoadContent(Texture2D enemyTexture, Texture2D bulletTexture)
        {
            texture = enemyTexture;
            foreach (Bullet bullet in bullets)
            {
                bullet.texture = bulletTexture;
            }
        }

        public void Update(float deltaTime)
        {
            UpdateEntity(deltaTime);

            if (dead)
            {
                move = false;
                position = new Vector2(screenWidth * 0.5f, texture.Height);
                dead = false;
                return;
            }

            // Enemy movement
            if ((player.position - position).LengthSquared() < chaseRadiusSquared && player.position != position)
            {
                destination = player.position;
                direction = position - destination;
                direction.Normalize();
                rotationDestination = (float)Math.Atan2(direction.Y * -forwardDirection.X - direction.X * -forwardDirection.Y, direction.X * -forwardDirection.X + direction.Y * -forwardDirection.Y);
                move = true;
                chasingPlayer = true;
                rotationAlpha = 0.0f;
            }
            if (!move)
            {
                Random random = new Random();
                destination = new Vector2(random.Next(0, screenWidth), random.Next(0, screenHeight));
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

                //Enemy-Player Collision
                Rectangle playerBounds = new Rectangle((player.position - player.texture.Bounds.Size.ToVector2() * player.scale * 0.5f).ToPoint(), (player.texture.Bounds.Size.ToVector2() * player.scale).ToPoint());
                if (playerBounds.Intersects(new Rectangle((position - texture.Bounds.Size.ToVector2() * scale * 0.5f).ToPoint(), (texture.Bounds.Size.ToVector2() * scale).ToPoint())))
                {
                    player.dead = true;
                }
            }

            foreach (Bullet bullet in bullets)
            {
                bullet.Update(deltaTime, chasingPlayer, out stopShoot, position, forwardDirection);
                if (stopShoot)
                {
                    chasingPlayer = false;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, Color.White, rotation, new Vector2(texture.Width * 0.5f, texture.Height * 0.5f), scale, SpriteEffects.None, 0.0f);

            foreach (Bullet bullet in bullets)
            {
                bullet.Draw(spriteBatch);
            }
        }
    }
}
