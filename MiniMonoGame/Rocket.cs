using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniMonoGame
{
    public class Rocket : Bullet
    {
        public void InitRocket(Vector2 position, Vector2 scale, int screenWidth, int screenHeight, float rotation = 0.0f, float speed = 400.0f, Enemy[] enemies = null, Player player = null)
        {
            Init(position, scale, screenWidth, screenHeight, rotation, speed, enemies, player);
        }

        public void UpdateRocket(float deltaTime, bool shoot, out bool stopShoot, Vector2 initPosition, Vector2 initDirecton, out bool increaseScore)
        {
            Update(deltaTime, shoot, out stopShoot, initPosition, initDirecton, out increaseScore);
        }

        public void DrawRocket(SpriteBatch spriteBatch)
        {
            Draw(spriteBatch);
        }
    }
}
