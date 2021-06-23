using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MiniMonoGame
{
    class Planet : Entity
    {
        public float speed;
        public bool move;
        public float rotationSpeed;
        public float movementTolerance;
        public float rotationDestination;
        public float rotationAlpha;
        public Vector2 destination;
        public Vector2 forwardDirection;
        public Vector2 rightDirection;
        public string texturePath;
        private int screenWidth;
        private int screenHeight;
        public void Init(Vector2 position, Vector2 scale, int screenWidth, int screenHeight, float rotation = 0.0f, float speed = 100.0f, float rotationSpeed = 1.0f, float movementTolerance = 1.0f)
        {
            InitEntity(position, scale, rotation);
            this.speed = speed;
            this.movementTolerance = movementTolerance;
            this.rotationSpeed = rotationSpeed;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            move = false;
            forwardDirection = new Vector2(0.0f, -1.0f);
            rightDirection = new Vector2(1.0f, 0.0f);
        }

        public void LoadContent(Texture2D texture)
        {
            this.texture = texture;
        }

        public void Update(float deltaTime)
        {
            UpdateEntity(deltaTime);

            // Planet movement
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
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, Color.White, rotation, new Vector2(texture.Width * 0.5f, texture.Height * 0.5f), scale, SpriteEffects.None, 0.0f);
        }
    }
}
