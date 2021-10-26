using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniMonoGame
{
    public interface IEnemy
    {
        string TexturePath { get; set; }
        IBullet[] Bullets { get; }
        Texture2D Texture { get; }
        Vector2 Position { get; }
        Vector2 Scale { get; }
        float ExplosionTimer { get; }
        bool Dead { get; }
        void Init(Vector2 position, Vector2 scale, float rotation = 0.0f, float speed = 100.0f, float chaseRadius = 450.0f, float rotationSpeed = 1.0f, float movementTolerance = 1.0f, int numberOfBullets = 5, int health = 1);
        void LoadContent(Texture2D enemyTexture, Texture2D explosionTexture, Texture2D bulletTexture = null);
        void Update(float deltaTime);
        void Draw(SpriteBatch spriteBatch);
        bool DecreaseHealth();
        void Respawn(Vector2 position);
    }
}
