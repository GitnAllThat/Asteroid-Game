using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using _2d_Objects;
using Shapes;
using Asteroid_Game;

namespace _2DLevelCreator
{
    public class VectorHelper
    {
        public static BoundingCircle CreateVertexCollider(Vector3 position, float cameraDepth)
        {
            float radius = 0.0015f;
            radius = ((cameraDepth - position.Z) * radius);

            return new BoundingCircle(new Transform(position, 0, new Vector3(1, 1, 1)), new Motion(), Vector3.Zero, radius, 0, true);
        }

        public static void DrawVertex(Vector3 position, AsteroidGame xnaWindow, Color colour)
        {
            float radius = 0.0015f;
            radius = ((xnaWindow.camera.CameraPosition.Z - position.Z) * radius);

            (new Circle(position, radius, 55, Circle.DrawStyle.Both, colour, Color.White)).Draw(xnaWindow);
        }

        public static List<Vector3> CenterVerticesAroundOriginBoxStyle(List<Vector3> list)
        {
            float top = list[0].Y, bottom = list[0].Y;
            float left = list[0].X, right = list[0].X;

            for (int iCount = (list.Count - 1); iCount >= 1; --iCount)
            {
                if (list[iCount].X > right) right = list[iCount].X;
                if (list[iCount].X < left) left = list[iCount].X;
                if (list[iCount].Y > top) top = list[iCount].Y;
                if (list[iCount].Y < bottom) bottom = list[iCount].Y;
            }

            Vector3 center = new Vector3((left + right) * 0.5f, (top + bottom) * 0.5f, 0);

            for (int iCount = list.Count - 1; iCount >= 0; --iCount) list[iCount] -= center;

            return list;
        }

        public static List<Vector3> RemoveDuplicates(List<Vector3> list)
        {
            for (int iCount = list.Count - 1; iCount >= 0; --iCount)
            {
                for (int jCount = iCount - 1; jCount >= 0; --jCount)
                {
                    if (list[iCount] == list[jCount])
                    {
                        list.RemoveAt(iCount);
                        break;
                    }
                }
            }
            return list;
        }

        public static List<Vector3> MakeClockwise(List<Vector3> list)
        {
            Vector3 center = VectorHelper.GetCenter(list);
            for (int iCount = 0, iCountMax = list.Count; iCount < iCountMax; ++iCount)
            {
                float angle1 = VectorHelper.GetAngleBetweenVectorDirections(center, list[iCount]) + 90;
                float angle2 = VectorHelper.GetAngleBetweenVectorDirections(center, list[(iCount + 1) % iCountMax]) + 90;

                if (angle1 < 0) angle1 += 360;
                if (angle2 < 0) angle2 += 360;

                if (angle1 > angle2)
                {
                    Vector3 temp = list[iCount];
                    list[iCount] = list[(iCount + 1) % iCountMax];
                    list[(iCount + 1) % iCountMax] = temp;
                    iCount = 0;
                }
            }

            return list;
        }

        public static float GetAngleBetweenVectorDirections(Vector3 v1, Vector3 v2)
        {
            float angle1 = (float)MathHelper.ToDegrees((float)Math.Atan2(v1.Y, v1.X));
            float angle2 = (float)MathHelper.ToDegrees((float)Math.Atan2(v2.Y, v2.X));
            if (angle1 < 0) angle1 += 360;
            if (angle2 < 0) angle2 += 360;

            return angle1 - angle2;
        }

        public static Vector3 GetCenter(List<Vector3> list)
        {
            Vector3 center = Vector3.Zero;
            for (int iCount = list.Count - 1; iCount >= 0; --iCount)
                center += list[iCount];

            return (center /= list.Count);
        }

        public static Vector3 GetCenter(Vector3[] array)
        {
            Vector3 center = Vector3.Zero;
            for (int iCount = array.Length - 1; iCount >= 0; --iCount)
                center += array[iCount];

            return (center /= array.Length);
        }

        public static List<Vector3> ToList(Vector3[] array)
        {
            List<Vector3> list = new List<Vector3>();

            for (int iCount = 0, iCountMax = array.Length; iCount < iCountMax; ++iCount)
            { list.Add(array[iCount]); }

            return list;
        }

        public static Vector3[] ToArray(List<Vector3> list)
        {
            Vector3[] array = new Vector3[list.Count];

            for (int iCount = 0, iCountMax = list.Count; iCount < iCountMax; ++iCount)
            { array[iCount] = list[iCount]; }

            return array;
        }
    }



    public class VertexPositionTextureWrapper
    {
        #region Data Variables

        VertexPositionTexture _vpt;
        public VertexPositionTexture vpt
        {
            get { return this._vpt; }
            set { this._vpt = value; }
        }

        private Vector3 _position;
        public Vector3 Position
        {
            get { return this._position; }
            set
            {
                this._position = value;
                this._vpt.Position = value;
            }
        }

        private Vector2 _textureCoordinate;
        public Vector2 TextureCoordinate
        {
            get { return this._textureCoordinate; }
            set
            {
                this._textureCoordinate = value;
                this._vpt.TextureCoordinate = value;
            }
        }

        #endregion

        #region Constructors

        public VertexPositionTextureWrapper()
        {
            _position = Vector3.Zero;
            _textureCoordinate = Vector2.Zero;
            vpt = new VertexPositionTexture(_position, _textureCoordinate);
        }

        public VertexPositionTextureWrapper(Vector3 position, Vector2 texCoord)
        {
            _position = position;
            _textureCoordinate = texCoord;
            vpt = new VertexPositionTexture(_position, _textureCoordinate);
        }

        //Copy Constructor
        public VertexPositionTextureWrapper(VertexPositionTextureWrapper vPTW)
        {
            _position = vPTW.Position;
            _textureCoordinate = vPTW.TextureCoordinate;
            vpt = new VertexPositionTexture(_position, _textureCoordinate);
        }

        #endregion
    }



    public class VertexPositionTextureArray
    {

        #region Data Members

        private VertexPositionTextureWrapper[] _VertexPositionTextureWrapper;
        public VertexPositionTextureWrapper[] VertexPositionTextureWrapper
        {
            get { return this._VertexPositionTextureWrapper; }
            set { this._VertexPositionTextureWrapper = value; }
        }

        public int Length
        {
            get { return this._VertexPositionTextureWrapper.Length; }
            set { }
        }

        #endregion

        #region Constructors

        //Default Constructor
        public VertexPositionTextureArray()
        {
            this.VertexPositionTextureWrapper =
                new VertexPositionTextureWrapper[6] {   (new VertexPositionTextureWrapper(new Vector3(-0.5f, 0.5f, 0), new Vector2(0, 0))),
                                                        (new VertexPositionTextureWrapper(new Vector3(0.5f, 0.5f, 0), new Vector2(1, 0))),
                                                        (new VertexPositionTextureWrapper(new Vector3(-0.5f, -0.5f, 0), new Vector2(0, 1))),
                                                        (new VertexPositionTextureWrapper(new Vector3(-0.5f, -0.5f, 0), new Vector2(0, 1))),
                                                        (new VertexPositionTextureWrapper(new Vector3(0.5f, 0.5f, 0), new Vector2(1, 0))),
                                                        (new VertexPositionTextureWrapper(new Vector3(0.5f, -0.5f, 0), new Vector2(1, 1)))};
        }

        //Copy Constructor
        public VertexPositionTextureArray(VertexPositionTextureWrapper[] vertexPositionTextureWrapper)
        {
            this.VertexPositionTextureWrapper = new VertexPositionTextureWrapper[vertexPositionTextureWrapper.Length];
            for (int iCount = 0, iCountMax = this.VertexPositionTextureWrapper.Length; iCount < iCountMax; ++iCount)
            { this.VertexPositionTextureWrapper[iCount] = new VertexPositionTextureWrapper(vertexPositionTextureWrapper[iCount]); }
        }

        #endregion

        public VertexPositionTexture[] GetVertexPositionTextureArray()
        {
            VertexPositionTexture[] VertexPositionTextureArray = new VertexPositionTexture[_VertexPositionTextureWrapper.Length];
            for (int iCount = 0, iCountMax = _VertexPositionTextureWrapper.Length; iCount < iCountMax; ++iCount)
            {
                VertexPositionTextureArray[iCount] = new VertexPositionTexture(this._VertexPositionTextureWrapper[iCount].Position, this._VertexPositionTextureWrapper[iCount].TextureCoordinate);
            }
            return VertexPositionTextureArray;
        }




        #region Static Functions

        public static List<Vector3> GetVerticePositions(VertexPositionTextureArray vpta)
        {
            List<Vector3> list = new List<Vector3>();
            for (int iCount = 0, iCountMax = vpta.Length; iCount < iCountMax; ++iCount)
            {
                list.Add(vpta.VertexPositionTextureWrapper[iCount].Position);
            }
            return list;
        }

        public static Vector3 GetVertexPosCenter(VertexPositionTextureArray vpta)
        {
            Vector3 center = Vector3.Zero;

            for (int iCount = 0, iCountMax = vpta.Length; iCount < iCountMax; ++iCount) { center += vpta.VertexPositionTextureWrapper[iCount].Position; }

            center /= vpta.Length;

            return center;
        }

        public static Vector3 GetVertexPosCenter(List<VertexPositionTextureWrapper> list_vptw)
        {
            Vector3 center = Vector3.Zero;

            for (int iCount = 0, iCountMax = list_vptw.Count; iCount < iCountMax; ++iCount) { center += list_vptw[iCount].Position; }

            center /= list_vptw.Count;

            return center;
        }

        public static Vector2 GetTexCoordCenter(VertexPositionTextureArray vpta)
        {
            Vector2 center = Vector2.Zero;

            for (int iCount = 0, iCountMax = vpta.Length; iCount < iCountMax; ++iCount)
            {
                center += vpta.VertexPositionTextureWrapper[iCount].TextureCoordinate;
            }
            center /= vpta.Length;

            return center;
        }

        public static Vector2 GetTexCoordCenter(List<VertexPositionTextureWrapper> list_vptw)
        {
            Vector2 center = Vector2.Zero;

            for (int iCount = 0, iCountMax = list_vptw.Count; iCount < iCountMax; ++iCount) { center += list_vptw[iCount].TextureCoordinate; }

            center /= list_vptw.Count;

            return center;
        }

        public static Vector2 ConvertToTextureCoordSpace(Vector3 vector)
        {
            vector.Y *= -1;
            return new Vector2(vector.X, vector.Y);
        }

        public static Vector3 ConvertTextureCoordSpaceToWorld(Vector2 vector)
        {
            vector.Y *= -1;
            return new Vector3(vector, 0);
        }

        public static void CenterVerticesAroundOriginBoxStyle(VertexPositionTextureArray vpta)
        {
            List<Vector3> list = VertexPositionTextureArray.GetVerticePositions(vpta);

            VectorHelper.CenterVerticesAroundOriginBoxStyle(list);

            for (int iCount = 0, iCountMax = vpta.VertexPositionTextureWrapper.Length; iCount < iCountMax; ++iCount)
            {
                vpta.VertexPositionTextureWrapper[iCount].Position = list[iCount];
            }
        }

        #endregion


        #region Removing Entry From Array

        public void RemoveEntries(List<VertexPositionTextureWrapper> list)
        {
            for (int iCount = 0, iCountMax = this.Length; iCount < iCountMax; ++iCount)
            {
                if (this.VertexPositionTextureWrapper[iCount] == null) continue;
                if (CountNotNullEntries() == 3) continue;


                for (int jCount = 0, jCountMax = list.Count; jCount < jCountMax; ++jCount)
                {
                    //if (list[jCount] == null) continue;

                    if (list[jCount] == this.VertexPositionTextureWrapper[iCount])
                    {
                        DeleteFace(iCount);
                        list.RemoveAt(jCount);
                        break;
                    }
                }
            }

            //Clean up VertexPositionTextureWrapper
            list.Clear();
            RemoveNullEntries();
        }


        private void DeleteFace(int index)
        {
            for (int iCount = 0, iCountMax = this.Length; iCount < iCountMax; iCount += 3)
            {
                if (index == iCount || index == (iCount + 1) | index == (iCount + 2))
                {
                    this.VertexPositionTextureWrapper[iCount] = null;
                    this.VertexPositionTextureWrapper[iCount + 1] = null;
                    this.VertexPositionTextureWrapper[iCount + 2] = null;
                }
            }
        }

        private void RemoveNullEntries()
        {
            int arrayCount = 0;

            for (int iCount = 0, iCountMax = this.Length; iCount < iCountMax; ++iCount)
            {
                if (this.VertexPositionTextureWrapper[iCount] != null) ++arrayCount;
            }

            VertexPositionTextureWrapper[] newVPTW = new VertexPositionTextureWrapper[arrayCount];

            for (int iCount = 0, jCount = 0, iCountMax = this.Length; iCount < iCountMax; ++iCount)
            {
                if (this.VertexPositionTextureWrapper[iCount] != null)
                    newVPTW[jCount++] = new VertexPositionTextureWrapper(this.VertexPositionTextureWrapper[iCount]);
            }

            this.VertexPositionTextureWrapper = null;
            this.VertexPositionTextureWrapper = newVPTW;
        }

        private int CountNotNullEntries()
        {
            int count = 0;

            for (int iCount = 0, iCountMax = this.VertexPositionTextureWrapper.Length; iCount < iCountMax; ++iCount)
            { if (this.VertexPositionTextureWrapper[iCount] != null) ++count; }

            return count;
        }

        #endregion
    }




}
