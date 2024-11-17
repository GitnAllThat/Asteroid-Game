
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Asteroid_Game;

namespace _2d_Objects
{
    public class AABB 
    {
        public float HalfWidth  { get; set; }
        public float HalfHeight { get; set; }

        public Vector3 vPosition; //offset relative to the rigid body


        public AABB(Vector3 vPosition,float halfHeight, float halfWidth)
        {
            this.HalfWidth = halfWidth;
            this.HalfHeight = halfHeight;
            this.vPosition = vPosition;
        }

        public AABB(Vector3[] vertsOS, Vector3 vPos) 
        {
            float left = vertsOS[0].X, right = vertsOS[0].X, up = vertsOS[0].Y, down = vertsOS[0].Y;
            for (int i = 1, numVert = vertsOS.Length; i < numVert; ++i)
            {
                if      (vertsOS[i].X < left) left = vertsOS[i].X;
                else if (vertsOS[i].X > right) right = vertsOS[i].X;

                if      (vertsOS[i].Y > up) up = vertsOS[i].Y;
                else if (vertsOS[i].Y < down) down = vertsOS[i].Y;
            }
            this.HalfWidth = (right - left) * 0.5f;
            this.HalfHeight = (up - down) * 0.5f;

            Vector3 offset = new Vector3(left + this.HalfWidth, down + this.HalfHeight, 0);  //(CHANGE) could facter this var away
            this.vPosition = vPos + offset;
        }

        /// <summary>
        /// Check if a Point intersects an AABB.
        /// Works faster when x and y are tested separately. (Can factor out unnecesary calculations)
        /// </summary>
        /// <returns> Returns True if intersection is present </returns>
        public static bool AABBIntersectAABB(AABB aabbA, AABB aabbB)
        {
            float axis, dimension;
            axis = aabbA.vPosition.X - aabbB.vPosition.X;
            dimension = aabbA.HalfWidth + aabbB.HalfWidth;
            if (axis <= dimension && axis >= -dimension)
            {
                axis = aabbA.vPosition.Y - aabbB.vPosition.Y;
                dimension = aabbA.HalfHeight + aabbB.HalfHeight;
                if (axis <= dimension && axis >= -dimension)
                    return true;
            }
            return false;
        }
        
        /// <summary>
        /// Check if a Point intersects an AABB.
        /// Works faster when x and y are tested separately. (Can factor out unnecesary calculations)
        /// </summary>
        /// <returns> True if Intersection</returns>
        public static bool PointIntersectAABB(AABB aabb, Vector3 position)
        {
            position.X -= aabb.vPosition.X;
            if (position.X <= aabb.HalfWidth && position.X >= -aabb.HalfWidth)
            {
                position.Y -= aabb.vPosition.Y;
                if (position.Y <= aabb.HalfHeight && position.Y >= -aabb.HalfHeight)
                    return true;
            }
            return false;
        }

        public static AABB CombineAABBs(AABB aabb1, AABB aabb2)
        {
            float left = (aabb1.vPosition.X - aabb1.HalfWidth < aabb2.vPosition.X - aabb2.HalfWidth) ? aabb1.vPosition.X - aabb1.HalfWidth : aabb2.vPosition.X - aabb2.HalfWidth;
            float right = (aabb1.vPosition.X + aabb1.HalfWidth > aabb2.vPosition.X + aabb2.HalfWidth) ? aabb1.vPosition.X + aabb1.HalfWidth : aabb2.vPosition.X + aabb2.HalfWidth;
            float bottom = (aabb1.vPosition.Y - aabb1.HalfHeight < aabb2.vPosition.Y - aabb2.HalfHeight) ? aabb1.vPosition.Y - aabb1.HalfHeight : aabb2.vPosition.Y - aabb2.HalfHeight;
            float top = (aabb1.vPosition.Y + aabb1.HalfHeight > aabb2.vPosition.Y + aabb2.HalfHeight) ? aabb1.vPosition.Y + aabb1.HalfHeight : aabb2.vPosition.Y + aabb2.HalfHeight;

            return new AABB(new Vector3((left + right) * 0.5f, (top + bottom) * 0.5f, 0), ((top - bottom) * 0.5f), ((right - left) * 0.5f));
        }





        public void Draw(AsteroidGame xnaWindow, Color colour)
        {
            /* //Reimplement
            Matrix worldMatrix = Matrix.CreateTranslation(this.vPosition);

            xnaWindow.effect.CurrentTechnique = xnaWindow.effect.Techniques["Technique_Colored"];
            xnaWindow.effect.Parameters["WorldViewProjMatrix"].SetValue(worldMatrix * xnaWindow.camera.ViewProjMatrix);
            xnaWindow.effect.Parameters["RotationMatrix"].SetValue(Matrix.CreateRotationZ(0));
            xnaWindow.effect.Parameters["Scale"].SetValue(new Vector3(1, 1, 1));


            VertexPositionColor[] vpc = new VertexPositionColor[5];
            vpc[0].Color = vpc[1].Color = vpc[2].Color = vpc[3].Color = vpc[4].Color = colour;

            vpc[0].Position = new Vector3(-this.HalfWidth, this.HalfHeight, 0);
            vpc[1].Position = new Vector3(this.HalfWidth, this.HalfHeight, 0);
            vpc[2].Position = new Vector3(this.HalfWidth, -this.HalfHeight, 0);
            vpc[3].Position = new Vector3(-this.HalfWidth, -this.HalfHeight, 0);
            vpc[4].Position = vpc[0].Position;



            foreach (EffectPass pass in xnaWindow.effect.CurrentTechnique.Passes)
            {

                pass.Apply();

                xnaWindow.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, vpc, 0, 4);
            }
            */
        }
    }
}
