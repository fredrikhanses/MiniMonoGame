using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniMonoGame
{
    public class Rocket : Enemy
    {
        public void InitRocket(Vector2 position, Vector2 scale, Player player, int screenWidth, int screenHeight, float rotation = 0.0f, float speed = 100.0f, float chaseRadius = 450.0f, float rotationSpeed = 1.0f, float movementTolerance = 1.0f, int numberOfBullets = 5)
        {
            Init(position, scale, player, screenWidth, screenHeight, rotation, speed, chaseRadius, rotationSpeed, movementTolerance, numberOfBullets);
        }

        public void LoadContentRocket(Texture2D rocketTexture, Texture2D bulletTexture, Texture2D explosionTexture)
        {
            LoadContent(rocketTexture, bulletTexture, explosionTexture);
        }

        public void UpdateRocket(float deltaTime)
        {
            Update(deltaTime);
        }

        public void DrawRocket(SpriteBatch spriteBatch)
        {
            Draw(spriteBatch);
        }

        public void RespawnRocket()
        {
            Respawn();
        }
    }
}
