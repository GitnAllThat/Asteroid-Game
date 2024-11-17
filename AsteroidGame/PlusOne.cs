using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Shapes;
using Things;
using Global_Data;
using _2DLevelCreator;

namespace Asteroid_Game
{
    public class PlusOne
    {
        public static float shortenBy = 0.4f;
        public static float minLength = 0.01f;


        public Thing2D thing2D_PlusOne;
        public float life = 1;
        public Transform transform = new Transform();

        public PlusOne(Thing2D thing2D, Vector3 pos, float size)
        {
            thing2D_PlusOne = new Thing2D(thing2D, new List<Thing2D>());
            transform = new Transform(pos, 0, new Vector3(size));
        }

        public void Update(float timeDifference)
        {
            life -= timeDifference * 0.5f;
            transform.vPosition.Y += life * 0.01f;
            thing2D_PlusOne.Transparency -= timeDifference * 0.3f;
        }

        public void Draw(AsteroidGame game)
        {
            this.thing2D_PlusOne.Draw(game, this.transform, Matrix.CreateRotationZ(this.transform.zRotation));
        }

    }
}
