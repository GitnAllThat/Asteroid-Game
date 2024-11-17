using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Asteroid_Game;

namespace Shapes
{
    public class Circle
    {
        public enum DrawStyle : int { Fill, Outline, Both };

        #region Static Functions

        public static Vector3[] GenerateVerts(int vertCount, float radius)
        {
            Vector3[] verts = new Vector3[vertCount];

            for (int iCount = 0; iCount < vertCount; iCount++)
            {
                float angle = -(float)(((float)iCount / vertCount) * (Math.PI * 2)) + MathHelper.ToRadians(90);
                verts[iCount] = new Vector3(((float)Math.Cos(angle) * radius), ((float)Math.Sin(angle) * radius), 0);
            }
            return verts;
        }

        #endregion


        #region Data Members

        public Vector3 vPosition { get; set; }
        private float Radius{ get; set; }
        private int Precision { get; set; }
        private Color Colour { get; set; }
        private Color OutlineColour { get; set; }
        private bool FillCircle { get; set; }
        private bool HasOutline { get; set; }

        private VertexPositionColor[] vpcArray;
        private short[] indices;

        #endregion


        #region Constructors

        /// <summary>
        ///  Circle Constructor
        /// </summary>
        /// <param name="position"> The Circle's Position</param>
        /// <param name="radius"> Circle Radius</param>
        /// <param name="precision"> How many triangles(or lines) used to create the circle</param>
        /// <param name="fillCircle"> Whether to fill the circle or not</param>
        /// <param name="colour"> General Colour of the Circle</param>
        public Circle(Vector3 position, float radius, int precision, DrawStyle drawstyle, Color colour, Color colourOutline)
        {
            this.vPosition = position;
            this.Radius = radius;
            this.Colour = colour;
            this.OutlineColour = colourOutline;

            this.HandleDrawStyle(drawstyle);

            this.Precision = Math.Max(3, Math.Min(150, precision));

            this.SetupVerts();
            this.SetUpIndices();
        }

        public Circle(Vector3 position, float radius, DrawStyle drawstyle, Color colour, Color colourOutline)
        {
            this.vPosition = position;
            this.Radius = radius;
            this.Colour = colour;
            this.OutlineColour = colourOutline;

            this.HandleDrawStyle(drawstyle);

            this.Precision = 50;

            this.SetupVerts();
            this.SetUpIndices();
        }

        #endregion


        #region Functions

        private void SetVpaColours(Color colour)
        {
            for (int i = 0; i < vpcArray.Length; i++)
                vpcArray[i].Color = colour;
        }

        private void HandleDrawStyle(DrawStyle drawstyle)
        {
            if (drawstyle == DrawStyle.Both)
            {
                this.FillCircle = true;
                this.HasOutline = true;
            }
            else if (drawstyle == DrawStyle.Fill)
            {
                this.FillCircle = true;
                this.HasOutline = false;
            }
            else if (drawstyle == DrawStyle.Outline)
            {
                this.FillCircle = false;
                this.HasOutline = true;
            }
        }

        public void SetRadius(float radius)
        {
            this.Radius = radius;
            this.SetupVerts();
        }

        public void SetPrecision(int precision)
        {
            this.Precision = Math.Max(3, Math.Min(150, precision));
            this.SetupVerts();
            this.SetUpIndices();
        }

        private void SetupVerts()
        {
            Vector3[] vertices = Circle.GenerateVerts(this.Precision, this.Radius);

            vpcArray = new VertexPositionColor[this.Precision + 2];    //An extra to rejoin the circle(for outline) and another for the midpoint(nicer drawing).

            for (int i = 0; i < this.Precision; i++)
                vpcArray[i] = new VertexPositionColor(vertices[i], this.Colour);

            vpcArray[this.Precision] = vpcArray[0];
            vpcArray[this.Precision + 1] = new VertexPositionColor(Vector3.Zero, this.Colour);
        }

        private void SetUpIndices()
        {
            this.indices = new short[this.Precision * 3];

            for (int i = 1, j = 0; i <= this.Precision; i++)
            {
                indices[j++] = 0;
                indices[j++] = (short)i;
                indices[j++] = (short)(i + 1);
            }
            indices[(this.Precision * 3) - 1] = 1;
        }
        #endregion

        #region Draw Malarkey

        public void Draw(AsteroidGame monogameWindow)
        {
            /* //Reimplement
            Matrix worldMatrix = Matrix.CreateTranslation(this.vPosition);

            monogameWindow.effect.CurrentTechnique = monogameWindow.effect.Techniques["Technique_Colored"];
            monogameWindow.effect.Parameters["WorldViewProjMatrix"].SetValue(worldMatrix * monogameWindow.camera.ViewProjMatrix);
            monogameWindow.effect.Parameters["RotationMatrix"].SetValue(Matrix.Identity);
            monogameWindow.effect.Parameters["Scale"].SetValue(new Vector3(1, 1, 1));


            if (this.FillCircle)
            {
                this.SetVpaColours(this.Colour);
                foreach (EffectPass pass in monogameWindow.effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    monogameWindow.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, this.vpcArray, 0, this.vpcArray.Length, indices, 0, indices.Length / 3, VertexPositionColor.VertexDeclaration);
                }
            }

            if (this.HasOutline)
            {
                this.SetVpaColours(this.OutlineColour);
                foreach (EffectPass pass in monogameWindow.effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    monogameWindow.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, vpcArray, 0, vpcArray.Length - 2);
                }
            }
            */
        }

        public void DrawScreenSpace(AsteroidGame monogameWindow)
        {
            /* //Reimplement
            Matrix worldMatrix = Matrix.CreateTranslation(this.vPosition);

            monogameWindow.effect.CurrentTechnique = monogameWindow.effect.Techniques["Technique_Colored"];
            monogameWindow.effect.Parameters["WorldViewProjMatrix"].SetValue(worldMatrix);
            monogameWindow.effect.Parameters["RotationMatrix"].SetValue(Matrix.Identity);
            monogameWindow.effect.Parameters["Scale"].SetValue(new Vector3(1, 1, 1));

            this.SetVpaColours(this.Colour);
            foreach (EffectPass pass in monogameWindow.effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                monogameWindow.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, this.vpcArray, 0, this.vpcArray.Length, indices, 0, indices.Length / 3, VertexPositionColor.VertexDeclaration);
            }
            */
        }

        #endregion
    }
}
