
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Shapes;
using Asteroid_Game;

namespace _2d_Objects
{
    static partial class Collision_Methods
    {
        public static List<Circle> _contactPoints = new List<Circle>();
        public static List<Circle> _generalPoints = new List<Circle>();
        public static List<Line> _vectorsToDraw = new List<Line>();
        public static List<Line> _line = new List<Line>();


        public static Vector3 ScaleTool = Vector3.Zero;

        public static float angle1 = 0;
        public static float angle2 = 0;
        public static float angleBetween = 0;
        public static float angleEngine = 0;
        public static float angle = 0;
        public static float distance = 0;

        public static List<CollisionData> list_CD;



        public static void ClearDebug()
        {
            Collision_Methods._contactPoints.Clear();
            Collision_Methods._generalPoints.Clear();
            Collision_Methods._vectorsToDraw.Clear();
        }

        public static void DrawDebuggingTools(AsteroidGame xnaWindow)
        {
            /*
            for (int i = 0; i < list_CD.Count; ++i)
            {
                VectorHelper.DrawVertex(list_CD[i].vContactPointA + list_CD[i].rigidBody_A.GetCentriod_World(), xnaWindow, Color.Red);
                VectorHelper.DrawVertex(list_CD[i].vContactPointB + list_CD[i].rigidBody_B.GetCentriod_World(), xnaWindow, Color.Red);
            }
            */

            for (int i = 0; i < Collision_Methods._contactPoints.Count; i++) { Collision_Methods._contactPoints[i].Draw(xnaWindow);}
            for (int i = 0; i < Collision_Methods._generalPoints.Count; i++) { Collision_Methods._generalPoints[i].Draw(xnaWindow); }
            for (int i = 0; i < Collision_Methods._vectorsToDraw.Count; i++) { Collision_Methods._vectorsToDraw[i].Draw(xnaWindow); }
            for (int i = 0; i < Collision_Methods._line.Count; i++) { Collision_Methods._line[i].Draw(xnaWindow); }
        }
    }
}
