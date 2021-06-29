using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniMonoGame
{
    public class Boss : Enemy
    {
        public Enemy[] rockets;

        public override void Init(Vector2 position, Vector2 scale, Player player, int screenWidth, int screenHeight, float rotation = 0.0f, float speed = 50.0f, float chaseRadius = 450.0f, float rotationSpeed = 0.0f, float movementTolerance = 2.0f, int numberOfRockets = 1, int health = 10)
        {
            base.Init(position, scale, player, screenWidth, screenHeight, rotation, speed, chaseRadius, rotationSpeed, movementTolerance, 0, health);
            rockets = new Enemy[numberOfRockets];
            for (int i = 0; i < numberOfRockets; i++)
            {
                Enemy rocket = new Enemy();
                rocket.Init(position, Vector2.One, player, screenWidth, screenHeight, 0.0f, 400.0f, 10000.0f, 10.0f, 5.0f, 0, 1);
                rockets[i] = rocket;
            }
        }

        public override void LoadContent(Texture2D bossTexture, Texture2D rocketTexture, Texture2D explosionTexture)
        {
            Texture2D rocketExplosionTexture = Game.Loader.Load<Texture2D>("explosion");
            foreach(Enemy rocket in rockets)
            {
                rocket.LoadContent(rocketTexture, rocketTexture, rocketExplosionTexture);
            }
            base.LoadContent(bossTexture, rocketTexture, explosionTexture);
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            foreach (Enemy rocket in rockets)
            {
                rocket.Update(deltaTime);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            foreach (Enemy rocket in rockets)
            {
                rocket.Draw(spriteBatch);
                if (!dead && rocket.explosionTimer <= 0.0f)
                {
                    rocket.Respawn();
                    rocket.position = position;
                }
            }
        }

        public override void Respawn()
        {
            base.Respawn();
            foreach (Enemy rocket in rockets)
            {
                rocket.Respawn();
                rocket.position = position;
            }
        }
    }
}
