using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Global_Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Asteroid_Game;

namespace Shapes
{
    public class Line
    {
        private Vector3 lineVector = new Vector3();      //Note: Not world position. Also at half the length (-lineVector is the other half)
        public Vector3 position = new Vector3();        //Center of the line

        public float thickness = 1;
        public Color colourFrom = Color.White;
        public Color colourTo = Color.White;
        public bool flipEdge = false;

        private VertexPositionColor[] vpcArray = new VertexPositionColor[4];

        public Line(Vector3 from, Vector3 to, Color colour, float thickness)
        {
            this.position = (from + to) * 0.5f;
            this.lineVector = to - this.position;

            this.colourFrom = colour;
            this.colourTo = colour;
            this.thickness = thickness;
        }
        public Line(Vector3 from, Vector3 to, Color colourFrom, Color colourTo, float thickness)
        {
            this.position = (from + to) * 0.5f;
            this.lineVector = to - this.position;

            this.colourFrom = colourFrom;
            this.colourTo = colourTo;
            this.thickness = thickness;
        }






        public Vector3 GetLineVector()
        {
            return lineVector;
        }



        public void SetupVpcArray(float cameraZPos)
        {
            Vector3 perpEdge = this.lineVector;
            float temp = perpEdge.X; perpEdge.X = -perpEdge.Y; perpEdge.Y = temp;
            perpEdge.Normalize();


            float distance = cameraZPos - this.lineVector.Z;
            float thick = (distance * thickness) / 5000;


            //vpcArray[0].Position = this.lineVector + this.position - (perpEdge * thick); vpcArray[0].Color = this.colourTo;
            //vpcArray[1].Position = -this.lineVector + this.position - (perpEdge * thick); vpcArray[1].Color = this.colourFrom;
            //vpcArray[2].Position = this.lineVector + this.position + (perpEdge * thick); vpcArray[2].Color = this.colourTo2;
            //vpcArray[3].Position = -this.lineVector + this.position + (perpEdge * thick); vpcArray[3].Color = this.colourFrom2;

            vpcArray[2].Position = this.lineVector + this.position - (perpEdge * thick); vpcArray[0].Color = this.colourFrom;
            vpcArray[0].Position = -this.lineVector + this.position - (perpEdge * thick); vpcArray[1].Color = this.colourTo;
            vpcArray[3].Position = this.lineVector + this.position + (perpEdge * thick); vpcArray[2].Color = this.colourFrom;
            vpcArray[1].Position = -this.lineVector + this.position + (perpEdge * thick); vpcArray[3].Color = this.colourTo;
        }



        public void Draw(AsteroidGame game)
        {
            this.SetupVpcArray(game.camera.CameraPosition.Z);  //It is relative to the camera therefore need to calculate thickness

            
            game.basicEffect.World = Matrix.Identity;        //I have done all its position inside of vpcArray. Not the smartest way tbh.
            game.basicEffect.View = game.camera.ViewMatrix;
            game.basicEffect.TextureEnabled = false;
            game.basicEffect.VertexColorEnabled = true;

            foreach (EffectPass pass in game.basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vpcArray, 0, 2);
            }
        }
    }
}
