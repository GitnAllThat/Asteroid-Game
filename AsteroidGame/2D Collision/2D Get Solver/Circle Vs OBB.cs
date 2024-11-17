using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;


namespace _2d_Objects
{
    static partial class Collision_Methods
    {
        public static List<CollisionData> GetSolver_CircleVsOBB(BoundingCircle bc, OBB obb)
        {

            Vector3 dist = Vector3.Transform(bc.transform.vPosition - obb.transform.vPosition, Matrix.CreateRotationZ(-obb.transform.zRotation));


            Vector3 clamp; clamp.Z = 0;

            //Find xClamp
            if (dist.X > obb.HalfWidth)
            {
                clamp.X = obb.HalfWidth;
            }
            else if (dist.X < -obb.HalfWidth)
            {
                clamp.X = -obb.HalfWidth;
            }
            else
            {
                clamp.X = dist.X;
            }

            //Find yClamp
            if (dist.Y > obb.HalfHeight)
            {
                clamp.Y = obb.HalfHeight;
            }
            else if (dist.Y < -obb.HalfHeight)
            {
                clamp.Y = -obb.HalfHeight;
            }
            else
            {
                clamp.Y = dist.Y;
            }


            Vector3 finalNormal;
            float distance;
            if (dist == clamp)  // Circle is inside OBB
            {
                if (clamp.X > 0)
                    clamp.X = obb.HalfWidth;
                else
                    clamp.X = -obb.HalfWidth;

                if (clamp.Y > 0)
                    clamp.Y = obb.HalfHeight;
                else
                    clamp.Y = -obb.HalfHeight;



                if (Math.Abs(clamp.X) - Math.Abs(dist.X) < Math.Abs(clamp.Y) - Math.Abs(dist.Y))
                {
                    clamp.Y = dist.Y;
                    finalNormal = new Vector3(clamp.X, 0, 0);
                }
                else
                {
                    clamp.X = dist.X;
                    finalNormal = new Vector3(0, clamp.Y, 0);
                }
                distance = -(dist - clamp).Length();
            }
            else                // Circle is outside AABB
            {
                finalNormal = dist - clamp;
                distance = finalNormal.Length();
            }

            distance -= bc.Radius;
            finalNormal = -Vector3.Normalize(finalNormal);
            Matrix m = obb.rotMatrix;
            finalNormal = Vector3.Transform(finalNormal, m);
            clamp = Vector3.Transform(clamp, m);








            CollisionData collisionSolverData = new CollisionData(distance, finalNormal, bc, obb, 0000, (finalNormal * bc.Radius) - bc.CentroidRotated, clamp - obb.CentroidRotated);
 


            List<CollisionData> contactList = new List<CollisionData>(); contactList.Add(collisionSolverData);
            return contactList;
        }
    }
}


/*
    /////////////////
    //  Debugging  //
    /////////////////
    if (((Collision_Methods.lookingAt1 == Collision_Methods.Want1 && Collision_Methods.lookingAt2 == Collision_Methods.Want2) ||
        (Collision_Methods.lookingAt1 == Collision_Methods.Want2 && Collision_Methods.lookingAt2 == Collision_Methods.Want1)))
    {

        //Collision_Methods._generalPoints.Add(new Circle(bc.objectCenter, bc.Radius, 55, false, Color.Red));
        //Collision_Methods._generalPoints.Add(new Circle(bc.objectCenter + (finalNormal * bc.Radius), 0.1f, 55, true, Color.White));

        //Vector3[] vvv = obb.GetWorldVerts(Vector3.Zero);
        //for (int i = 0; i < vvv.Length; ++i)
        //    Collision_Methods._vectorsToDraw.Add(new Line(vvv[i], vvv[(i+1)%vvv.Length], Color.Red, 2));
        //Collision_Methods._generalPoints.Add(new Circle(obb.objectCenter + clamp, 0.1f, 55, true, Color.Green));


        Collision_Methods._generalPoints.Add(new Circle(collisionSolverData.vContactPointA + bc.transform.vPosition, 0.05f, 55, true, Color.Green));
        Collision_Methods._generalPoints.Add(new Circle(collisionSolverData.vContactPointB + obb.transform.vPosition, 0.05f, 55, true, Color.Red));
    }
    /////////////////
    //  Debugging  //
    /////////////////
*/