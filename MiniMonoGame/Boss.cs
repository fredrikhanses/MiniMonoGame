using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniMonoGame
{
    public class Boss : Enemy, IBoss
    {
        public IEnemy[] Rockets { get; private set; }

        public override void Init(Vector2 position, Vector2 scale, float rotation = 0.0f, float speed = 50.0f, float chaseRadius = 450.0f, float rotationSpeed = 0.0f, float movementTolerance = 2.0f, int numberOfRockets = 1, int health = 10)
        {
            base.Init(position, scale, rotation, speed, chaseRadius, rotationSpeed, movementTolerance, 0, health);
            Rockets = new IEnemy[numberOfRockets];
            for (int i = 0; i < numberOfRockets; i++)
            {
                IEnemy rocket = new Enemy();
                rocket.Init(position, Vector2.One, 0.0f, 400.0f, 10000.0f, 10.0f, 5.0f, 0, 1);
                Rockets[i] = rocket;
            }
        }

        public override void LoadContent(Texture2D bossTexture, Texture2D bossExplosionTexture, Texture2D rocketTexture)
        {
            Texture2D rocketExplosionTexture = GAME.Loader.Load<Texture2D>("explosion");
            foreach(IEnemy rocket in Rockets)
            {
                rocket.LoadContent(rocketTexture, rocketExplosionTexture);
            }
            base.LoadContent(bossTexture, bossExplosionTexture);
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            foreach (IEnemy rocket in Rockets)
            {
                rocket.Update(deltaTime);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            foreach (IEnemy rocket in Rockets)
            {
                rocket.Draw(spriteBatch);
                if (!Dead && rocket.ExplosionTimer <= 0.0f)
                {
                    rocket.Respawn(Position);
                }
            }
        }

        public override void Respawn(Vector2 position)
        {
            base.Respawn(position);

            foreach (IEnemy rocket in Rockets)
            {
                rocket.Respawn(position);
            }
        }
    }
}
