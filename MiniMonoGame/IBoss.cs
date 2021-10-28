using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniMonoGame
{
    public interface IBoss
    {
        public IEnemy[] Rockets { get; }
        bool Spinning { get; }
        void Init(Vector2 position, Vector2 scale, float rotation = 0.0f, float speed = 50.0f, float chaseRadius = 450.0f, float rotationSpeed = 0.0f, float movementTolerance = 2.0f, int numberOfRockets = 1, int health = 10, int bulletDamage = 1);
        void LoadContent(Texture2D bossTexture, Texture2D bossExplosionTexture, Texture2D rocketTexture);
        void Update(float deltaTime);
        void Draw(SpriteBatch spriteBatch);
        void Respawn(Vector2 position);
        void StartToSpin();
    }
}
