
using System.Collections.Generic;
using Microsoft.Xna.Framework;



namespace _2d_Objects
{
    static partial class Collision_Methods
    {
        public static List<CollisionData> GetSolver_CircleVsCircle(BoundingCircle bc1, BoundingCircle bc2)
        {
            Vector3 dist = bc2.transform.vPosition - bc1.transform.vPosition;

            float distance = dist.Length();

            Vector3 finalNormal;
            if (distance == 0)      // Checks to see if circles are on top of each other (normalised dist would be NAN).
            {
                finalNormal = new Vector3(1,0,0);   //Circles are on top so any normal can be given.
            }
            else                    // Circles are not on top of each other so can compute normal by normalising the distance between the objects.
            {
                finalNormal = dist / distance;
            }

            distance -= (bc2.Radius + bc1.Radius);





            CollisionData collisionSolverData = new CollisionData(distance, finalNormal, bc1, bc2, 0000, (finalNormal * bc1.Radius) - bc1.CentroidRotated, (-finalNormal * bc2.Radius) - bc2.CentroidRotated);


            List<CollisionData> contactList = new List<CollisionData>(); contactList.Add(collisionSolverData);
            return contactList;
        }
    }
}


