using System;
using Microsoft.Xna.Framework;

using _2DLevelCreator;

namespace _2d_Objects
{


    public  static partial class Collision_Methods
    {
        






        public static Vector3 CalculateCentroid(Vector3[] verts)
        {

            Vector3 midpoint1, midpoint2;
            LineEquation line1, line2;
            Vector3 polyCentroid = new Vector3(0, 0, 0);
            float a = 0, b = 0, h = 0;



            int numTriangles = verts.Length - 2;
            Vector3[] triangleCentroid = new Vector3[numTriangles];
            float[] triangleArea = new float[numTriangles];

            float polygonArea = 0;

            for (int i = 1; i <= numTriangles; ++i)
            {

                midpoint1 = verts[0] - ((verts[0] - verts[i]) * 0.5f);
                line1 = new LineEquation(midpoint1, verts[i + 1]);
                midpoint2 = verts[i] - ((verts[i] - verts[i + 1]) * 0.5f);
                line2 = new LineEquation(midpoint2, verts[0]);
                LineEquation.LineIntersectionPoint(line1, line2, ref triangleCentroid[i - 1]);


                Collision_Methods.GetTriangleHeightBaseandA(verts[0], verts[i], verts[i + 1], ref a, ref b, ref h);

                triangleArea[i - 1] = 0.5f * b * h;
                polygonArea += triangleArea[i - 1];




                polyCentroid += triangleCentroid[i - 1] * triangleArea[i - 1];

            }
            polyCentroid /= polygonArea;


            return polyCentroid;
        }



        public static void GetTriangleHeightBaseandA(Vector3 vert1, Vector3 vert2, Vector3 vert3, ref float a, ref float b, ref float h)
        {
            Vector3 baseVert1, baseVert2, edgeVert1, edgeVert2;
            if ((vert1 - vert2).LengthSquared() > (vert2 - vert3).LengthSquared())
            {
                if ((vert1 - vert2).LengthSquared() > (vert3 - vert1).LengthSquared())
                {
                    baseVert1 = vert1;
                    baseVert2 = vert2;
                    edgeVert1 = vert2;
                    edgeVert2 = vert3;
                }
                else
                {
                    baseVert1 = vert3;
                    baseVert2 = vert1;
                    edgeVert1 = vert1;
                    edgeVert2 = vert2;
                }
            }
            else
            {
                if ((vert2 - vert3).LengthSquared() > (vert3 - vert1).LengthSquared())
                {
                    baseVert1 = vert2;
                    baseVert2 = vert3;
                    edgeVert1 = vert3;
                    edgeVert2 = vert1;
                }
                else
                {
                    baseVert1 = vert3;
                    baseVert2 = vert1;
                    edgeVert1 = vert1;
                    edgeVert2 = vert2;
                }
            }

            b = (baseVert1 - baseVert2).Length();

            Vector3 edge = (baseVert1 - baseVert2); edge.Normalize();
            float dot = Vector3.Dot(edgeVert1 - edgeVert2, edge);

            a = Math.Abs(dot);

            h = ((baseVert2 + ((baseVert2 - baseVert1) * (dot / b))) - edgeVert2).Length();

        }


        /// <summary>
        /// Builds the contact points for the two colliding objects. 
        /// Need to provide the found edge along with the related sv.
        /// For the contact points to be found, the second edge will need to be found.
        /// This will be the edge connected to the sv. The sv has 2 possible edges to choose from, so 2 extra verts will be sent in.
        /// The program will then find the correct edge by using the edge whos normal is most opposite the known edge
        /// </summary>
        /// <param name="A_edgeVert1"> First Vert from a known edge </param>
        /// <param name="A_edgeVert2"> Second Vert from a known edge </param>
        /// <param name="B_SvVert"> Definate Vert from unknown edge </param>
        /// <param name="B_edgeVert1"> Possible Vert for unknown edge </param>
        /// <param name="B_edgeVert2"> Possible Vert for unknown edge </param>
        /// <returns></returns>
        public static Vector3[] GetContactSet(Vector3 A_edgeVert1, Vector3 A_edgeVert2, Vector3 B_SvVert, Vector3 B_edgeVert1, Vector3 B_edgeVert2, bool foundInA, ref bool svVert2Index)
        {
            Vector3[] contactSet = new Vector3[4];


            Vector3 edgeNormal1 = Vector3.Cross(new Vector3(0, 0, -1), A_edgeVert1 - A_edgeVert2);
            Vector3 edgeNormal2 = Vector3.Cross(new Vector3(0, 0, -1), B_edgeVert1 - B_SvVert);
            Vector3 edgeNormal3 = Vector3.Cross(new Vector3(0, 0, -1), B_SvVert - B_edgeVert2);

            if (Vector3.Dot(edgeNormal1, edgeNormal2) < Vector3.Dot(edgeNormal1, edgeNormal3))
                B_edgeVert2 = B_SvVert;
            else
            {
                B_edgeVert1 = B_SvVert;
                svVert2Index = true;
            }

            if (foundInA)
            {
                contactSet[0] = PointOnEdgeClosestToPointClamped(A_edgeVert1, A_edgeVert2, B_edgeVert1);
                contactSet[1] = PointOnEdgeClosestToPointClamped(B_edgeVert1, B_edgeVert2, A_edgeVert2);
                contactSet[2] = PointOnEdgeClosestToPointClamped(A_edgeVert1, A_edgeVert2, B_edgeVert2);
                contactSet[3] = PointOnEdgeClosestToPointClamped(B_edgeVert1, B_edgeVert2, A_edgeVert1);
            }
            else
            {
                contactSet[0] = PointOnEdgeClosestToPointClamped(B_edgeVert1, B_edgeVert2, A_edgeVert2);
                contactSet[1] = PointOnEdgeClosestToPointClamped(A_edgeVert1, A_edgeVert2, B_edgeVert1);
                contactSet[2] = PointOnEdgeClosestToPointClamped(B_edgeVert1, B_edgeVert2, A_edgeVert1);
                contactSet[3] = PointOnEdgeClosestToPointClamped(A_edgeVert1, A_edgeVert2, B_edgeVert2);
            }




            return contactSet;
        }
        /*
            if (((Collision_Methods.lookingAt1 == Collision_Methods.Want1 && Collision_Methods.lookingAt2 == Collision_Methods.Want2) ||
            (Collision_Methods.lookingAt1 == Collision_Methods.Want2 && Collision_Methods.lookingAt2 == Collision_Methods.Want1)))
            {
                Collision_Methods._vectorsToDraw.Add(new Line(A_edgeVert1, A_edgeVert2, Color.Yellow, 2));
                Collision_Methods._vectorsToDraw.Add(new Line(B_edgeVert1, B_edgeVert2, Color.Red, 2));
                Collision_Methods._vectorsToDraw.Add(new Line(contactSet[0], contactSet[1], Color.Red, 2));
                Collision_Methods._vectorsToDraw.Add(new Line(contactSet[2], contactSet[3], Color.Green, 2));
                Collision_Methods._generalPoints.Add(new Circle(contactSet[0], 0.1f, 50, true, Color.Red));                              //Debugging
                Collision_Methods._generalPoints.Add(new Circle(contactSet[1], 0.1f, 50, true, Color.Red));                              //Debugging
                Collision_Methods._generalPoints.Add(new Circle(contactSet[2], 0.1f, 50, true, Color.Green));                              //Debugging
                Collision_Methods._generalPoints.Add(new Circle(contactSet[3], 0.1f, 50, true, Color.Green));                              //Debugging
            }
        */







        private static Vector3 PointOnEdgeClosestToPointClamped(Vector3 edgeVert1, Vector3 edgeVert2, Vector3 point)
        {
            Vector3 e1 = edgeVert1 - point;
            Vector3 e2 = edgeVert2 - point; 
            
            /*
             Can make a function at a later date that does this by simple using the below PointOnEdgeClosestToOrigin function but first taking the edge and 
             * subtracting the points location.  This puts the point at the origin and the edge relative to that.
             */
            return PointOnEdgeClosestToOriginClamped(e1, e2) + point;
        }



        public static bool IntersectLineVsCircle(LineEquation line, float radiusSquared, ref Vector3 v1, ref Vector3 v2)
        {
            //NOTE: This circle is at the origin.
            //Solve Quadratic Equation of the Circle (x^2 + y^2 = radius) using the Line Equation of the edge. "http://goo.gl/YQoKs" and "http://goo.gl/BQiOg"
            //First to solve: b^2 -4ac
            if (line.inTermsOf_X)
            {
                if (line.multiplier != 0)
                {
                    float a = 1 + (line.multiplier * line.multiplier);
                    float b = (line.multiplier * line.constant) * 2;
                    float c = (line.constant * line.constant) - radiusSquared;
                    float _b2_4ac = (float)Math.Sqrt((b * b) - (4 * a * c));
                    if (float.IsNaN(_b2_4ac))   return false;
                        
                    //Now to find the 2 results
                    v1 = new Vector3(-(b + _b2_4ac) / (2 * a), 0, 0); v1.Y = (line.multiplier * v1.X) + line.constant;
                    v2 = new Vector3(-(b - _b2_4ac) / (2 * a), 0, 0); v2.Y = (line.multiplier * v2.X) + line.constant;
                }
                else
                {
                    float r = (radiusSquared - (line.constant * line.constant));
                    if (r < 0) return false;
                    else r = (float)Math.Sqrt(r);
                    v1 = new Vector3(r, line.constant, 0);
                    v2 = new Vector3(-r, line.constant, 0);
                }
            }
            else
            {
                float r = (radiusSquared - (line.constant * line.constant));
                if (r < 0) return false;
                else r = (float)Math.Sqrt(r);
                v1 = new Vector3(line.constant, r, 0);
                v2 = new Vector3(line.constant, -r, 0);
            }
            return true;
            
        }





        static float f_180_DivBy_Pi = (180 / 3.14159265f);
        static float f_360_DivBy_Pi = (360 / 3.14159265f);
        public static float GetAngle(Vector3 vector)
        {
            return (float)Math.Atan2(vector.Y, vector.X) * f_180_DivBy_Pi;
        }

        /// <summary>
        /// Pass in 2 Vectors and you shall recieve an angle between the two
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static float GetAngle(Vector3 v1, Vector3 v2)
        {
            v1.Normalize();
            v2.Normalize();
            Vector3 v3 = new Vector3(0, 1, 0);

            double ydif = v1.Y - v3.Y;
            double xdif = v1.X - v3.X;
            float atan = (float)Math.Atan2(ydif, xdif);
            float firstAngle = -(atan * f_360_DivBy_Pi);

            ydif = v2.Y - v3.Y;
            xdif = v2.X - v3.X;
            atan = (float)Math.Atan2(ydif, xdif);
            float secondAngle = -(atan * f_360_DivBy_Pi);

            float finalAngle = secondAngle - firstAngle;
            if (finalAngle < 0) finalAngle += 360;

            return finalAngle;
        }

        public static float GetSmallestAngle(float angle1, float angle2)
        {
            float angle = (angle1 - angle2) % 360;

            if (angle < -180) return angle + 360;
            else if (angle > 180) return angle -360;
            else return angle;
        }




        /// <summary>
        /// Takes the distance to travel and the speed and works out how long it will take to run that distance.
        /// WARNING : Do not pass in a velocity of 0 for BOTH X and Y. Either or is fine.
        /// Note: Does not need to follow same path. ie the velocity does not need to be the exacy fraction of the distance
        /// </summary>
        /// <param name="distanceToTravel"></param>
        /// <param name="travelVelocity"></param>
        /// <returns></returns>
        private static float GetTravelTime(Vector3 distanceToTravel, Vector3 travelVelocity)
        {
            if (distanceToTravel.X == 0 && distanceToTravel.Y == 0) return 0;           //No Distance to travel.
            if (travelVelocity.X == 0 && travelVelocity.Y == 0) return float.MaxValue;  //No movement. Therefore it wont travel the distance.

            float xTime = float.MaxValue, yTime = float.MaxValue;

            if (travelVelocity.X != 0) xTime = distanceToTravel.X / travelVelocity.X; else if (distanceToTravel.X <= 0) xTime *= -1;

            if (travelVelocity.Y != 0) yTime = distanceToTravel.Y / travelVelocity.Y; else if( distanceToTravel.Y <= 0 ) yTime *= -1;


            if (xTime == 0) return yTime;
            if (yTime == 0) return xTime;

            if (xTime <= 0 &&  travelVelocity.X <= 0) return xTime;
            if (yTime <= 0 &&  travelVelocity.Y <=0 ) return yTime;

            //if (xTime < 0 && yTime < 0) { if (xTime < yTime) return xTime; else return yTime; }

            if (xTime > yTime) return xTime; else return yTime;

        }



   




        public static bool ClampePointToEdge(Vector3 edgeVert1, Vector3 edgeVert2, ref Vector3 point)
        {
            Vector3 temp = point;
            //Closest point to the origin may not reside on the edge so remaider code clamps the point to the edge.
            //Clamp X position to the edge if projectedPoint's X is not on the edge
            if (edgeVert1.X > edgeVert2.X)                                              // EdgeVert 1 has the largest x value.
            {
                if (point.X > edgeVert1.X) 
                    point.X = edgeVert1.X;         //Is the projected point's X value larger than edgevert 1 and need clamping?
                else                                                                    
                if (point.X < edgeVert2.X) 
                    point.X = edgeVert2.X;         //If it doesent then is it smaller than edgeVert 2 and need clamping?
            }
            else                                                                        // EdgeVert 2 has the largest x value.
                if (point.X > edgeVert2.X) 
                    point.X = edgeVert2.X;         //Is the projected point's X value larger than edgevert 2 and need clamping?
                else if (point.X < edgeVert1.X) 
                    point.X = edgeVert1.X;    //If it doesent then is it smaller than edgeVert 1 and need clamping?





            //Clamp Y position to the edge if projectedPoint's Y is not on the edge
            if (edgeVert1.Y > edgeVert2.Y)                                              // EdgeVert 1 has the largest y value.
            {
                if (point.Y > edgeVert1.Y) 
                    point.Y = edgeVert1.Y;         //Is the projected point's Y value larger than edgevert 1 and need clamping?
                else                                                                
                if (point.Y < edgeVert2.Y) 
                    point.Y = edgeVert2.Y;         //If it doesent then is it smaller than edgeVert 2 and need clamping?
            }
            else                                                                        // EdgeVert 2 has the largest y value.
                if (point.Y > edgeVert2.Y) 
                    point.Y = edgeVert2.Y;         //Is the projected point's Y value larger than edgevert 2 and need clamping?
                else if (point.Y < edgeVert1.Y) 
                    point.Y = edgeVert1.Y;    //If it doesent then is it smaller than edgeVert 1 and need clamping?
            
            if(point == temp)   return false;
            return true;
        }






        /// <summary>
        /// This takes in an edge and projects the origin onto it. The point will be clamped within the bounds
        /// of the edge.
        /// </summary>
        /// <param name="edgeVert1"> First Vertex </param>
        /// <param name="edgeVert2"> Second Vertex </param>
        /// <returns></returns>
        public static Vector3 PointOnEdgeClosestToOriginClamped(Vector3 edgeVert1, Vector3 edgeVert2)
        {
            if (edgeVert1 == edgeVert2) //(WARNING)(CHANGE) This if statement shouldnt exist. program should not allow an edge of zero length! (Keep breakpoint to remind me if program fails here)
                return edgeVert1;   //(CHANGE) Might need this if 2 vertices are on top of each other.(Tbh make a bit of code to remove 0 length edges)

            Vector3 unitOfEdge = edgeVert1 - edgeVert2; unitOfEdge.Normalize();     //Gets the normalised direction between the 2 points
            
            float dot = Vector3.Dot(edgeVert1, unitOfEdge);

            Vector3 projectedPoint = edgeVert1 - (dot * unitOfEdge);                //Finds the closest point to the origin.


            //Closest point to the origin may not reside on the edge so remaider code clamps the point to the edge.
            //Clamp X position to the edge if projectedPoint's X is not on the edge
            if (edgeVert1.X > edgeVert2.X)                                              // EdgeVert 1 has the largest x value.
            {
                if (projectedPoint.X > edgeVert1.X) 
                    projectedPoint.X = edgeVert1.X;         //Is the projected point's X value larger than edgevert 1 and need clamping?
                else                                                                    
                if (projectedPoint.X < edgeVert2.X) 
                    projectedPoint.X = edgeVert2.X;         //If it doesent then is it smaller than edgeVert 2 and need clamping?
            }
            else                                                                        // EdgeVert 2 has the largest x value.
                if (projectedPoint.X > edgeVert2.X) 
                    projectedPoint.X = edgeVert2.X;         //Is the projected point's X value larger than edgevert 2 and need clamping?
                else if (projectedPoint.X < edgeVert1.X) 
                    projectedPoint.X = edgeVert1.X;    //If it doesent then is it smaller than edgeVert 1 and need clamping?





            //Clamp Y position to the edge if projectedPoint's Y is not on the edge
            if (edgeVert1.Y > edgeVert2.Y)                                              // EdgeVert 1 has the largest y value.
            {
                if (projectedPoint.Y > edgeVert1.Y) 
                    projectedPoint.Y = edgeVert1.Y;         //Is the projected point's Y value larger than edgevert 1 and need clamping?
                else                                                                
                if (projectedPoint.Y < edgeVert2.Y) 
                    projectedPoint.Y = edgeVert2.Y;         //If it doesent then is it smaller than edgeVert 2 and need clamping?
            }
            else                                                                        // EdgeVert 2 has the largest y value.
                if (projectedPoint.Y > edgeVert2.Y) 
                    projectedPoint.Y = edgeVert2.Y;         //Is the projected point's Y value larger than edgevert 2 and need clamping?
                else if (projectedPoint.Y < edgeVert1.Y) 
                    projectedPoint.Y = edgeVert1.Y;    //If it doesent then is it smaller than edgeVert 1 and need clamping?

            


            return projectedPoint;
        }

        /// <summary>
        /// This takes in an edge and extends it to infinity. The origin will then be projected onto this line.
        /// 
        /// </summary>
        /// <param name="edgeVert1"> First Vertex </param>
        /// <param name="edgeVert2"> Second Vertex </param>
        /// <returns></returns>
        private static Vector3 PointOnEdgeClosestToOrigin(Vector3 edgeVert1, Vector3 edgeVert2) //Needs optimising
        {
            Vector3 unitOfEdge = edgeVert1 - edgeVert2; unitOfEdge.Normalize();     //Gets the normalised direction between the 2 points
            
            float dot = Vector3.Dot(-edgeVert1, unitOfEdge);

            return (edgeVert1 + (dot * unitOfEdge));  // Returns the projected point.
        }
        /// <summary>
        /// 
        /// This function will run through all of the vertices and will find the vertex that is most in the direction of the reverseEdgeNormal.
        /// Basically an edge of one object needs to find the vertex of another objects which is the most in the direction of the reverse of the edges normal.
        /// 
        /// Note: Can find multiple vertices.
        /// 
        /// </summary>
        /// <param name="vertices"> All the vertices from one object </param>
        /// <param name="reverseEdgeNormal"> The reverse normal of the edge that requires a support vertex</param>
        /// <returns> Returns one or more support vertices</returns>
        public static int[] GetSupportVertices(ref Vector3[] vertices, Vector3 reverseEdgeNormal)
        {
            if (vertices.Length <= 0) return null;                  // Dont bother doing any checks if no verts are present.
            int[] supportVertices = new int[vertices.Length];       // Array to hold the element of the support Vertices that will be found.

            double dot; int svCount = 1, svAdded = 1;

            double fMostInDir = Vector3.Dot(vertices[0], reverseEdgeNormal);    // Sets the Most in direction to the first Vertex.

            supportVertices[0] = 0;                                   // Just adds the first Vertex. Saves setting the fMostInDir to some rediculous starting value.                  

            // Now looping through edges to find the support Vertex starting from 1 (added the first vert by default).
            for (int i = 1, countI = vertices.Length; i < countI; ++i)
            {
                dot = Vector3.Dot(vertices[i], reverseEdgeNormal);

                if (dot > fMostInDir)
                {
                    fMostInDir = dot;                   // Sets the new most in direction
                    supportVertices[svAdded] = i;       // Adds the support vertex to the array.
                    svCount = 1;                        // Reset the svCount to 1 (as a new closer sv has been found)
                    ++svAdded;
                }
                else if (dot == fMostInDir)             // Need to add a support Vertex if it is equal in direction as the currently found vertex.
                {
                    supportVertices[svAdded] = i;       // Adds the support vertex to the array.
                    ++svCount;                          // Increase the ammount of found svs
                    ++svAdded;
                }
            }

            //Originally i had a supportVertices.Clear() to reset the new svs.  This was expensive. Instead the following replaced it with better performance.
            int[] supportVertices2 = new int[svCount];
            for (int i = svAdded - svCount, j = 0, countI = svAdded; i < countI; ++i, ++j)
                supportVertices2[j] = supportVertices[i];

            return supportVertices2;
        }
    }

}
