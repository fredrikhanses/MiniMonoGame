using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniMonoGame
{
    public interface IPlayer
    {
        IBullet[] Bullets { get; }
        Texture2D Texture { get; }
        Vector2 Position { get; }
        Vector2 Scale { get; }
        float ExplosionTimer { get; }
        int Score { get; }
        int CurrentHealth { get; }
        int BaseHealth { get; }
        float CurrentEnergy { get; }
        float BaseEnergy { get; }
        int Index { get; }
        bool Dead { get; }
        void Init(Vector2 position, Vector2 scale, float rotation = 0.0f, float speed = 200.0f, float rotationSpeed = 2.0f, float movementTolerance = 1.0f, int numberOfBullets = 100, int health = 10, float energy = 10.0f, int damage = 1);
        void LoadContent(Texture2D playerTexture, Texture2D explosionTexture, Texture2D bulletTexture, Texture2D shieldTexture);
        void Update(float deltaTime);
        void Draw(SpriteBatch spriteBatch);
        void Respawn(Vector2 position);
        void DecreaseHealth(int amount = 1);
        void IncreaseHealth(int amount = 1);
        void IncreaseEnergy(int amount = 1);
    }
}
