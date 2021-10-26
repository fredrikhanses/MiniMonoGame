using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniMonoGame
{
    public interface IPlayer
    {
        bool Dead { get; }
        IBullet[] Bullets { get; }
        int Score { get; }
        float ExplosionTimer { get; }
        Texture2D Texture { get; }
        Vector2 Position { get; }
        Vector2 Scale { get; }
        void Init(Vector2 position, Vector2 scale, float rotation = 0.0f, float speed = 100.0f, float rotationSpeed = 1.0f, float movementTolerance = 1.0f, int numberOfBullets = 100);
        void LoadContent(Texture2D playerTexture, Texture2D explosionTexture, Texture2D bulletTexture);
        void Update(float deltaTime);
        void Draw(SpriteBatch spriteBatch);
        void Respawn(Vector2 position);
        void Die();
    }
}
