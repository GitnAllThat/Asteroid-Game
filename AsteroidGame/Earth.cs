using Microsoft.Xna.Framework;
using Things;
using _2DLevelCreator;

namespace Asteroid_Game
{
    public class Earth
    {
        public Transform transform;
        Thing2D thing2D;
        public float impactRotation = 0;

        public Earth(Vector3 position, float rotation, Vector3 scale, Thing2D thing2D)
        {
            this.transform = new Transform(position, rotation, scale);
            this.thing2D = thing2D;
        }




        public void Draw(AsteroidGame game)
        {
            Transform t = new Transform(this.transform.vPosition, this.transform.zRotation, this.transform.vScale);
            this.thing2D.Draw(game, t, Matrix.CreateRotationZ(this.transform.zRotation));
        }
    }
}
