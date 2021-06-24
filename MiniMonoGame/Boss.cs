using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniMonoGame
{
    public class Boss : Enemy
    {
        public Rocket[] rockets;

        public void InitBoss(Vector2 position, Vector2 scale, Player player, int screenWidth, int screenHeight, float rotation = 0.0f, float speed = 100.0f, float chaseRadius = 450.0f, float rotationSpeed = 1.0f, float movementTolerance = 1.0f, int numberOfBullets = 5, int numberOfRockets = 2)
        {
            Init(position, scale, player, screenWidth, screenHeight, rotation, speed, chaseRadius, rotationSpeed, movementTolerance, numberOfBullets);
            rockets = new Rocket[numberOfRockets];
            for (int i = 0; i < numberOfRockets; i++)
            {
                Rocket rocket = new Rocket();
                rocket.InitRocket(position, Vector2.One, player, screenWidth, screenHeight, 0.0f, 400.0f, 1000.0f, 10.0f, 5.0f, 0);
                rockets[i] = rocket;
            }
        }

        public void LoadContentBoss(Texture2D bossTexture, Texture2D bulletTexture, Texture2D rocketTexture, Texture2D explosionTexture)
        {
            foreach(Rocket rocket in rockets)
            {
                rocket.LoadContent(rocketTexture, bulletTexture, explosionTexture);
            }
            LoadContent(bossTexture, bulletTexture, explosionTexture);
        }

        public void UpdateBoss(float deltaTime)
        {
            Update(deltaTime);

            foreach (Rocket rocket in rockets)
            {
                rocket.UpdateRocket(deltaTime);
            }
        }

        public void DrawBoss(SpriteBatch spriteBatch)
        {
            Draw(spriteBatch);
            foreach (Rocket rocket in rockets)
            {
                rocket.DrawRocket(spriteBatch);
                if (!dead && rocket.explosionTimer <= 0.0f)
                {
                    rocket.RespawnRocket();
                }
            }
        }

        public void RespawnBoss()
        {
            Respawn();
            foreach (Rocket rocket in rockets)
            {
                rocket.RespawnRocket();
                rocket.position = position;
            }
        }
    }
}
