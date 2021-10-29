using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniMonoGame
{
    public class Bullet : IBullet
    {
        public Texture2D Texture { get; private set; }
        public Vector2 Position { get; private set; }
        public Vector2 Scale { get; private set; }
        public bool Move { get; private set; }

        private Texture2D explosionTexture;
        private Vector2 direction;
        private float speed;
        private float rotation;
        private float explosionTimer;
        private bool explode;
        private bool playerOwned;
        private int damage;

        public void Init(Vector2 position, Vector2 scale, bool playerOwned = false, float rotation = 0.0f, float speed = 400.0f, int damage = 1)
        {
            this.damage = damage;
            Position = position;
            this.rotation = rotation;
            Scale = scale;
            direction = Vector2.Zero;
            this.speed = speed;
            this.playerOwned = playerOwned;
            Move = false;
            explode = false;
            explosionTimer = 0.5f;
        }

        public void LoadContent(Texture2D bulletTexture, Texture2D explosionTexture)
        {
            Texture = bulletTexture;
            this.explosionTexture = explosionTexture;
        }

        public void Update(float deltaTime, bool shoot, out bool stopShoot, Vector2 initPosition, Vector2 initDirecton, out bool increaseScore)
        {
            increaseScore = false;

            if (explode)
            {
                if (explosionTimer > 0.0f)
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
            if (shoot && !Move)
            {
                Position = initPosition;
                direction = initDirecton;
                Move = true;
                stopShoot = true;
            }
            else
            {
                stopShoot = false;
            }
            if (Move)
            {
                Position += direction * speed * deltaTime;
                RestrictToScreen();

                if (playerOwned)
                {
                    foreach (Enemy enemy in GAME.Enemies)
                    {
                        if (!enemy.Dead && CheckCollision(enemy))
                        {
                            Explode();
                            increaseScore = enemy.DecreaseHealth(damage);
                            return;
                        }
                    }

                    foreach (IEnemy rocket in GAME.Boss.Rockets)
                    {
                        if (!rocket.Dead && CheckCollision(rocket))
                        {
                            Explode();
                            rocket.DecreaseHealth(damage);
                            return;
                        }
                    }
                    if (!GAME.BossEnemy.Dead && CheckCollision(GAME.BossEnemy))
                    {
                        Explode();
                        increaseScore = GAME.BossEnemy.DecreaseHealth(damage);
                        return;
                    }
                }
                else
                {
                    foreach (IPlayer player in GAME.Players)
                    {
                        if (!player.Dead && CheckCollision(player))
                        {
                            Explode();
                            player.DecreaseHealth(damage);
                            return;
                        }
                        foreach (IBullet bullet in player.Bullets)
                        {
                            if (bullet.Move && CheckCollision(bullet))
                            {
                                Explode();
                                bullet.Explode();
                                return;
                            }
                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Move)
            {
                spriteBatch.Draw(Texture, Position, null, Color.White, rotation, new Vector2(Texture.Width * 0.5f, Texture.Height * 0.5f), Scale, SpriteEffects.None, 0.0f);
            }
            else if (explode && explosionTimer >= 0.0f)
            {
                spriteBatch.Draw(explosionTexture, Position, null, Color.White, rotation, new Vector2(Texture.Width * 0.5f, Texture.Height * 0.5f), Scale, SpriteEffects.None, 0.0f);
            }
        }

        public void Explode()
        {
            Move = false;
            explode = true;
        }

        public void Respawn()
        {
            Move = false;
        }

        private void RestrictToScreen()
        {
            if (Position.X > GAME.ScreenWidth - (Texture.Width * 0.5f) * Scale.X ||
                Position.Y > GAME.ScreenHeight - (Texture.Height * 0.5f) * Scale.Y ||
                Position.X < (Texture.Width * 0.5f) * Scale.X || Position.Y < (Texture.Height * 0.5f) * Scale.Y)
            {
                Move = false;
            }
        }

        private Rectangle GetBounds(Vector2 position, Texture2D texture, Vector2 scale)
        {
            return new Rectangle((position - texture.Bounds.Size.ToVector2() * scale * 0.5f).ToPoint(), (texture.Bounds.Size.ToVector2() * scale).ToPoint());
        }

        private bool CheckCollision(IBullet bullet)
        {
            return GetBounds(bullet.Position, bullet.Texture, bullet.Scale).Intersects(GetBounds(Position, Texture, Scale));
        }

        private bool CheckCollision(IPlayer player)
        {
            return GetBounds(player.Position, player.Texture, player.Scale).Intersects(GetBounds(Position, Texture, Scale));
        }

        private bool CheckCollision(IEnemy enemy)
        {
            return GetBounds(enemy.Position, enemy.Texture, enemy.Scale).Intersects(GetBounds(Position, Texture, Scale));
        }
    }
}
