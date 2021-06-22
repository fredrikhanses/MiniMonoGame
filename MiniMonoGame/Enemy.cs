using Microsoft.Xna.Framework;
using System;

namespace MiniMonoGame
{
    public class Enemy : Entity
    {
        public float speed;
        public bool move;
        public bool dead;
        public float rotationSpeed;
        public float movementTolerance;
        public float rotationDestination;
        public float rotationAlpha;
        public Vector2 destination;
        public Vector2 forwardDirection;
        public Vector2 rightDirection;

        public void Init(Vector2 position, Vector2 scale, float rotation = 0.0f, float speed = 100.0f, float rotationSpeed = 1.0f, float movementTolerance = 1.0f)
        {
            InitEntity(position, scale, rotation);
            this.speed = speed;
            this.movementTolerance = movementTolerance;
            this.rotationSpeed = rotationSpeed;
            move = false;
            dead = false;
            forwardDirection = new Vector2(0.0f, -1.0f);
            rightDirection = new Vector2(1.0f, 0.0f);
        }

        public void Update(float deltaTime, int screenWidth, int screenHeight, Player player)
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
        }
    }
}
