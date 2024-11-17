
using System.Collections.Generic;
using Microsoft.Xna.Framework;



namespace _2d_Objects
{
    static partial class Collision_Methods
    {

        public static List<CollisionData> GetSolver_CircleVsConvex(BoundingCircle bc, ConvexPolygon polygon)
        {
            /*
            if (((Collision_Methods.lookingAt1 == Collision_Methods.Want1 && Collision_Methods.lookingAt2 == Collision_Methods.Want2) ||
            (Collision_Methods.lookingAt1 == Collision_Methods.Want2 && Collision_Methods.lookingAt2 == Collision_Methods.Want1)))
            {
                Vector3[] vvv = polygon.GetWorldVerts(new Vector3(0,0,0));
                for (int i = 0; i < vvv.Length; ++i)
                {
                    Vector3 ve1 = vvv[i];                   // First vertex that makes up the edge.
                    Vector3 ve2 = vvv[(i + 1) % vvv.Length];    // Second vertex that makes up the edge.
                    Collision_Methods._vectorsToDraw.Add(new Line(ve1, ve2, Color.LightBlue, 2));
                }
                //Debugging
            }
            */


            Vector3 mdCenter = polygon.transform.vPosition - bc.transform.vPosition;            //Holds the center of the minkowski difference
            Vector3[] verts = polygon.GetWorldVerts(mdCenter);              //Gets the polygons world verts in relation to the circle being moved to the origin.
            Vector3 mdCenter2 = polygon.transform.vPosition - bc.transform.vPosition;

            Vector3 edgeVert1, edgeVert2, reverseEdgeNormal, finalNormal;
            Vector3 worldFaceNormal = polygon.FaceNormal;

            Vector3 closestPoint = new Vector3(0, 0, 0), projectedPoint;
            float closestDistance = float.MaxValue, currentDistance;
            bool isNegDist = false;


            for (int i = 0, vertNo = polygon.verts.Length; i < vertNo; ++i)
            {
                edgeVert1 = verts[i];                   // First vertex that makes up the edge.
                edgeVert2 = verts[(i + 1) % vertNo];    // Second vertex that makes up the edge.
                reverseEdgeNormal = Vector3.Cross(worldFaceNormal, edgeVert1 - edgeVert2);      //Gets the edges reverse edge normal.
                
                //Collision_Methods._vectorsToDraw.Add(new Line(edgeVert1, edgeVert2, Color.Red, 2));

                if (Vector3.Dot(mdCenter2, reverseEdgeNormal) >= 0)                              // Removes Backfacing Edges.
                {
                    projectedPoint = PointOnEdgeClosestToOriginClamped(edgeVert1, edgeVert2);   //Finds the location on the edge that is closest the origin.(Clamps it if it is out of bounds)
                    //Collision_Methods._generalPoints.Add(new Circle(projectedPoint, 0.05f, 55, true, Color.Red));
                    currentDistance = projectedPoint.LengthSquared();                           //Gets its distance
                    if (currentDistance < closestDistance)                                      //Checks to see if it is shorter than the currently found distance
                    {
                        closestPoint = projectedPoint;          // Stores new closest point
                        closestDistance = currentDistance;      // Updates the closest distance
                        if (Vector3.Dot(mdCenter2, projectedPoint) > 0) isNegDist = false; else isNegDist = true;
                    }
                }
            }

            //Collision_Methods._generalPoints.Add(new Circle(closestPoint, 0.05f, 55, true, Color.White));
            if (isNegDist)
            {
                closestDistance = -closestPoint.Length();    // Gets the true distance. (Was holding lengthsquared instead of length).
                closestDistance -= bc.Radius;
                finalNormal = -Vector3.Normalize(closestPoint);
            }
            else
            {
                closestDistance = closestPoint.Length();    // Gets the true distance. (Was holding lengthsquared instead of length).
                closestDistance -= bc.Radius;
                finalNormal = Vector3.Normalize(closestPoint);
            }




            CollisionData collisionSolverData = new CollisionData(closestDistance, finalNormal, bc, polygon, 0000, (finalNormal * bc.Radius) - bc.CentroidRotated, (closestPoint - mdCenter) - polygon.CentroidRotated);




            //Collision_Methods._generalPoints.Add(new Circle(Vector3.Zero, bc.Radius, 55, false, Color.Green));
            //Collision_Methods._generalPoints.Add(new Circle(collisionSolverData.vContactPointA + bc.transform.vPosition, 0.02f, 55, true, Color.Blue));
            //Collision_Methods._generalPoints.Add(new Circle(collisionSolverData.vContactPointB + polygon.transform.vPosition, 0.02f, 55, true, Color.Yellow));


            List<CollisionData> contactList = new List<CollisionData>(); contactList.Add(collisionSolverData);
            return contactList;
        }

    }
}

/*
    if (((Collision_Methods._lookingAt1 == Collision_Methods._Want1 && Collision_Methods._lookingAt2 == Collision_Methods._Want2) ||
    (Collision_Methods._lookingAt1 == Collision_Methods._Want2 && Collision_Methods._lookingAt2 == Collision_Methods._Want1)))
    {
        Collision_Methods._generalPoints.Add(new Circle(collisionSolverData.vContactPointA, 0.1f, 55, true, Color.Orange));
        Collision_Methods._generalPoints.Add(new Circle(collisionSolverData.vContactPointB, 0.1f, 55, true, Color.Red));
        Collision_Methods._generalPoints.Add(new Circle(new Vector3(0,0,0), bc.Radius, 55, false, Color.LightBlue));


        Vector3[] vvv = polygon.GetWorldVerts(Vector3.Zero);
        for (int i = 0; i < vvv.Length; ++i)
            Collision_Methods._vectorsToDraw.Add(new Line(vvv[i], vvv[(i + 1) % vvv.Length], Color.Red, 2));


        Collision_Methods._vectorsToDraw.Add(new Line(bc.transform.vPosition + (finalNormal * bc.Radius), bc.transform.vPosition + (finalNormal * bc.Radius) + finalNormal, Color.LightBlue, 2));                                                   //Debugging
    }
*/