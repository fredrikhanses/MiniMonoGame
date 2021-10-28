using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniMonoGame
{
    public interface IPlanet
    {
        string TexturePath { get; set; }
        void Init(Vector2 position, Vector2 scale, float rotation = 0.0f, float speed = 10.0f, float rotationSpeed = 0.01f, float movementTolerance = 1.0f);
        void LoadContent(Texture2D texture);
        void Update(float deltaTime);
        void Draw(SpriteBatch spriteBatch);
    }
}
