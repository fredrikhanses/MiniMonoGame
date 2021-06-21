using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniMonoGame
{
    public class Entity
    {
        public Texture2D texture;
        public Vector2 position;
        public Vector2 scale;
        public Vector2 direction;

        public void InitEntity(Vector2 position, Vector2 scale)
        {
            this.position = position;
            this.scale = scale;
            direction = Vector2.Zero;
        }

        public void TextureEntity(Texture2D texture)
        {
            this.texture = texture;
        }

        public void UpdateEntity(float deltaTime)
        {

        }
    }
}
