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
    public interface IRigidBody
    {
        List<CollisionData> GetSolver_ThisVsBoundingCircle<T>(T bc) where T : BoundingCircle;
        List<CollisionData> GetSolver_ThisVsOBB<T>(T obb) where T : OBB;
        List<CollisionData> GetSolver_ThisVsConvexPolygon<T>(T polygon) where T : ConvexPolygon;
    }

    public enum RbFlags
    {
        IsStatic        = 1<<0,
        CanTranslate    = 1<<1,
        CanRotate       = 1<<2,
        CanScale        = 1<<3
    }

    public  abstract class RigidBody : IRigidBody
    {
        #region Data Members

        public Transform transform = new Transform();
        public Motion motion = new Motion();
        public Matrix rotMatrix = Matrix.CreateRotationZ(0);              //Storing the rotation matrix because it is used more than once per frame. Somewhat expensive to create.

        public Vector3 Centroid{ get; set; }                    //This is in object space.
        public Vector3 CentroidRotated { get; set; }            //This is in object space with rotation applied


        public float invMass { get; set; }                      // 1/Mass
        public float invMoI_zAxis { get; set; }                 // Will hold the moment of inertia of the rigid body with respect to the z-axis.

        private bool _Static;
        public bool Static
        {
            get { return this._Static; }
            set { this._Static = value; this.CalculateMass(); this.CalculateMomentOfInertia(); }
        }



        public UniqueIdentifier_Reference PhysicalPropertyID { get; set; }  //(WARNING)( NEED TO UPDATE AFTER INDEX CHANGE)                // kg/cu.m

        public Vector3 FaceNormal { get; set; }
        public Color Colour { get; set; }

        public AABB aabb { get; set; }                      //(CHANGE) We dont ever use this
        public AABB motionPathAABB { get; set; }

        public List<TouchingContactContainer> touchingContactsContainerList = new List<TouchingContactContainer>();

        #endregion



        #region Constructors
        
        //Constructor
        public RigidBody(Transform transform, Motion motion, Vector3 centroid, int PhysicalPropertyIndex, bool isStatic)
        {
            this.transform = new Transform(transform);
            this.motion = new Motion(motion);
            this.Centroid = centroid;

            this.PhysicalPropertyID = new UniqueIdentifier_Reference(Physical_Property.list_PhysicalProperties[0].FindID(PhysicalPropertyIndex));

            this.FaceNormal = new Vector3(0, 0, 1);
            this.Static = isStatic;

            this.Colour = new Color(Globals.randNum.Next(0, 155), Globals.randNum.Next(0, 155), Globals.randNum.Next(0, 155), 255);
        }

        // Copy constructor.
        public RigidBody(RigidBody rigidBody)
        {
            this.transform = new Transform(rigidBody.transform);
            this.motion = new Motion(rigidBody.motion);
            this.Centroid = rigidBody.Centroid;

            this.PhysicalPropertyID = new UniqueIdentifier_Reference(rigidBody.PhysicalPropertyID);

            this.FaceNormal = new Vector3(0, 0, 1);
            this.Static = rigidBody.Static;

            this.Colour = new Color(Globals.randNum.Next(0, 155), Globals.randNum.Next(0, 155), Globals.randNum.Next(0, 155), 255);
            this.rotMatrix = rigidBody.rotMatrix;
        }

        public virtual T DeepCopy<T>() where T : RigidBody
        {
            return (T)null;
        }

        //Load Constructor
        public RigidBody(string fileData)
        {
            this.Load(fileData);
        }

        #endregion



        #region Functions

        public Vector3 GetCentriod_World() { return this.transform.vPosition + this.CentroidRotated; }
        public abstract void ScaleBy(Vector3 scale);    //Forces classes that inherit rigidbody to write their own scaleby routine
        public virtual void RescaleTo(Vector3 scale) { ScaleBy(scale / this.transform.vScale); }


        public virtual void CreateAABB() {  }
        public virtual void CreateMotionPathAABB() { }
        public virtual void CalculateMomentOfInertia() { this.invMoI_zAxis = 1; }
        public virtual void CalculateMass() { this.invMass = 1; }
        public virtual float CalculateArea() { return 1; }

        public virtual void Update() 
        {
            this.UpdateRotationMatrix();
            this.UpdateRotatedCentroid();
            this.CreateAABB();               // The aabb will appear to lag behind the rb. This is because the rbs pos is updated after this aabb is made. This is how it should be.
            this.CreateMotionPathAABB();     // The aabb will appear to lag behind the rb. This is because the rbs pos is updated after this aabb is made. This is how it should be.
        }
        public void UpdateRotationMatrix()
        {
            this.rotMatrix = Matrix.CreateRotationZ(this.transform.zRotation);
        }
        public void UpdateRotatedCentroid()
        {
            this.CentroidRotated = Vector3.Transform(this.Centroid, this.rotMatrix);
        }

        public void AddRotation(float rotation)
        {
            Vector3 centroidWorld = this.transform.vPosition + this.CentroidRotated;

            this.transform.zRotation += rotation;
            this.UpdateRotationMatrix();
            this.UpdateRotatedCentroid();

            this.transform.vPosition = centroidWorld - this.CentroidRotated;
        }

        public virtual void Rebuild() { }

        #endregion



        #region Draw Functions

        public virtual void Draw(AsteroidGame xnaWindow) { }

        public virtual void DrawObjectCenter(AsteroidGame xnaWindow) 
        {
            (new Circle(this.transform.vPosition, 0.0018f * (this.transform.vPosition.Z - xnaWindow.camera.CameraPosition.Z), 111, Circle.DrawStyle.Both, Color.White, Color.White)).Draw(xnaWindow);
            (new Circle(this.transform.vPosition, 0.0015f * (this.transform.vPosition.Z - xnaWindow.camera.CameraPosition.Z), 111, Circle.DrawStyle.Both, Color.Black, Color.White)).Draw(xnaWindow);
            (new Circle(this.transform.vPosition, 0.0009f * (this.transform.vPosition.Z - xnaWindow.camera.CameraPosition.Z), 111, Circle.DrawStyle.Both, Color.Red, Color.White)).Draw(xnaWindow);
        }

        public virtual void DrawCentroid(AsteroidGame xnaWindow)
        {
            (new Circle(this.GetCentriod_World(), 0.0018f * (this.transform.vPosition.Z - xnaWindow.camera.CameraPosition.Z), 111, Circle.DrawStyle.Both, Color.White, Color.White)).Draw(xnaWindow);
            (new Circle(this.GetCentriod_World(), 0.0015f * (this.transform.vPosition.Z - xnaWindow.camera.CameraPosition.Z), 111, Circle.DrawStyle.Both, Color.Black, Color.White)).Draw(xnaWindow);
            (new Circle(this.GetCentriod_World(), 0.0009f * (this.transform.vPosition.Z - xnaWindow.camera.CameraPosition.Z), 111, Circle.DrawStyle.Both, Color.Yellow, Color.White)).Draw(xnaWindow);
        }

        public virtual void HighLight(AsteroidGame xnaWindow)
        {
            this.aabb.Draw(xnaWindow, Color.LightGreen);
        }

        #endregion



        #region Solver Functions

        public virtual List<CollisionData> GetSolver_ThisVsBoundingCircle<T>(T bc) where T : BoundingCircle { return null; }
        public virtual List<CollisionData> GetSolver_ThisVsOBB<T>(T obb) where T : OBB { return null; }
        public virtual List<CollisionData> GetSolver_ThisVsConvexPolygon<T>(T polygon) where T : ConvexPolygon { return null; }

        public List<CollisionData> GetCollisionSolver<T>(T t)
        {
            if (t is BoundingCircle)                                                            //Checks whether the object, that is to be tested is a BoundingCircle.
            {
                return GetSolver_ThisVsBoundingCircle<BoundingCircle>((BoundingCircle)(object)t);
            }
            else if (t is OBB)                                                                  //Checks whether the object, that is to be tested is a OBB.
            {
                return GetSolver_ThisVsOBB<OBB>((OBB)(object)t);
            }
            else if (t is ConvexPolygon)                                                        //Checks whether the object, that is to be tested is a Polygon.
            {
                return GetSolver_ThisVsConvexPolygon<ConvexPolygon>((ConvexPolygon)(object)t);
            }
            else
                return null;
        }

        #endregion



        #region Load Functions

        public virtual void Load(string fileData)
        {
            // PhysicalPropertyID needs to be set before Static. 
            this.PhysicalPropertyID = new UniqueIdentifier_Reference(Physical_Property.list_PhysicalProperties[0].FindID(Convert.ToInt32(StringMalarkey.ExtractString(fileData, "PHYSPROPERTYINDEX"))));
            this.Static = StringMalarkey.GetBoolFromString(fileData, "Static");
            this.Centroid = StringMalarkey.GetVector3FromString(fileData, "ObjCenter");
        }

        #endregion
        

        #region Load Functions

        public virtual void Save(StreamWriter sw)
        {
            sw.Write(" #PHYSPROPERTYINDEX# " + this.PhysicalPropertyID.Index + " #/PHYSPROPERTYINDEX# ");
            sw.Write(" #ObjCenter# " + this.Centroid.X + ", " + this.Centroid.Y + " #/ObjCenter# ");
            sw.Write(" #Static# " + ((this.Static) ? "1" : "0") + " #/Static# ");
        }

        #endregion
    }
}
