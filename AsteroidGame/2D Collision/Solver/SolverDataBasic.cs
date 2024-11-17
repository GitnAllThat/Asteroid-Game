using Microsoft.Xna.Framework;
using Asteroid_Game;


namespace _2d_Objects
{



    public class CollisionDataBasic
    {

        public Vector3  vNormal         { get; set; }
        public Vector3  vTangent        { get; set; }
        public Vector3  vContactPoint  { get; set; }
        public Vector3  vPerpCToCpA     { get; set; }

        public float    distance        { get; set; }
        public RainbowLine line { get; set; }
        public Vector3 point { get; set; }


        public CollisionDataBasic(float distance, Vector3 vNormal, Vector3 point1, RainbowLine line1, Vector3 contactpoint)
        {
            this.line = line1;
            this.point = point1;

            this.distance = distance;
            this.vNormal = vNormal;
            this.vTangent = Vector3.Cross(this.vNormal, new Vector3(0, 0, -1));

            this.vContactPoint = contactpoint;

            this.vPerpCToCpA = Vector3.Cross(vContactPoint, new Vector3(0, 0, -1));
        }
    }
}
