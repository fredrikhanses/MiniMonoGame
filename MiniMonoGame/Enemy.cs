using Microsoft.Xna.Framework;
using System;

namespace MiniMonoGame
{
    public class Enemy : Entity
    {
        public float speed;
        public int move;

        public void Init(Vector2 position, Vector2 scale, float rotation = 0.0f, float speed = 50.0f)
        {
            InitEntity(position, scale, rotation);
            this.speed = speed;
            move = -1;
        }

        public void Update(float deltaTime, int screenWidth, int screenHeight, int totalGameTime)
        {
            UpdateEntity(deltaTime);

            // Enemy movement
            if (totalGameTime % 4 == 0)
            {
                Random random = new Random();
                move = random.Next(0, 8);
            }
            if (move == 0 || move == 1 || move == 2)
            {
                position.Y -= speed * deltaTime;
            }
            if (move == 3 || move == 4 || move == 5)
            {
                position.Y += speed * deltaTime;
            }
            if (move == 0 || move == 3 || move == 6)
            {
                position.X -= speed * deltaTime;
            }
            if (move == 1 || move == 4 || move == 7)
            {
                position.X += speed * deltaTime;
            }

            if (position.X > screenWidth - texture.Width / 2)
            {
                position.X = screenWidth - texture.Width / 2;
            }
            else if (position.X < texture.Width / 2)
            {
                position.X = texture.Width / 2;
            }
            if (position.Y > screenHeight - texture.Height / 2)
            {
                position.Y = screenHeight - texture.Height / 2;
            }
            else if (position.Y < texture.Height / 2)
            {
                position.Y = texture.Height / 2;
            }
        }
    }
}
