using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniMonoGame
{
    public class Bullet : Entity 
    {
        public float speed;
        public bool move;
        private Player player;
        private Enemy[] enemies;
        private int screenWidth;
        private int screenHeight;
        public void Init(Vector2 position, Vector2 scale, int screenWidth, int screenHeight, float rotation = 0.0f, float speed = 400.0f, Enemy[] enemies = null, Player player = null)
        {
            InitEntity(position, scale, rotation);
            this.speed = speed;
            this.enemies = enemies;
            this.player = player;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            move = false;
        }

        public void Update(float deltaTime, bool shoot, out bool stopShoot, Vector2 initPosition, Vector2 initDirecton, out bool increaseScore)
        {
            UpdateEntity(deltaTime);
            increaseScore = false;
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
                        if (!enemy.dead)
                        {
                            Rectangle enemyBounds = new Rectangle((enemy.position - enemy.texture.Bounds.Size.ToVector2() * enemy.scale * 0.5f).ToPoint(), (enemy.texture.Bounds.Size.ToVector2() * enemy.scale).ToPoint());
                            if (enemyBounds.Intersects(new Rectangle((position - texture.Bounds.Size.ToVector2() * scale * 0.5f).ToPoint(), (texture.Bounds.Size.ToVector2() * scale).ToPoint())))
                            {
                                move = false;
                                enemy.dead = true;
                                enemy.move = false;
                                enemy.chasingPlayer = false;
                                enemy.stopShoot = true;
                                increaseScore = true;
                            }
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
                    foreach (Bullet bullet in player.bullets)
                    {
                        if (bullet.move)
                        {
                            Rectangle playerBulletBounds = new Rectangle((bullet.position - bullet.texture.Bounds.Size.ToVector2() * bullet.scale * 0.5f).ToPoint(), (bullet.texture.Bounds.Size.ToVector2() * bullet.scale).ToPoint());
                            if (playerBulletBounds.Intersects(new Rectangle((position - texture.Bounds.Size.ToVector2() * scale * 0.5f).ToPoint(), (texture.Bounds.Size.ToVector2() * scale).ToPoint())))
                            {
                                move = false;
                                bullet.move = false;
                            }
                        }
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

        public void Draw(SpriteBatch spriteBatch)
        {
            if (move)
            {
                spriteBatch.Draw(texture, position, null, Color.White, rotation, new Vector2(texture.Width * 0.5f, texture.Height * 0.5f), scale, SpriteEffects.None, 0.0f);
            }
        }
    }
}
