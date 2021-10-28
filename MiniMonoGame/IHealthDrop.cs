using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniMonoGame
{
    public interface IHealthDrop
    {
        Texture2D Texture { get; }
        bool Used { get; }
        void Init(Vector2 position, Vector2 scale, float rotation = 0.0f, float rotationSpeed = 1.0f, int value = 1);
        void LoadContent(Texture2D texture);
        void Update(float deltaTime);
        void Draw(SpriteBatch spriteBatch);
        void Spawn(Vector2 position);
        void Respawn();
    }
}
