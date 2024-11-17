
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.IO;
using _2DLevelCreator;
using Shapes;


namespace _2d_Objects
{
    public class ConvexPolygon : ConvexRigidBody
    {
        #region Static Functions

        public static ConvexPolygon ConvertToConvexPolygon(RigidBody rigidBody)
        {
            if (rigidBody is ConvexPolygon) return (ConvexPolygon)rigidBody;

            Vector3[] verts = new Vector3[3];

            if (rigidBody is OBB)
                verts = ((OBB)(rigidBody)).verts;


            if (rigidBody is BoundingCircle)
                verts = Circle.GenerateVerts(6, ((BoundingCircle)(rigidBody)).Radius);


            return new ConvexPolygon(   new Transform(rigidBody.transform.vPosition, rigidBody.transform.zRotation, new Vector3(1, 1, 1)),
                                        new Motion(), Vector3.Zero,
                                        verts,
                                        rigidBody.PhysicalPropertyID.Index, rigidBody.Static);
        }


        #endregion



        #region Constructors

        //Default Constructor
        public ConvexPolygon(Transform transform, Motion motion, Vector3 centroid, Vector3[] verts, int PhysicalPropertyIndex, bool isStatic): base(transform, motion, centroid, PhysicalPropertyIndex, isStatic)
        {
            this.verts = verts;
            this.Rebuild();
        }

        //Copy Constructor
        public ConvexPolygon(ConvexPolygon poly): base(poly)    
        {
            this.verts = (Vector3[])poly.verts.Clone();
            this.Rebuild();
        }

        public override T DeepCopy<T>()
        {
            return (T)(object)(new ConvexPolygon((ConvexPolygon)(object)this));
        }


        //Load Constructor
        public ConvexPolygon(string fileData): base(fileData)
        {
            this.Rebuild();
        }

        #endregion



        #region Functions

        public override void Rebuild()
        {
            this.CalculateMomentOfInertia();
            this.UpdateRotationMatrix();
            this.UpdateWorldVerts();
            this.CreateAABB();
            this.CreateMotionPathAABB();
        }

        public override void CalculateMomentOfInertia()
        {
            // Moment of inertia for Quad in the z-axis is : (bh/12)(b² + h²)
            if (this.Static)
            {
                this.invMoI_zAxis = 0;
                this.invMass = 0;
            }
            else
            {
                if (this.verts == null) return;
                //Loop through triangles
                //Find midpoint of 2 verts.
                //Get line equation from this midpoint to the remaining vert.
                //Find modpoint of 2 verts. (different pair).
                //Get line equation from this midpoint to the remaining vert.
                //Find intersection between these lines.
                //Store intersection point as triangles centroid.

                //Now pick an edge. This length will be our base. Store this value.
                //Now project another edge onto this first edge.
                //Get the distance from this point to the vert not used to determine the first edge.
                //This distance will be the height. Store this value





                Vector3 midpoint1,midpoint2;
                LineEquation line1,line2;
                Vector3 polyCentroid= new Vector3(0,0,0);
                float a = 0, b = 0, h = 0;
  


                int numTriangles = this.verts.Length-2;
                Vector3[] triangleCentroid = new Vector3[numTriangles];
                float[] triangleArea = new float[numTriangles];
                float[] triangleMoI_zc = new float[numTriangles];
                float polygonArea = 0;

                for (int i = 1; i <= numTriangles; ++i)
                {

                    midpoint1 = this.verts[0] - ((this.verts[0] - this.verts[i]) * 0.5f);
                    line1 = new LineEquation(midpoint1, this.verts[i + 1 ]);
                    midpoint2 = this.verts[i] - ((this.verts[i] - this.verts[i + 1]) * 0.5f);
                    line2 = new LineEquation(midpoint2, this.verts[0]);
                    LineEquation.LineIntersectionPoint(line1, line2, ref triangleCentroid[i-1]);


                    Collision_Methods.GetTriangleHeightBaseandA(this.verts[0], this.verts[i], this.verts[i + 1], ref a, ref b, ref h);

                    triangleArea[i - 1] = 0.5f * b * h;
                    polygonArea += triangleArea[i - 1];

                    triangleMoI_zc[i - 1] = ((b*b*b*h)-(b*b*h*a) + (b*h*a*a)+ (b*h*h*h))/36;


                    polyCentroid += triangleCentroid[i - 1] * triangleArea[i - 1];
     
                }
                polyCentroid /= polygonArea;
                //this.objectCenter = polyCentroid;
                float distance; // Will hold the distance between the triangle CoM and polygon CoM
                //Now calculate MoI zc for the polygon.

                this.invMoI_zAxis=0;
                for (int i = 1; i <= numTriangles; ++i)
                {
                    distance = (polyCentroid - triangleCentroid[i - 1]).Length();
                    this.invMoI_zAxis += (triangleMoI_zc[i - 1] + (triangleArea[i - 1] * distance * distance));
                }

                this.invMass = polygonArea * Physical_Property.list_PhysicalProperties[this.PhysicalPropertyID.Index].density;
                this.invMoI_zAxis = 1 / (invMass * this.invMoI_zAxis);

                this.invMass = 1 / this.invMass;
            }

        }

        public override float CalculateArea() 
        {
            float a = 0, b = 0, h = 0;
            float polygonArea = 0;

            for (int iCount = 1, numTriangles = this.verts.Length - 2; iCount <= numTriangles; ++iCount)
            {
                Collision_Methods.GetTriangleHeightBaseandA(this.verts[0], this.verts[iCount], this.verts[iCount + 1], ref a, ref b, ref h);

                polygonArea += 0.5f * b * h;
            }
            return polygonArea; 
        }


        public override void ScaleBy(Vector3 scale)
        {

            transform.vScale *= scale;

            this.Centroid *= scale;


            for (int i = 0; i < this.verts.Length; ++i)
            {
                this.verts[i] *= scale;
            }

            this.CalculateMass();
            this.CalculateMomentOfInertia();
            this.CreateAABB();
            this.CreateMotionPathAABB();
        }

        #endregion



        #region Collision Solver Functions

        public override List<CollisionData> GetSolver_ThisVsBoundingCircle<T>(T bc)
        {
            List<CollisionData> collisionSolverData = Collision_Methods.GetSolver_CircleVsConvex(bc, this);
            for (int i = 0; i < collisionSolverData.Count; ++i) collisionSolverData[i].SwapA_B();
            return collisionSolverData;
        }

        public override List<CollisionData> GetSolver_ThisVsOBB<T>(T obb)
        {
            List<CollisionData> collisionSolverData = Collision_Methods.GetSolver_ConvexVsConvex(this, obb);
            return collisionSolverData;
        }

        public override List<CollisionData> GetSolver_ThisVsConvexPolygon<T>(T polygon)
        {
            List<CollisionData> collisionSolverData = Collision_Methods.GetSolver_ConvexVsConvex(this, polygon);
            return collisionSolverData;
        }

        #endregion



        #region Load Functions

        public override void Load(string fileData)
        {
            base.Load(fileData); //Might not need this line
            this.verts = StringMalarkey.GetVector3ArrayFromString(fileData, "RbVerts");
        }

        #endregion

        #region Save Functions

        public override void Save(StreamWriter sw)
        {
            base.Save(sw);

            sw.Write(" #RbID# " + "2" + " #/RbID# ");
            
            sw.Write(" #RbVerts# ");
            for (int iCount = 0; iCount < this.verts.Length; ++iCount)
            { sw.Write(this.verts[iCount].X / this.transform.vScale.X + ", " + this.verts[iCount].Y / this.transform.vScale.Y + ", "); }
            sw.Write(" #/RbVerts# ");
        }

        #endregion
    }
}
