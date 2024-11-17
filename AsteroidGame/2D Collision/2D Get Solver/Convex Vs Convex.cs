
using System.Collections.Generic;
using Microsoft.Xna.Framework;


namespace _2d_Objects
{
    static partial class Collision_Methods
    {
        // This handles the convex objects: aabb, obb, polygon.
        public static List<CollisionData> GetSolver_ConvexVsConvex(ConvexRigidBody rigidBodyA, ConvexRigidBody rigidBodyB)
        {

            Vector3[] vertsA = rigidBodyA.vertsWorld;
            Vector3[] vertsB = rigidBodyB.vertsWorld;
            int[] SVs;
            Vector3 reverseEdgeNormal;
            Vector3 worldFaceNormal = rigidBodyB.FaceNormal;
            Vector3 mdCenter = rigidBodyA.transform.vPosition - rigidBodyB.transform.vPosition;
            Vector3 closestPoint = new Vector3(0,0,0);
            Vector3 currentPoint;
            float closestDistance = float.MaxValue;
            float currentDistance;
            Vector3 edgeVert2;
            Vector3 finalNormal = new Vector3(0, 0, 0);
            Vector3 vContactPointA = new Vector3(0, 0, 0);
            Vector3 vContactPointB = new Vector3(0, 0, 0);
            int foundEdgeVert = 0;
            int foundSvVert = 0;
            bool foundInA = false;


            ///////////////////////////////////////////////////////
            ///   B's Edges With A's as Support Vertices Start  ///
            ///////////////////////////////////////////////////////
            //Calculate the Minkowski Difference using A's Edges and B's as support vetrices
            for (int i = 0,   vertNum = vertsB.Length; i < vertNum; ++i)
            {
                edgeVert2 = vertsB[(i + 1) % vertNum];
                reverseEdgeNormal = Vector3.Cross(worldFaceNormal, vertsB[i] - edgeVert2);

                //if (((Collision_Methods._lookingAt1 == Collision_Methods._Want1 && Collision_Methods._lookingAt2 == Collision_Methods._Want2) ||
                //(Collision_Methods._lookingAt1 == Collision_Methods._Want2 && Collision_Methods._lookingAt2 == Collision_Methods._Want1)))
                //{
                //    Collision_Methods._vectorsToDraw.Add(new Line(vertsB[i], edgeVert2, Color.LightBlue, 2));
                //}

                if (Vector3.Dot(mdCenter, -reverseEdgeNormal) >= 0)             //Remove Backfacing Edges. NOTE: minus is added so that edge is facing the right direction for this object.
                {
                    SVs = GetSupportVertices(ref vertsA, reverseEdgeNormal);    //Gets the support vertex(or vertices) for the current edge.

                    for (int j = 0; j < SVs.Length; ++j)                        //Check to see if the support vetex/es produce a time of contact.
                    {
                        //Collision_Methods._vectorsToDraw.Add(new Line(vertsA[SVs[j]] - vertsB[i], vertsA[SVs[j]] - edgeVert2, Color.Orange, 2));

                        currentPoint = PointOnEdgeClosestToOriginClamped(vertsA[SVs[j]] - vertsB[i], vertsA[SVs[j]] - edgeVert2);

                        //Collision_Methods._generalPoints.Add(new Circle(currentPoint, 0.1f, 55, true, Color.Red));

                        currentDistance = currentPoint.LengthSquared();
                        if (currentDistance < closestDistance)
                        {
                            closestDistance = currentDistance;
                            closestPoint = currentPoint;
                            finalNormal = reverseEdgeNormal;
                            vContactPointB = vertsA[SVs[j]] - closestPoint - rigidBodyB.transform.vPosition;
                            vContactPointA = vertsA[SVs[j]] - rigidBodyA.transform.vPosition;

                            foundEdgeVert = i;
                            foundSvVert = SVs[j];
                        }
                    }
                }
            }
            ///////////////////////////////////////////////////////
            ///   B's Edges With A's as Support Vertices End    ///
            ///////////////////////////////////////////////////////


            worldFaceNormal = rigidBodyA.FaceNormal;
            ///////////////////////////////////////////////////////
            ///   A's Edges With B's as Support Vertices Start  ///
            ///////////////////////////////////////////////////////
            for (int i = 0, vertNum = vertsA.Length; i < vertNum; ++i)
            {
                edgeVert2 = vertsA[(i + 1) % vertNum];
                reverseEdgeNormal = Vector3.Cross(worldFaceNormal, vertsA[i] - edgeVert2);

                if (Vector3.Dot(mdCenter, reverseEdgeNormal) >= 0)              //Remove Backfacing Edges. NOTE: no need to put a minus in front of the reverseEdgeNormal.
                {
                    SVs = GetSupportVertices(ref vertsB, reverseEdgeNormal);    //Gets the support vertex(or vertexes) for the current edge.

                    for (int j = 0; j < SVs.Length; ++j)                        //Check to see if the support vetex/es produce a time of contact.
                    {
                        //Collision_Methods._vectorsToDraw.Add(new Line(vertsA[i] - vertsB[SVs[j]], edgeVert2 - vertsB[SVs[j]], Color.Blue, 2));

                        currentPoint = PointOnEdgeClosestToOriginClamped(vertsA[i] - vertsB[SVs[j]] , edgeVert2 - vertsB[SVs[j]]);

                        //Collision_Methods._generalPoints.Add(new Circle(currentPoint, 0.1f, 55, true, Color.Red));

                        currentDistance = currentPoint.LengthSquared();
                        if (currentDistance < closestDistance)
                        {
                            closestDistance = currentDistance;
                            closestPoint = currentPoint;
                            finalNormal = -reverseEdgeNormal;

                            vContactPointA = closestPoint + vertsB[SVs[j]] - rigidBodyA.transform.vPosition;
                            vContactPointB = vertsB[SVs[j]] - rigidBodyB.transform.vPosition;

                            foundEdgeVert = i;
                            foundSvVert = SVs[j];
                            foundInA = true;
                        }
                    }
                }
            }
            ///////////////////////////////////////////////////////
            ///   A's Edges With B's as Support Vertices End    ///
            ///////////////////////////////////////////////////////



            Vector3[] contactSet;
            Vector3 pos1 = rigidBodyA.transform.vPosition, pos2= rigidBodyB.transform.vPosition;
            FeaturePair fp; int fpKey;
            int[] indexPair = new int[4];
            bool svVert2Check = false;
            if (foundInA)
            {
                indexPair[0] = (foundEdgeVert + 1) % vertsA.Length; indexPair[2] = foundEdgeVert;
                contactSet = GetContactSet(vertsA[foundEdgeVert], vertsA[(foundEdgeVert + 1) % vertsA.Length], vertsB[foundSvVert], vertsB[(foundSvVert + vertsB.Length - 1) % vertsB.Length], vertsB[(foundSvVert + 1) % vertsB.Length], foundInA, ref svVert2Check);
                if (svVert2Check) { indexPair[1] = foundSvVert; indexPair[3] = (foundSvVert + 1) % vertsB.Length; } else { indexPair[1] = (foundSvVert + vertsB.Length - 1) % vertsB.Length; indexPair[3] = foundSvVert; }



            }
            else
            {
                indexPair[0] = (foundEdgeVert + 1) % vertsB.Length; indexPair[2] = foundEdgeVert;
                contactSet = GetContactSet(vertsB[foundEdgeVert], vertsB[(foundEdgeVert + 1) % vertsB.Length], vertsA[foundSvVert], vertsA[(foundSvVert + vertsA.Length - 1) % vertsA.Length], vertsA[(foundSvVert + 1) % vertsA.Length], foundInA, ref svVert2Check);
                if (svVert2Check) { indexPair[1] = foundSvVert; indexPair[3] = (foundSvVert + 1) % vertsA.Length; } else { indexPair[1] = (foundSvVert + vertsA.Length - 1) % vertsA.Length; indexPair[3] = foundSvVert; }


            }





            
            finalNormal.Normalize();
            List<CollisionData> contactList = new List<CollisionData>(); 

            //Add first contact
            closestPoint = contactSet[0] - contactSet[1];
            closestDistance = closestPoint.Length();

            if ((Vector3.Dot(-finalNormal, contactSet[0] - contactSet[1]) < 0)) //Make the distance negative if rigidbodies are inside each other.
            { closestDistance = -closestDistance; }

            if (foundInA) { fp = new FeaturePair(closestDistance, foundSvVert, foundEdgeVert, eFeaturePair.VertexBEdgeA); } else { fp = new FeaturePair(closestDistance, foundSvVert, foundEdgeVert, eFeaturePair.VertexAEdgeB); }
            fpKey = TouchingContact.GenerateFeaturePairKey(fp, indexPair[1]);

            CollisionData collisionSolverData = new CollisionData(closestDistance, finalNormal, rigidBodyA, rigidBodyB, 0, (contactSet[0] - pos1) - rigidBodyA.CentroidRotated, (contactSet[1] - pos2) - rigidBodyB.CentroidRotated);
            //collisionSolverData.vContactPointA = contactSet[0] - pos1;
            //collisionSolverData.vContactPointB = contactSet[1] - pos2;

            collisionSolverData.vPerpCToCpA = Vector3.Cross(collisionSolverData.vContactPointA, new Vector3(0, 0, -1));
            collisionSolverData.vPerpCToCpB = Vector3.Cross(collisionSolverData.vContactPointB, new Vector3(0, 0, -1));
            contactList.Add(collisionSolverData);







            
            //Add Second contact
            closestPoint = contactSet[2] - contactSet[3];
            closestDistance = closestPoint.Length();

            if (Vector3.Dot(-finalNormal, contactSet[2] - contactSet[3]) < 0) //Make the distance negative if rigidbodies are inside each other.
            { closestDistance = -closestDistance; }

            if (foundInA) { fp = new FeaturePair(closestDistance, foundSvVert, foundEdgeVert, eFeaturePair.VertexBEdgeA); } else { fp = new FeaturePair(closestDistance, foundSvVert, foundEdgeVert, eFeaturePair.VertexAEdgeB); }
            fpKey = TouchingContact.GenerateFeaturePairKey(fp, indexPair[3]);

            collisionSolverData = new CollisionData(closestDistance, finalNormal, rigidBodyA, rigidBodyB, 2, (contactSet[2] - pos1) - rigidBodyA.CentroidRotated, (contactSet[3] - pos2) - rigidBodyB.CentroidRotated);

            contactList.Add(collisionSolverData);







            if (contactList[1].distance < contactList[0].distance) contactList.Reverse(); //Put closest first
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
                Collision_Methods._vectorsToDraw.Add(new Line(contactList[0].vContactPointA + rigidBodyA.transform.vPosition, contactList[0].vContactPointA + rigidBodyA.transform.vPosition + finalNormal, Color.Orange, 2));
                Collision_Methods._vectorsToDraw.Add(new Line(contactList[0].vContactPointA + rigidBodyA.transform.vPosition, contactList[0].vContactPointA + rigidBodyA.transform.vPosition + contactList[0].vPerpCToCpA, Color.Red, 2));
                Collision_Methods._vectorsToDraw.Add(new Line(contactList[0].vContactPointB + rigidBodyB.transform.vPosition, contactList[0].vContactPointB + rigidBodyB.transform.vPosition + finalNormal, Color.LightBlue, 2));
                Collision_Methods._vectorsToDraw.Add(new Line(contactList[0].vContactPointB + rigidBodyB.transform.vPosition, contactList[0].vContactPointB + rigidBodyB.transform.vPosition + contactList[0].vPerpCToCpB, Color.Yellow, 2));
                //Collision_Methods._vectorsToDraw.Add(new Line(contactList[0].vContactPointA + rigidBodyA.transform.vPosition, collisionSolverData.vContactPointB + rigidBodyB.transform.vPosition, Color.White, 1));

                Collision_Methods._vectorsToDraw.Add(new Line(contactList[1].vContactPointA + rigidBodyA.transform.vPosition, contactList[1].vContactPointA + rigidBodyA.transform.vPosition + finalNormal, Color.Orange, 2));
                Collision_Methods._vectorsToDraw.Add(new Line(contactList[1].vContactPointA + rigidBodyA.transform.vPosition, contactList[1].vContactPointA + rigidBodyA.transform.vPosition + contactList[1].vPerpCToCpA, Color.Red, 2));
                Collision_Methods._vectorsToDraw.Add(new Line(contactList[1].vContactPointB + rigidBodyB.transform.vPosition, contactList[1].vContactPointB + rigidBodyB.transform.vPosition + finalNormal, Color.LightBlue, 2));
                Collision_Methods._vectorsToDraw.Add(new Line(contactList[1].vContactPointB + rigidBodyB.transform.vPosition, contactList[1].vContactPointB + rigidBodyB.transform.vPosition + contactList[1].vPerpCToCpB, Color.Yellow, 2));
                //Collision_Methods._vectorsToDraw.Add(new Line(contactList[1].vContactPointA + rigidBodyA.transform.vPosition, collisionSolverData.vContactPointB + rigidBodyB.transform.vPosition, Color.White, 1));
            }

            if (((Collision_Methods.lookingAt1 == Collision_Methods.Want1 && Collision_Methods.lookingAt2 == Collision_Methods.Want2) ||
                (Collision_Methods.lookingAt1 == Collision_Methods.Want2 && Collision_Methods.lookingAt2 == Collision_Methods.Want1)))
            {
                Collision_Methods._vectorsToDraw.Add(new Line(contactList[0].vContactPointA + rigidBodyA.transform.vPosition, contactList[0].vContactPointA + rigidBodyA.transform.vPosition + finalNormal, Color.Orange, 2));
                Collision_Methods._vectorsToDraw.Add(new Line(contactList[0].vContactPointA + rigidBodyA.transform.vPosition, contactList[0].vContactPointA + rigidBodyA.transform.vPosition + contactList[0].vPerpCToCpA, Color.Red, 2));
                Collision_Methods._vectorsToDraw.Add(new Line(contactList[0].vContactPointB + rigidBodyB.transform.vPosition, contactList[0].vContactPointB + rigidBodyB.transform.vPosition + finalNormal, Color.LightBlue, 2));
                Collision_Methods._vectorsToDraw.Add(new Line(contactList[0].vContactPointB + rigidBodyB.transform.vPosition, contactList[0].vContactPointB + rigidBodyB.transform.vPosition + contactList[0].vPerpCToCpB, Color.Yellow, 2));


                Collision_Methods._vectorsToDraw.Add(new Line(contactList[1].vContactPointA + rigidBodyA.transform.vPosition, contactList[1].vContactPointA + rigidBodyA.transform.vPosition + finalNormal, Color.Orange, 2));
                Collision_Methods._vectorsToDraw.Add(new Line(contactList[1].vContactPointA + rigidBodyA.transform.vPosition, contactList[1].vContactPointA + rigidBodyA.transform.vPosition + contactList[1].vPerpCToCpA, Color.Red, 2));
                Collision_Methods._vectorsToDraw.Add(new Line(contactList[1].vContactPointB + rigidBodyB.transform.vPosition, contactList[1].vContactPointB + rigidBodyB.transform.vPosition + finalNormal, Color.LightBlue, 2));
                Collision_Methods._vectorsToDraw.Add(new Line(contactList[1].vContactPointB + rigidBodyB.transform.vPosition, contactList[1].vContactPointB + rigidBodyB.transform.vPosition + contactList[1].vPerpCToCpB, Color.Yellow, 2));



                Vector3[] vvv = rigidBodyA.GetWorldVerts(Vector3.Zero);
                for (int i = 0; i < vvv.Length; ++i)
                    Collision_Methods._vectorsToDraw.Add(new Line(vvv[i], vvv[(i + 1) % vvv.Length], Color.Red, 2));

                Collision_Methods._generalPoints.Add(new Circle(contactList[0].vContactPointA, 0.1f, 55, true, Color.Red));
                Collision_Methods._generalPoints.Add(new Circle(contactList[1].vContactPointA, 0.1f, 55, true, Color.Red));

                vvv = rigidBodyB.GetWorldVerts(Vector3.Zero);
                for (int i = 0; i < vvv.Length; ++i)
                    Collision_Methods._vectorsToDraw.Add(new Line(vvv[i], vvv[(i + 1) % vvv.Length], Color.Blue, 2));
                Collision_Methods._generalPoints.Add(new Circle(contactList[0].vContactPointB, 0.1f, 55, true, Color.Blue));
                Collision_Methods._generalPoints.Add(new Circle(contactList[1].vContactPointB, 0.1f, 55, true, Color.Blue));
 * 
 *             Collision_Methods._generalPoints.Add(new Circle(collisionSolverData.vContactPointA + rigidBodyA.transform.vPosition, 0.02f, 55, true, Color.Blue));
            Collision_Methods._generalPoints.Add(new Circle(collisionSolverData.vContactPointB + rigidBodyB.transform.vPosition, 0.02f, 55, true, Color.Yellow));
            }
    /////////////////
    //  Debugging  //
    /////////////////
 */