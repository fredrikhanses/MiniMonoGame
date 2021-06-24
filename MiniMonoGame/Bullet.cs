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
        private Boss boss;
        private int screenWidth;
        private int screenHeight;
        public Texture2D explosionTexture;
        public bool explode;
        public float explosionTimer;

        public void Init(Vector2 position, Vector2 scale, int screenWidth, int screenHeight, float rotation = 0.0f, float speed = 400.0f, Enemy[] enemies = null, Player player = null, Boss boss = null)
        {
            InitEntity(position, scale, rotation);
            this.speed = speed;
            this.enemies = enemies;
            this.boss = boss;
            this.player = player;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            move = false;
            explode = false;
            explosionTimer = 0.5f;
        }

        public void Update(float deltaTime, bool shoot, out bool stopShoot, Vector2 initPosition, Vector2 initDirecton, out bool increaseScore)
        {
            UpdateEntity(deltaTime);
            increaseScore = false;

            if (explode)
            {
                if (explosionTimer >= 0.0f)
                {
                    explosionTimer -= deltaTime;
                }
                else
                {
                    explode = false;
                }
                stopShoot = true;
                return;
            }

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
                                explode = true;
                                enemy.dead = true;
                                enemy.move = false;
                                enemy.chasingPlayer = false;
                                enemy.stopShoot = true;
                                increaseScore = true;
                                break;
                            }
                        }
                    }
                }

                if (boss != null)
                {
                    if (!boss.dead)
                    {
                        Rectangle bossBounds = new Rectangle((boss.position - boss.texture.Bounds.Size.ToVector2() * boss.scale * 0.5f).ToPoint(), (boss.texture.Bounds.Size.ToVector2() * boss.scale).ToPoint());
                        if (bossBounds.Intersects(new Rectangle((position - texture.Bounds.Size.ToVector2() * scale * 0.5f).ToPoint(), (texture.Bounds.Size.ToVector2() * scale).ToPoint())))
                        {
                            move = false;
                            explode = true;
                            boss.dead = true;
                            boss.move = false;
                            boss.chasingPlayer = false;
                            boss.stopShoot = true;
                            increaseScore = true;
                        }
                    }

                    if (!explode)
                    {
                        foreach (Rocket rocket in boss.rockets)
                        {
                            if (!rocket.dead)
                            {
                                Rectangle rocketBounds = new Rectangle((rocket.position - rocket.texture.Bounds.Size.ToVector2() * rocket.scale * 0.5f).ToPoint(), (rocket.texture.Bounds.Size.ToVector2() * rocket.scale).ToPoint());
                                if (rocketBounds.Intersects(new Rectangle((position - texture.Bounds.Size.ToVector2() * scale * 0.5f).ToPoint(), (texture.Bounds.Size.ToVector2() * scale).ToPoint())))
                                {
                                    move = false;
                                    explode = true;
                                    rocket.dead = true;
                                    rocket.move = false;
                                    rocket.chasingPlayer = false;
                                    rocket.stopShoot = true;
                                    increaseScore = true;
                                    break;
                                }
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
                        explode = true;
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
                                explode = true;
                                bullet.explode = true;
                                break;
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
            else if (explode && explosionTimer >= 0.0f)
            {
                spriteBatch.Draw(explosionTexture, position, null, Color.White, rotation, new Vector2(texture.Width * 0.5f, texture.Height * 0.5f), scale, SpriteEffects.None, 0.0f);
            }
        }
    }
}
