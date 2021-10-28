using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniMonoGame
{
    public class Boss : Enemy, IBoss
    {
        public IEnemy[] Rockets { get; private set; }
        public bool Spinning { get; private set; }

        private float baseChaseRadiusSquared;
        private const float spinChaseRadiusSquared = 100000000.0f;
        private const float spinSpeed = 10.0f;

        public override void Init(Vector2 position, Vector2 scale, float rotation = 0.0f, float speed = 50.0f, float chaseRadius = 450.0f, float rotationSpeed = 0.0f, float movementTolerance = 2.0f, int numberOfRockets = 1, int health = 10, int bulletDamage = 1)
        {
            base.Init(position, scale, rotation, speed, chaseRadius, rotationSpeed, movementTolerance, 100, health, bulletDamage);
            Spinning = false;
            baseChaseRadiusSquared = ChaseRadiusSquared;
            Rockets = new IEnemy[numberOfRockets];
            for (int i = 0; i < numberOfRockets; i++)
            {
                IEnemy rocket = new Enemy();
                rocket.Init(position, Vector2.One, 0.0f, 400.0f, 10000.0f, 10.0f, 5.0f, 0, 1, bulletDamage);
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
            base.LoadContent(bossTexture, bossExplosionTexture, GAME.Loader.Load<Texture2D>("bullet"));
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            if (Spinning && !Dead)
            {
                Rotation += deltaTime * spinSpeed;
            }

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

            Spinning = false;
            Rotation = 0.0f;
            ChaseRadiusSquared = baseChaseRadiusSquared;

            foreach (IEnemy rocket in Rockets)
            {
                rocket.Respawn(position);
            }
        }

        public void StartToSpin()
        {
            Spinning = true;
            ChaseRadiusSquared = spinChaseRadiusSquared;
        }
    }
}
