using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniMonoGame
{
    public class Boss : Enemy
    {
        public Rocket[] rockets;
        bool increaseScore;

        public void InitBoss(Vector2 position, Vector2 scale, Player player, int screenWidth, int screenHeight, float rotation = 0.0f, float speed = 100.0f, float chaseRadius = 450.0f, float rotationSpeed = 1.0f, float movementTolerance = 1.0f, int numberOfBullets = 5, int numberOfRockets = 2)
        {
            Init(position, scale, player, screenWidth, screenHeight, rotation, speed, chaseRadius, rotationSpeed, movementTolerance, numberOfBullets);
            rockets = new Rocket[numberOfRockets];
            for (int i = 0; i < numberOfRockets; i++)
            {
                Rocket rocket = new Rocket();
                rocket.InitRocket(position, Vector2.One, screenWidth, screenHeight, 0.0f, 400.0f, null, player);
                rockets[i] = rocket;
            }
        }

        public void LoadContentBoss(Texture2D bossTexture, Texture2D bulletTexture, Texture2D rocketTexture, Texture2D explosionTexture)
        {
            foreach(Rocket rocket in rockets)
            {
                rocket.texture = rocketTexture;
                rocket.explosionTexture = explosionTexture;
            }
            LoadContent(bossTexture, bulletTexture, explosionTexture);
        }

        public void UpdateBoss(float deltaTime)
        {
            Update(deltaTime);

            Vector2 shootDirection = player.position - position;
            shootDirection.Normalize();
            foreach (Rocket rocket in rockets)
            {
                rocket.UpdateRocket(deltaTime, true, out stopShoot, position, shootDirection, out increaseScore);
            }
        }

        public void DrawBoss(SpriteBatch spriteBatch)
        {
            Draw(spriteBatch);
            foreach (Rocket rocket in rockets)
            {
                rocket.DrawRocket(spriteBatch);
            }
        }

        public void RespawnBoss()
        {
            Respawn();
            foreach (Rocket rocket in rockets)
            {
                rocket.move = false;
            }
        }
    }
}
