using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using _2DLevelCreator;

using Asteroid_Game;
using Global_Data;

namespace Things
{
    public class Thing2D
    {
        //Static Properties
            public  static List<Thing2D> Thing2D_List = new List<Thing2D>();     //Static List to hold any created Thing2D's

            public static void DeleteFromList(Thing2D thing2D)
            {
                int index = thing2D.ID.GetIndexFunction();
                thing2D.list.RemoveAt(index);

                for (int iCount = thing2D.list.Count - 1; iCount >= 0; --iCount)
                { thing2D.list[iCount].ID.UpdateIndex(); }
            }

        //Static Properties



        public int GetMyIndex()
        {
            for (int iCount = this.list.Count - 1; iCount >= 0; --iCount)
                if (this.list[iCount] == this) return iCount;
            return 0;               // returns the default thing2D if no match.
        }
        public UniqueIdentifier FindID(int index)
        {
            if (index >= 0 && index < this.list.Count) return this.list[index].ID;
            return this.ID;    // returns current index.
        }
        public List<Thing2D> list;





        public UniqueIdentifier ID { get; set; }
        public UniqueIdentifier_Reference MaterialID { get; set; } 





        public float Transparency { get; set; } //(CHANGE) This is temporary. Put transparenct in the material class
        public int primitiveCount;                                                  //Number of triangles.(Saves recalculating each Draw function)
        public VertexPositionTextureArray vertexPositionTextureArray { get; set; }  //Holds the vertices and texture coordinates.



        //Constructor
        public Thing2D(string uniqueID, UniqueIdentifier_Reference MaterialID, VertexPositionTextureArray vertexPositionTexture, List<Thing2D> list)
        {
            this.vertexPositionTextureArray = new VertexPositionTextureArray(vertexPositionTexture.VertexPositionTextureWrapper);

            this.MaterialID = MaterialID;



            this.primitiveCount = this.vertexPositionTextureArray.VertexPositionTextureWrapper.Length/3;
            this.Transparency = 1;      //Opengl 0 is transparent 1 is opague. DirectX it is the opposite


            this.list = list;
            this.ID = new UniqueIdentifier(uniqueID, list.Count, this.GetMyIndex, this.FindID);
            list.Add(this);
        }

        //Copy Constructor
        public Thing2D(Thing2D thing2D, List<Thing2D> list)
        {
            this.vertexPositionTextureArray = new VertexPositionTextureArray(thing2D.vertexPositionTextureArray.VertexPositionTextureWrapper);
            this.MaterialID = thing2D.MaterialID;

            this.primitiveCount = this.vertexPositionTextureArray.VertexPositionTextureWrapper.Length/3;
            this.Transparency = 1;      //Opengl 0 is transparent 1 is opague. DirectX it is the opposite


            this.list = list;
            this.ID = new UniqueIdentifier(thing2D.ID.UniqueID, list.Count, this.GetMyIndex, this.FindID);
            list.Add(this);
        }

        //Load Constructor
        public Thing2D(string fileData, List<Thing2D> list)
        {
            this.list = list;
            this.Load(fileData);
            list.Add(this);
        }


        #region Load Functions

        public virtual void Load(string fileData)
        {
            this.ID = new UniqueIdentifier(StringMalarkey.ExtractString(fileData, "UNIQUEID"), list.Count, this.GetMyIndex, this.FindID);
            this.MaterialID = new UniqueIdentifier_Reference(Material.list_Material[0].FindID(Convert.ToInt32(StringMalarkey.ExtractString(fileData, "MATERIAL"))));
            this.vertexPositionTextureArray = StringMalarkey.GetVertPosTexArrayFromString(fileData);

            this.primitiveCount = this.vertexPositionTextureArray.VertexPositionTextureWrapper.Length / 3;
            this.Transparency = 1;      //Opengl 0 is transparent 1 is opague. DirectX it is the opposite
        }

        #endregion




        public void Draw(AsteroidGame game, Transform transform, Matrix rotMatrix)
        {

            Matrix worldMatrix = Matrix.CreateTranslation(transform.vPosition);

            worldMatrix = Matrix.CreateScale(transform.vScale) * rotMatrix * worldMatrix;
            /*      
           Matrix worldMatrix = Matrix.CreateTranslation(transform.vPosition);

           Globals.effect.CurrentTechnique = Globals.effect.Techniques["Technique_Textured"];
           Globals.effect.Parameters["WorldViewProjMatrix"].SetValue(worldMatrix * Globals.camera.ViewProjMatrix);
           Globals.effect.Parameters["RotationMatrix"].SetValue(rotMatrix);
           Globals.effect.Parameters["xTexture"].SetValue(Material.list_Material[this.MaterialID.Index].Texture2D);
           Globals.effect.Parameters["Scale"].SetValue(transform.vScale);
           Globals.effect.Parameters["Transparency"].SetValue(this.Transparency);

           foreach (EffectPass pass in Globals.effect.CurrentTechnique.Passes)
           {
               pass.Apply();
               Globals.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, this.vertexPositionTextureArray.GetVertexPositionTextureArray(), 0, this.primitiveCount);
           }
           Globals.effect.CurrentTechnique = Globals.effect.Techniques["Pretransformed"];

           */

            game.basicEffect.World = worldMatrix;
            game.basicEffect.View = game.camera.ViewMatrix;
            game.basicEffect.Texture = Material.list_Material[this.MaterialID.Index].Texture2D;
            game.basicEffect.TextureEnabled = true;
            game.basicEffect.VertexColorEnabled = false;
            game.basicEffect.Alpha = this.Transparency;

            foreach (EffectPass pass in game.basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, this.vertexPositionTextureArray.GetVertexPositionTextureArray(), 0, this.primitiveCount);
            }
        }
        public void DrawScreenSpace(Effect effect, GraphicsDevice graphicsDevice, Transform transform, Matrix rotMatrix, Camera camera)
        {
            Matrix worldMatrix = Matrix.CreateTranslation(transform.vPosition);

            effect.CurrentTechnique = effect.Techniques["Technique_Textured"];
            effect.Parameters["WorldViewProjMatrix"].SetValue(worldMatrix);
            effect.Parameters["RotationMatrix"].SetValue(Matrix.Identity);
            effect.Parameters["xTexture"].SetValue(Material.list_Material[this.MaterialID.Index].Texture2D);
            effect.Parameters["Scale"].SetValue(transform.vScale * new Vector3(1, graphicsDevice.Viewport.AspectRatio, 1));
            effect.Parameters["Transparency"].SetValue(this.Transparency);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, this.vertexPositionTextureArray.GetVertexPositionTextureArray(), 0, this.primitiveCount);
            }
        }



        public void Highlight(Effect effect, GraphicsDevice graphicsDevice, Transform transform, Matrix rotMatrix, Camera camera)
        {
            Matrix worldMatrix = Matrix.CreateTranslation(transform.vPosition);

            effect.CurrentTechnique = effect.Techniques["Technique_Colored"];
            effect.Parameters["WorldViewProjMatrix"].SetValue(worldMatrix * camera.ViewProjMatrix);
            effect.Parameters["RotationMatrix"].SetValue(Matrix.CreateRotationZ(transform.zRotation));
            effect.Parameters["Scale"].SetValue(transform.vScale);

            VertexPositionColor[] vpc = new VertexPositionColor[this.vertexPositionTextureArray.Length];
            for (int i = 0; i < this.vertexPositionTextureArray.Length; i++)     //Set Up Vertices
            {
                vpc[i].Position = this.vertexPositionTextureArray.VertexPositionTextureWrapper[i].Position;
                vpc[i].Color = new Color(0, 0, 128, 50);
            }


            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, vpc, 0, vpc.Length / 3);
            }
        }



    }
}
