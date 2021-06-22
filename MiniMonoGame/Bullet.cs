using Microsoft.Xna.Framework;

namespace MiniMonoGame
{
    public class Bullet : Entity 
    {
        public float speed;
        public bool move;

        public void Init(Vector2 position, Vector2 scale, float rotation = 0.0f, float speed = 400.0f)
        {
            InitEntity(position, scale, rotation);
            this.speed = speed;
            move = false;
        }

        public void Update(float deltaTime, int screenWidth, int screenHeight, bool shoot, out bool stopShoot, Vector2 initPosition, Vector2 initDirecton, Enemy[] enemies = null, Player player = null)
        {
            UpdateEntity(deltaTime);

            // Shoot Bullet
            if (shoot && !move)
            {
                position = initPosition;
                direction = initDirecton;
                move = true;
                stopShoot = true;
            }
            else
            {
                stopShoot = false;
            }
            if (move)
            {
                position.Y += speed * deltaTime * direction.Y;
                position.X += speed * deltaTime * direction.X;
                if (enemies != null)
                {
                    foreach (Enemy enemy in enemies)
                    {
                        Rectangle enemyBounds = new Rectangle((enemy.position - enemy.texture.Bounds.Size.ToVector2() * enemy.scale * 0.5f).ToPoint(), (enemy.texture.Bounds.Size.ToVector2() * enemy.scale).ToPoint());
                        if (enemyBounds.Intersects(new Rectangle((position - texture.Bounds.Size.ToVector2() * scale * 0.5f).ToPoint(), (texture.Bounds.Size.ToVector2() * scale).ToPoint())))
                        {
                            move = false;
                            enemy.dead = true;
                        }
                    }
                }

                if (player != null)
                {
                    Rectangle playerBounds = new Rectangle((player.position - player.texture.Bounds.Size.ToVector2() * player.scale * 0.5f).ToPoint(), (player.texture.Bounds.Size.ToVector2() * player.scale).ToPoint());
                    if (playerBounds.Intersects(new Rectangle((position - texture.Bounds.Size.ToVector2() * scale * 0.5f).ToPoint(), (texture.Bounds.Size.ToVector2() * scale).ToPoint())))
                    {
                        move = false;
                        player.dead = true;
                    }
                }

                if (position.X > screenWidth - (texture.Width * 0.5f) * scale.X ||
                    position.Y > screenHeight - (texture.Height * 0.5f) * scale.Y ||
                    position.X < (texture.Width * 0.5f) * scale.X || position.Y < (texture.Height * 0.5f) * scale.Y)
                {
                    move = false;
                }
            }
        }
    }
}
