using Microsoft.Xna.Framework;
using Things;
using _2DLevelCreator;

namespace Asteroid_Game
{
    public class Backdrop
    {
        public Transform transform;
        Thing2D thing2D;


        public Backdrop(Vector3 position, Vector3 scale, Thing2D thing2D)
        {
            this.transform = new Transform(position, 0, scale);
            this.thing2D = thing2D;
        }

        public void Update(float timeDifference)
        {

        }

        public void SetTransparency(float transparency)
        {
            this.thing2D.Transparency = transparency;
        }


        public void Draw(AsteroidGame game)
        {
            this.thing2D.Draw(game, this.transform, Matrix.CreateRotationZ(this.transform.zRotation));
        }
    }
}
