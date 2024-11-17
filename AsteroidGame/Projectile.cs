using Microsoft.Xna.Framework;
using Things;
using _2DLevelCreator;

namespace Asteroid_Game
{
    public class Projectile
    {
        public Transform transform;
        public Vector3 velocity = Vector3.Zero;
        float rotationSpeed = 1;
        Thing2D thing2D;


        public Projectile(Vector3 position, float size, Vector3 velocity, Thing2D thing2D)
        {
            this.transform = new Transform(position, 0, new Vector3(size, size, 1));
            this.velocity = velocity;
            this.thing2D = thing2D;
        }

        public void Update(float timeDifference)
        {
            this.transform.vPosition += (this.velocity * timeDifference);
            this.UpdateRotation(timeDifference);
        }

        public void UpdateRotation(float timeDifference)
        {
            this.transform.zRotation += (this.rotationSpeed * timeDifference);
        }


        public void Draw(AsteroidGame game)
        {
            this.thing2D.Draw(game, this.transform, Matrix.CreateRotationZ(this.transform.zRotation));
        }
    }
}
