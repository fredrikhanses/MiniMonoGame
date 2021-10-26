using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniMonoGame
{
    public interface IBullet
    {
        Texture2D Texture { get; }
        Vector2 Position { get; }
        Vector2 Scale { get; }
        bool Move { get; }
        void Init(Vector2 position, Vector2 scale, bool playerOwned = false, float rotation = 0.0f, float speed = 400.0f);
        void LoadContent(Texture2D bulletTexture, Texture2D explosionTexture);
        void Update(float deltaTime, bool shoot, out bool stopShoot, Vector2 initPosition, Vector2 initDirecton, out bool increaseScore);
        void Draw(SpriteBatch spriteBatch);
        void Explode();
        void Respawn();
    }
}
