using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.IO;
using _2DLevelCreator;
using Shapes;
using Asteroid_Game;
using Global_Data;

namespace _2d_Objects
{
    public class BoundingCircle : RigidBody
    {
        #region Static Functions

        public static BoundingCircle ConvertToBoundingCircle(RigidBody rigidBody)
        {
            if (rigidBody is BoundingCircle) return (BoundingCircle)rigidBody;

            Vector3[] verts = ((ConvexRigidBody)(rigidBody)).verts;

            float radius = 0;
            for (int iCount = verts.Length - 1; iCount >= 0; --iCount)
            {
                float length = verts[iCount].Length();
                if (length > radius) radius = length;
            }

            return new BoundingCircle(new Transform(rigidBody.transform.vPosition, rigidBody.transform.zRotation, new Vector3(1, 1, 1)),
                                        rigidBody.motion, Vector3.Zero, radius, rigidBody.PhysicalPropertyID.Index, rigidBody.Static);
        }

        #endregion



        #region Data Members

        private float _radius;
        public float Radius
        {
            get { return this._radius; }
            set { if (value < 0) this._radius = -value; else this._radius = value; }
        }

        #endregion



        #region Constructors

        //Default Constructor
        public BoundingCircle(Transform transform, Motion motion, Vector3 centroid, float radius, int PhysicalPropertyIndex, bool isStatic): base(transform, motion, centroid, PhysicalPropertyIndex, isStatic)
        {
            this.Radius = radius;

            this.Rebuild();
        }

        //Copy Constructor
        public BoundingCircle(BoundingCircle bc): base(bc)
        {
            this.Radius = bc.Radius;

            this.Rebuild();
        }

        public override T DeepCopy<T>()
        {
            return (T)(object)(new BoundingCircle((BoundingCircle)(object)this));
        }

        //Load Constructor
        public BoundingCircle(string fileData): base(fileData)
        {
            this.Rebuild();
        }

        #endregion



        #region Functions


        public override void Rebuild()
        {
            this.CalculateMass();
            this.CalculateMomentOfInertia();

            this.aabb = new AABB(this.transform.vPosition, this.Radius, this.Radius);
            this.CreateMotionPathAABB();
        }


        public override void CreateAABB() { this.aabb.vPosition = this.transform.vPosition; }     // No need to regenerate aabb for circle every frame so just update position.

        public override void CalculateMass()
        {
            if (this.Static)
            {
                this.invMass = 0;
            }
            else
                this.invMass = 1 / (Physical_Property.list_PhysicalProperties[this.PhysicalPropertyID.Index].density * CalculateArea()); 
        }

        public override float CalculateArea() 
        { 
            return (float)Math.PI * this.Radius * this.Radius; 
        }

        public override void CreateMotionPathAABB()
        {
            Vector3 velo = (this.motion.vVelocityPS * Globals.timeDifference) * 0.5f;

            float halfWidth = this.Radius + Math.Abs(velo.X);
            float halfHeight = this.Radius + Math.Abs(velo.Y);

            this.motionPathAABB = new AABB(this.transform.vPosition + velo, halfHeight, halfWidth);
        }

        public override void CalculateMomentOfInertia() 
        {
            // Moment of inertia for circle in the z-axis is : ½M(R^4)
            if (this.Static)
            {
                this.invMoI_zAxis = 0;
            }
            else
            {
                this.invMoI_zAxis = 1 / (0.5f * (1 / this.invMass) * (this.Radius * this.Radius * this.Radius * this.Radius)); 
            }
        }

        public override void RescaleTo(Vector3 scale)   //Need to make the scale the same for x and y
        {
            if (scale.X != scale.Y)
            {
                scale.X = (scale.X + scale.Y) * 0.5f;
                scale.Y = scale.X;
            }

            ScaleBy(scale / this.transform.vScale); 
        }

        public override void ScaleBy(Vector3 scale) 
        {
            if (scale.X != scale.Y)
            {
                scale.X = (scale.X + scale.Y) * 0.5f;
                scale.Y = scale.X;
            }

            transform.vScale *= scale;

            this.Radius *= scale.X;
            this.Centroid *= scale.X;

            this.CalculateMass();
            this.CalculateMomentOfInertia();
            this.aabb = new AABB(this.transform.vPosition, this.Radius, this.Radius);
            this.CreateMotionPathAABB();
        }

        #endregion



        #region Collision Solver Functions

        public override List<CollisionData> GetSolver_ThisVsBoundingCircle<T>(T bc)
        {
            return Collision_Methods.GetSolver_CircleVsCircle(this, bc);
        }

        public override List<CollisionData> GetSolver_ThisVsOBB<T>(T obb)
        {
            return Collision_Methods.GetSolver_CircleVsOBB(this, obb);
        }

        public override List<CollisionData> GetSolver_ThisVsConvexPolygon<T>(T polygon)
        {
            return Collision_Methods.GetSolver_CircleVsConvex(this, polygon);
        }

        #endregion



        #region Draw Function
        //DELETE THIS AFTER TESTING
        public override void Draw(AsteroidGame xnaWindow)
        {
            /* //Reimplement
            if (Globals.DRAWMOTIONPATHAABB && (this.invMass != 0 || Globals.DRAWSTATICMOTIONPATHAABB)) this.motionPathAABB.Draw(xnaWindow, Color.Red);
            if (Globals.DRAWAABB && (this.invMass != 0 || Globals.DRAWSTATICAABB)) this.motionPathAABB.Draw(xnaWindow, Color.Yellow);


            Matrix worldMatrix = Matrix.CreateTranslation(transform.vPosition);
            
            xnaWindow.effect.Parameters["WorldViewProjMatrix"].SetValue(worldMatrix * xnaWindow.camera.ViewProjMatrix);
            xnaWindow.effect.Parameters["RotationMatrix"].SetValue(Matrix.CreateRotationZ(this.transform.zRotation));
            xnaWindow.effect.Parameters["Scale"].SetValue(new Vector3(1, 1, 1));



            if (Globals.FILLRIGIDBODY)
                (new Circle(this.transform.vPosition, this.Radius, 150, Circle.DrawStyle.Both, this.Colour, Color.White)).Draw(xnaWindow);
            else
                (new Circle(this.transform.vPosition, this.Radius, 150, Circle.DrawStyle.Outline, Color.Black, Color.White)).Draw(xnaWindow);
            */
        }
        #endregion


        #region Load Functions

        public override void Load(string fileData)
        {
            this.Radius = 0.5f;
            base.Load(fileData); //Might not need this line
        }

        #endregion

        #region Save Functions

        public override void Save(StreamWriter sw)
        {
            base.Save(sw); 
            sw.Write(" #RbID# " + "0" + " #/RbID# ");
        }

        #endregion
    }
}
