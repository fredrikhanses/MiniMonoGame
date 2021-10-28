using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MiniMonoGame
{
    public class Planet : IPlanet
    {
        public string TexturePath { get; set; }

        private Texture2D texture;
        private Vector2 position;
        private Vector2 scale;
        private Vector2 direction;
        private Vector2 destination;
        private Vector2 forwardDirection;
        private Vector2 rightDirection;
        private float rotationSpeed;
        private float rotationDestination;
        private float rotationAlpha;
        private float rotation;
        private float speed;
        private float movementTolerance;
        private bool move;

        public void Init(Vector2 position, Vector2 scale, float rotation = 0.0f, float speed = 10.0f, float rotationSpeed = 0.01f, float movementTolerance = 1.0f)
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
            direction = Vector2.Zero;
            this.speed = speed;
            this.movementTolerance = movementTolerance;
            this.rotationSpeed = rotationSpeed;
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
            // Planet movement
            if (!move)
            {
                Random random = new Random();
                destination = new Vector2(random.Next(0, GAME.ScreenWidth), random.Next(0, GAME.ScreenHeight));
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
                position -= direction * speed * deltaTime;
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
