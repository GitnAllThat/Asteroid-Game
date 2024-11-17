using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shapes;


namespace Asteroid_Game
{
    public class RainbowLine
    {
        public static Random rand = new Random();

        public Vector3 halfLineVector = new Vector3();      //Note: Not world position. Also at half the length (-halfLineVector is the other half)
        public Vector3 position = new Vector3();            //Center of the line

        public int thickness = 1;
        public Color colourFrom = Color.White;
        public Color colourFrom2 = Color.White;
        public Color colourTo = Color.White;
        public Color colourTo2 = Color.White;

        public VertexPositionColor[] vpcArray = new VertexPositionColor[4];
        public Vector3 normal = new Vector3(0, 1, 0);

        public List<Circle> listCircles = new List<Circle>();
        public int noCircles = 14;

        public RainbowLine(Vector3 from, Vector3 to, Color colourFrom, Color colourTo, int thickness)
        {
            this.position = (from + to) * 0.5f;
            this.halfLineVector = to - this.position;


            this.colourFrom2 = colourFrom;
            this.colourTo2 = colourFrom;
            this.colourFrom = colourTo;
            this.colourTo = colourTo;
            this.thickness = thickness;

            this.SetupVpcArray(150);
            this.SetupCircles();
        }
        public RainbowLine(Vector3 from, Vector3 to, Color colourFrom, Color colourFrom2, Color colourTo, Color colourTo2, int thickness)
        {
            this.position = (from + to) * 0.5f;
            this.halfLineVector = to - this.position;

            this.colourFrom2 = colourFrom;
            this.colourTo2 = colourFrom2;
            this.colourFrom = colourTo;
            this.colourTo = colourTo2;
            this.thickness = thickness;

            this.SetupVpcArray(150);
            this.SetupCircles();
        }


        // Copy constructor.
        public RainbowLine(RainbowLine rainbowLine)
        {
            this.halfLineVector = rainbowLine.halfLineVector;      //Note: Not world position. Also at half the length (-lineVector is the other half)
            this.position = rainbowLine.position;        //Center of the line

            this.thickness = rainbowLine.thickness;
            this.colourFrom2 = rainbowLine.colourFrom2;
            this.colourTo2 = rainbowLine.colourTo2;
            this.colourFrom = rainbowLine.colourFrom;
            this.colourTo = rainbowLine.colourTo;

            this.vpcArray = (VertexPositionColor[])rainbowLine.vpcArray.Clone(); ;
            this.normal = rainbowLine.normal;

            this.listCircles = rainbowLine.listCircles;
            this.noCircles = rainbowLine.noCircles;
        }












        public void SetNormal()
        {
            this.normal = this.halfLineVector;
            float temp = this.normal.X; this.normal.X = -this.normal.Y; this.normal.Y = temp;
            this.normal.Normalize();
        }

        public Vector3 GetLineVector()
        {
            return halfLineVector;
        }

        public void SetupCircles()
        {
            float invPercentage = 1 / (((float)this.noCircles - 1));
            float nextInvPercentage = 0;
            Vector3 line = this.halfLineVector * 2;
            Vector3 lineNorm = this.normal * 2;
            Vector3 pos = this.position - this.halfLineVector;
            for (int i = 0, iMax = (int)noCircles; i < iMax; ++i)
            {
                Vector3 posOnLine = (pos + (line * nextInvPercentage));
                Vector3 normPosOnLine = posOnLine - (this.normal);
                Vector3 percentOfNormal = lineNorm * ((float)RainbowLine.rand.Next(1, 100)) * 0.01f;

                listCircles.Add(new Circle(normPosOnLine + percentOfNormal, 0.005f, Circle.DrawStyle.Both, Color.Red, Color.White));
                nextInvPercentage += invPercentage;
            }
        }




        public void SetupVpcArray(float cameraZPos)
        {
            this.SetNormal();


            float distance = cameraZPos - this.halfLineVector.Z;
            float thick = (distance * thickness) / 5000;


            //vpcArray[0].Position =  this.halfLineVector + this.position - (this.normal * thick); vpcArray[0].Color = this.colourTo;
            //vpcArray[1].Position = -this.halfLineVector + this.position - (this.normal * thick); vpcArray[1].Color = this.colourFrom;
            //vpcArray[2].Position =  this.halfLineVector + this.position + (this.normal * thick); vpcArray[2].Color = this.colourTo2;
            //vpcArray[3].Position = -this.halfLineVector + this.position + (this.normal * thick); vpcArray[3].Color = this.colourFrom2;
            
            //NOTE: the to and from colours are switched. put "* 5" after "* thick" to see.

            vpcArray[2].Position =  this.halfLineVector + this.position - (this.normal * thick); vpcArray[2].Color = this.colourTo2; 
            vpcArray[0].Position = -this.halfLineVector + this.position - (this.normal * thick); vpcArray[0].Color = this.colourFrom2;
            vpcArray[3].Position =  this.halfLineVector + this.position + (this.normal * thick); vpcArray[3].Color = this.colourTo;
            vpcArray[1].Position = -this.halfLineVector + this.position + (this.normal * thick); vpcArray[1].Color = this.colourFrom;

            //vpcArray[0].Color.A = 155;
            //vpcArray[1].Color.A = 155;
            //vpcArray[2].Color.A = 155;
            //vpcArray[3].Color.A = 155;
        }



        public void ShortenFromBack(float shortenPercentage)
        {
            if (shortenPercentage > 1) shortenPercentage = 1;
            if (shortenPercentage < 0) { shortenPercentage = 0; return; }



            Vector3 from = this.position - this.halfLineVector;
            Vector3 to = this.position + this.halfLineVector;


            from += (to - from) * shortenPercentage;
            //nF = F + (P(T - F))
            Color newColourFrom = new Color((int)(this.colourFrom.R + ((this.colourTo.R - this.colourFrom.R) * shortenPercentage)), (int)(this.colourFrom.G + ((this.colourTo.G - this.colourFrom.G) * shortenPercentage)), (int)(this.colourFrom.B + ((this.colourTo.B - this.colourFrom.B) * shortenPercentage)));
            Color newColourFrom2 = new Color((int)(this.colourFrom2.R + ((this.colourTo2.R - this.colourFrom2.R) * shortenPercentage)), (int)(this.colourFrom2.G + ((this.colourTo2.G - this.colourFrom2.G) * shortenPercentage)), (int)(this.colourFrom2.B + ((this.colourTo2.B - this.colourFrom2.B) * shortenPercentage)));



            this.position = (from + to) * 0.5f;
            this.halfLineVector = to - this.position;
            this.colourFrom  = newColourFrom;
            this.colourFrom2 = newColourFrom2;
            this.SetupVpcArray(150);
        }

        public void ShortenFromFront(float shortenPercentage)
        {
            if (shortenPercentage > 1) shortenPercentage = 1;
            if (shortenPercentage < 0) { shortenPercentage = 0; return; }



            Vector3 from = this.position - this.halfLineVector;
            Vector3 to = this.position + this.halfLineVector;


            to += (from - to) * shortenPercentage;
            //nT = T + (P(F - T))
            Color newColourTo = new Color((int)(this.colourTo.R + ((this.colourFrom.R - this.colourTo.R) * shortenPercentage)), (int)(this.colourTo.G + ((this.colourFrom.G - this.colourTo.G) * shortenPercentage)), (int)(this.colourTo.B + ((this.colourFrom.B - this.colourTo.B) * shortenPercentage)));
            Color newColourTo2 = new Color((int)(this.colourTo2.R + ((this.colourFrom2.R - this.colourTo2.R) * shortenPercentage)), (int)(this.colourTo2.G + ((this.colourFrom2.G - this.colourTo2.G) * shortenPercentage)), (int)(this.colourTo2.B + ((this.colourFrom2.B - this.colourTo2.B) * shortenPercentage)));



            this.position = (from + to) * 0.5f;
            this.halfLineVector = to - this.position;
            this.colourTo  = newColourTo;
            this.colourTo2 = newColourTo2;
            this.SetupVpcArray(150);
        }















        public void Draw(AsteroidGame game)
        {

            foreach (EffectPass pass in game.basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vpcArray, 0, 2);
                //xnaWindow.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, vpcArray, 0, 2); // Makes an interesting effect


                (new Line(vpcArray[2].Position, vpcArray[0].Position, Color.White, 1.9f)).Draw(game);
                (new Line(vpcArray[3].Position, vpcArray[1].Position, Color.White, 1.9f)).Draw(game);
            }


            /*
            xnaWindow.spriteBatch.Begin();
            xnaWindow.spriteBatch.Draw(new Texture2D(xnaWindow.GraphicsDevice, 1, 1, false, SurfaceFormat.Color), new Rectangle(100, 100, 1000, 1), Color.Blue);
            xnaWindow.spriteBatch.End();
            */
        }
    }
}
