
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Global_Data;
using _2DLevelCreator;
using Asteroid_Game;



namespace _2d_Objects
{
    public abstract class ConvexRigidBody : RigidBody
    {
        #region Data Members

        public Vector3[] verts;
        public Vector3[] vertsWorld;

        #endregion



        #region Constructors
        
        //Default Constructor
        public ConvexRigidBody(Transform transform, Motion motion, Vector3 centroid, int PhysicalPropertyIndex, bool isStatic) : base(transform, motion, centroid, PhysicalPropertyIndex, isStatic) { }

        //Copy Constructor.
        public ConvexRigidBody(RigidBody rigidBody) : base(rigidBody) { }
        
        //Load Constructor
        public ConvexRigidBody(string fileData) : base(fileData)
        {
        }

        #endregion



        #region Functions

        public override void  Update()
        {
            this.UpdateRotationMatrix();
            this.UpdateWorldVerts();
            this.CreateAABB();               // The aabb will appear to lag behind the rb. This is because the rbs pos is updated after this aabb is made. This is how it should be.
            this.CreateMotionPathAABB();     // The aabb will appear to lag behind the rb. This is because the rbs pos is updated after this aabb is made. This is how it should be.
        }

        public override void CreateAABB() {this.aabb = new AABB(this.GetWorldVerts(new Vector3(0,0,0)), this.transform.vPosition);}

        public override void CreateMotionPathAABB() 
        {
            int noVerts = this.verts.Length * 2;
            Vector3[] motionPathVerts = new Vector3[noVerts];
                

            for (int i = 0, count = this.verts.Length; i < count; ++i)
            {
                motionPathVerts[i] = Vector3.Transform(this.verts[i], this.rotMatrix);
            }

            Vector3 position = (this.motion.vVelocityPS * Globals.timeDifference);
            Matrix mZ = Matrix.CreateRotationZ(this.transform.zRotation + (this.motion.zRotPS * Globals.timeDifference));
            for (int i = this.verts.Length, count = motionPathVerts.Length; i < count; ++i)
            {
                motionPathVerts[i] = position + Vector3.Transform(this.verts[i - this.verts.Length], mZ);
            }

            this.motionPathAABB = new AABB(motionPathVerts, this.transform.vPosition);
        }

        public Vector3[] GetWorldVerts(Vector3 position)
        {
            int noVerts = this.verts.Length;
            Vector3[] worldVerts = new Vector3[noVerts];
            for (int i = 0; i < noVerts; ++i)
            {
                worldVerts[i] = position + Vector3.Transform(this.verts[i], this.rotMatrix);
            }
            return worldVerts;
        }

        public void UpdateWorldVerts()
        {
            int noVerts = this.verts.Length;
            this.vertsWorld = new Vector3[noVerts];
            for (int i = 0; i < noVerts; ++i)
            {
                this.vertsWorld[i] = this.transform.vPosition + Vector3.Transform(this.verts[i], this.rotMatrix);
            }
        }
        #endregion



        #region Draw Functions

        public override void Draw(AsteroidGame xnaWindow)
        {
            /* // Reimplement
            if (Globals.DRAWMOTIONPATHAABB && (this.invMass != 0 || Globals.DRAWSTATICMOTIONPATHAABB)) this.motionPathAABB.Draw(xnaWindow, Color.Red);
            if (Globals.DRAWAABB && (this.invMass != 0 || Globals.DRAWSTATICAABB)) this.motionPathAABB.Draw(xnaWindow, Color.Yellow);




            Matrix worldMatrix = Matrix.CreateTranslation(transform.vPosition);

            xnaWindow.effect.CurrentTechnique = xnaWindow.effect.Techniques["Technique_Colored"];
            xnaWindow.effect.Parameters["WorldViewProjMatrix"].SetValue(worldMatrix * xnaWindow.camera.ViewProjMatrix);
            xnaWindow.effect.Parameters["RotationMatrix"].SetValue(Matrix.CreateRotationZ(this.transform.zRotation));
            xnaWindow.effect.Parameters["Scale"].SetValue(new Vector3(1,1,1));


            Color outlineCol = Color.White;
            VertexPositionColor[] vpc = new VertexPositionColor[this.verts.Length];
            short[] indices = new short[(this.verts.Length -2) * 3];


            for (int i = 0; i < this.verts.Length; i++)     //Set Up Vertices
            {
                vpc[i].Position = this.verts[i];
                vpc[i].Color = this.Colour;
            }

            for (int i = 0, j = 0, count = indices.Length; j < count; ++i, j += 3)     //Set up Indices
            {
                indices[j] = 0;
                indices[j + 1] = (short)(i + 1);
                indices[j + 2] = (short)(i + 2);
            }

            if (Globals.FILLRIGIDBODY)
            {
                foreach (EffectPass pass in xnaWindow.effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    xnaWindow.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vpc, 0, vpc.Length, indices, 0, indices.Length / 3, VertexPositionColor.VertexDeclaration);
                }
            }
            else outlineCol = Color.Black;


            // Draw outline
            vpc = new VertexPositionColor[this.verts.Length + 1];
            for (int i = 0; i < this.verts.Length; i++)     //Set Up Vertices
            {
                vpc[i].Position = this.verts[i];
                vpc[i].Color = outlineCol;
            }
            vpc[this.verts.Length].Position = this.verts[0];
            vpc[this.verts.Length].Color = outlineCol;

            foreach (EffectPass pass in xnaWindow.effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                xnaWindow.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, vpc, 0, vpc.Length - 1);
            }
            */
        }

        #endregion



        #region Load Functions

        public override void Load(string fileData)
        {
            base.Load(fileData); //Might not need this line
        }

        #endregion

        #region Save Functions

        public override void Save(StreamWriter sw)
        {
            base.Save(sw);
        }

        #endregion
    }
}
