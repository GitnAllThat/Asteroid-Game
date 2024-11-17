using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.IO;
using _2DLevelCreator;


namespace _2d_Objects
{
    public class OBB : ConvexRigidBody
    {
        #region Static Functions

        public static OBB ConvertToOBB(RigidBody rigidBody)
        {
            if (rigidBody is OBB) return (OBB)rigidBody;

            if (rigidBody is BoundingCircle) return OBB.ConvertBoundingCircleToOBB((BoundingCircle)rigidBody);

            if (rigidBody is ConvexPolygon) return OBB.ConvertBoundingCircleToOBB((ConvexPolygon)rigidBody);

            return null;
        }

        private static OBB ConvertBoundingCircleToOBB(BoundingCircle boundingCircle)
        {
            return new OBB(new Transform(boundingCircle.transform.vPosition, boundingCircle.transform.zRotation, new Vector3(1, 1, 1)),
                                        boundingCircle.motion, Vector3.Zero, 
                                        2*(boundingCircle.Radius * (float)Math.Sin(MathHelper.ToRadians(45))),
                                        2*(boundingCircle.Radius * (float)Math.Cos(MathHelper.ToRadians(45))),
                                        boundingCircle.PhysicalPropertyID.Index, boundingCircle.Static);
        }

        private static OBB ConvertBoundingCircleToOBB(ConvexPolygon convexPolygon)
        {
            float width = 0, height = 0;

            for (int iCount = convexPolygon.verts.Length - 1; iCount >= 0; --iCount)
            {
                if (Math.Abs(convexPolygon.verts[iCount].X) > width) width = Math.Abs(convexPolygon.verts[iCount].X);
                if (Math.Abs(convexPolygon.verts[iCount].Y) > height) height = Math.Abs(convexPolygon.verts[iCount].Y);
            }

            return new OBB(new Transform(convexPolygon.transform.vPosition, convexPolygon.transform.zRotation, new Vector3(1, 1, 1)),
                                        convexPolygon.motion, Vector3.Zero, height, width,
                                        convexPolygon.PhysicalPropertyID.Index, convexPolygon.Static);
        }
        #endregion



        #region Data Members

        public float HalfWidth { get; set; }
        public float HalfHeight { get; set; }

        #endregion



        #region Constructors

        //Default Constructor.
        public OBB(Transform transform, Motion motion, Vector3 centroid, float height, float width, int PhysicalPropertyIndex, bool isStatic): base(transform, motion, centroid, PhysicalPropertyIndex, isStatic)
        {
            this.HalfHeight = height * 0.5f;
            this.HalfWidth = width * 0.5f;
            this.Rebuild();
        }
        //Copy Constructor.
        public OBB(OBB obb): base(obb) 
        {
            this.HalfHeight = obb.HalfHeight;
            this.HalfWidth = obb.HalfWidth;
            this.Rebuild();
        }

        public override T DeepCopy<T>()
        {
            return (T)(object)(new OBB((OBB)(object)this));
        }

        //Load Constructor
        public OBB(string fileData): base(fileData)
        {
            this.Rebuild();
        }

        #endregion



        #region Functions


        public override void Rebuild()
        {
            this.SetupVerts();
            this.CalculateMass();
            this.CalculateMomentOfInertia();

            this.UpdateRotationMatrix();
            this.UpdateWorldVerts();
            this.CreateAABB();
            this.CreateMotionPathAABB();
        }

        public override void CalculateMass()
        {
            if (this.Static)
            {
                this.invMass = 0;
            }
            else
            {
                this.invMass = 1 / (Physical_Property.list_PhysicalProperties[this.PhysicalPropertyID.Index].density * CalculateArea());
            }
        }

        public override float CalculateArea() {return (this.HalfHeight * this.HalfWidth) * 4;}

        protected void SetupVerts()
        {
            this.verts = new Vector3[4];
            this.verts[0] = new Vector3(-HalfWidth, HalfHeight, 0);
            this.verts[1] = new Vector3(HalfWidth, HalfHeight, 0);
            this.verts[2] = new Vector3(HalfWidth, -HalfHeight, 0);
            this.verts[3] = new Vector3(-HalfWidth, -HalfHeight, 0);
        }

        public override void CalculateMomentOfInertia()
        {
            // Moment of inertia for Quad in the z-axis is : (bh/12)(b² + h²)
            if (this.Static)
            {
                this.invMoI_zAxis = 0;
            }
            else
            {
                float mass = (1 / this.invMass);
                this.invMoI_zAxis = 1 / (mass * ((this.HalfWidth * this.HalfHeight) / 3) * ((this.HalfWidth * this.HalfWidth * 4) + (this.HalfHeight * this.HalfHeight * 4)));
            }
        }

        public override void ScaleBy(Vector3 scale)
        {
            transform.vScale *= scale;

            this.HalfWidth *= scale.X;
            this.HalfHeight *= scale.Y;
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
            List<CollisionData> collisionSolverData = Collision_Methods.GetSolver_CircleVsOBB(bc, this);
            for(int i = 0; i < collisionSolverData.Count; ++i)   collisionSolverData[i].SwapA_B();
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
            this.HalfWidth = 0.5f;
            this.HalfHeight = 0.5f;
        }

        #endregion

        #region Save Functions

        public override void Save(StreamWriter sw)
        {
            base.Save(sw);

            sw.Write(" #RbID# " + "1" + " #/RbID# ");
        }

        #endregion
    }
}
