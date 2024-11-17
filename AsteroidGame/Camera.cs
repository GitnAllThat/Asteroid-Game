using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Asteroid_Game;

namespace _2DLevelCreator
{
    public class Camera
    {
        private Matrix world_Matrix;
        private Matrix view_Matrix;
        private Matrix projection_Matrix;
        private Matrix viewProj_Matrix;
        private Matrix inv_World_Matrix;
        private Matrix inv_View_Matrix;
        private Matrix inv_Projection_Matrix;

        private Vector3 camera_Position;

        private float leftRight_Rotation;
        private float upDown_Rotation;

        public Vector3 upVector { get; set; }

        public Vector3 CameraPosition
        {
            get { return this.camera_Position; }
            set { this.camera_Position = value; this.UpdateViewMatrix(); }
        }

        public float LeftRight_Rotation
        {
            get { return this.leftRight_Rotation; }
            set { this.leftRight_Rotation = value; }
        }

        public float UpDown_Rotation
        {
            get { return this.upDown_Rotation; }
            set { this.upDown_Rotation = value; }
        }




        public Matrix WorldMatrix { get { return this.world_Matrix; } }
        public Matrix ViewMatrix { get { return this.view_Matrix; } }
        public Matrix ProjectionMatrix { get { return this.projection_Matrix; } }
        public Matrix InvWorldMatrix { get { return this.inv_World_Matrix; } }
        public Matrix InvViewMatrix { get { return this.inv_View_Matrix; } }
        public Matrix InvProjectionMatrix { get { return this.inv_Projection_Matrix; } }
        public Matrix ViewProjMatrix { get { return this.viewProj_Matrix; } }






        //Constructor
        public Camera(float aspectRation)
        {
            upVector = new Vector3(0, 1, 0);
            SetupCamera(aspectRation);
        }

        public void SetupCamera(float aspectRation)
        {
            //aspectRation = 2; //(WARNING) Debug MAKES ALL IMAGES TO SCALE
            this.projection_Matrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.Pi/20, aspectRation, 0.3f, 10000.0f);

            this.inv_Projection_Matrix = Matrix.Invert(projection_Matrix);

            this.UpdateViewMatrix();
        }

        public Matrix GetRotationMatrix()
        {
            return Matrix.CreateRotationX(this.UpDown_Rotation) * Matrix.CreateRotationY(this.LeftRight_Rotation);
        }



        public void CameraMovement(Vector3 vDirection, float timeElapsed)    //This will be removed and should be done inside the player class when it is created
        {
            Matrix rotation = GetRotationMatrix();
            Vector3 rotatedVector = Vector3.Transform(vDirection, rotation);

            this.CameraPosition += (rotatedVector * timeElapsed);
        }

        public void UpdateViewMatrix()
        {
            Matrix cameraRotation = GetRotationMatrix();

            Vector3 cameraRotatedTarget = Vector3.Transform(new Vector3(0, 0, -1), cameraRotation);
            Vector3 cameraFinalTarget = camera_Position + cameraRotatedTarget;

            Vector3 cameraRotatedUpVector = Vector3.Transform(upVector, cameraRotation);

            this.view_Matrix = Matrix.CreateLookAt(camera_Position, cameraFinalTarget, cameraRotatedUpVector);
            this.inv_View_Matrix = Matrix.Invert(view_Matrix);

            this.world_Matrix = Matrix.CreateTranslation(this.CameraPosition);
            this.inv_World_Matrix = Matrix.Invert(world_Matrix);
            this.viewProj_Matrix = this.view_Matrix * this.projection_Matrix;
        }











        public void CameraMovement(GameInput gameInput, AsteroidGame monoGameWindow)
        {
            #region Camera Zoom

            //Camera Zoom by Mousewheel.
            float zoomAmount = (gameInput.mouseStatePrevious.ScrollWheelValue - gameInput.mouseStateCurrent.ScrollWheelValue) * 0.1f;
            if (zoomAmount != 0)
            {
                float zDistance = this.CameraPosition.Z + zoomAmount;

                if (zDistance > -11 && zDistance < 11) zoomAmount = 0;


                Vector3 cameraCenter = CalculateCameraCenter(monoGameWindow, this.CameraPosition.Z + zoomAmount);
                this.CameraPosition = cameraCenter;
            }



            //Camera Zoom by Mouse(Maya Style).
            if (gameInput.isMouseOnScreen && gameInput.mouseStateCurrent.RightButton == ButtonState.Pressed && gameInput.keyboardStateCurrent.IsKeyDown(Keys.LeftAlt))
            {
                float distance = Vector3.Distance(monoGameWindow.gameInput.GetWorldMouseCoords_XYPlane(this, gameInput.mousePositionCurrent, 0, monoGameWindow.GraphicsDevice.PresentationParameters.BackBufferWidth, monoGameWindow.GraphicsDevice.PresentationParameters.BackBufferHeight), monoGameWindow.gameInput.GetWorldMouseCoords_XYPlane(this, gameInput.mousePositionPrevious, 0, monoGameWindow.GraphicsDevice.PresentationParameters.BackBufferWidth, monoGameWindow.GraphicsDevice.PresentationParameters.BackBufferHeight));
                if (Vector3.Dot(new Vector3(0.5f, -0.5f, 0), monoGameWindow.gameInput.GetWorldMouseCoords_XYPlane(this, gameInput.mousePositionCurrent, 0, monoGameWindow.GraphicsDevice.PresentationParameters.BackBufferWidth, monoGameWindow.GraphicsDevice.PresentationParameters.BackBufferHeight) - monoGameWindow.gameInput.GetWorldMouseCoords_XYPlane(this, gameInput.mousePositionPrevious, 0, monoGameWindow.GraphicsDevice.PresentationParameters.BackBufferWidth, monoGameWindow.GraphicsDevice.PresentationParameters.BackBufferHeight)) > 0)
                {
                    distance = -distance;
                }
                distance *= 6;

                this.CameraPosition = CalculateCameraCenter(monoGameWindow, this.CameraPosition.Z + distance);
            }

            #endregion





            #region Camera Pan

            //Pan The Camera(Maya Style).
            if (gameInput.isMouseOnScreen && gameInput.mouseStateCurrent.MiddleButton == ButtonState.Pressed && gameInput.keyboardStateCurrent.IsKeyDown(Keys.LeftAlt))
            {
                this.CameraPosition += monoGameWindow.gameInput.GetWorldMouseCoords_XYPlane(this, gameInput.mousePositionPrevious, 0, monoGameWindow.GraphicsDevice.PresentationParameters.BackBufferWidth, monoGameWindow.GraphicsDevice.PresentationParameters.BackBufferHeight) -
                                        monoGameWindow.gameInput.GetWorldMouseCoords_XYPlane(this, gameInput.mousePositionCurrent, 0, monoGameWindow.GraphicsDevice.PresentationParameters.BackBufferWidth, monoGameWindow.GraphicsDevice.PresentationParameters.BackBufferHeight);
            }

            #endregion

        }




        /// <summary>
        /// There are multiple XnaWindows which share the same graphics device, therefore sharing the same backbuffer. Windows of smaller sizes only
        /// use a portion of this window and therefore the focusing on an object needs to calculate and offset to properly center on the position.
        /// </summary>
        public void FocusOnPosition(Vector3 position, AsteroidGame monoGameWindow, float planeDepth)
        {
            this.CameraPosition = position + new Vector3(0, 0, this.CameraPosition.Z) - new Vector3(0, 0, planeDepth);

            Vector3 Offset = CalculateCameraCenter(monoGameWindow, planeDepth);

            this.CameraPosition = position + new Vector3(0, 0, this.CameraPosition.Z) + (new Vector3(position.X, position.Y, 0) - Offset);
        }


        /// <summary>
        /// Projects the Cameras Center onto the Specified depth
        /// planeDepth will calculate the position of the camera as if it were positioned on the xyPlane ad the specified depth.
        /// Note: camera X & Y coords are different depending on the depth. This is because of the backbuffer may be being cropped.
        /// </summary>
        public Vector3 CalculateCameraCenter(AsteroidGame monoGameWindow, float planeDepth)
        {
            Vector3 TopLeft = monoGameWindow.gameInput.GetWorldMouseCoords_XYPlane(this, new Vector3(0, 0, 0), planeDepth, monoGameWindow.GraphicsDevice.PresentationParameters.BackBufferWidth, monoGameWindow.GraphicsDevice.PresentationParameters.BackBufferHeight);
            Vector3 TopRight = monoGameWindow.gameInput.GetWorldMouseCoords_XYPlane(this, new Vector3(monoGameWindow.GraphicsDevice.Viewport.Width, 0, 0), planeDepth, monoGameWindow.GraphicsDevice.PresentationParameters.BackBufferWidth, monoGameWindow.GraphicsDevice.PresentationParameters.BackBufferHeight);
            Vector3 BottomRight = monoGameWindow.gameInput.GetWorldMouseCoords_XYPlane(this, new Vector3(monoGameWindow.GraphicsDevice.Viewport.Width, monoGameWindow.GraphicsDevice.Viewport.Height, 0), planeDepth, monoGameWindow.GraphicsDevice.PresentationParameters.BackBufferWidth, monoGameWindow.GraphicsDevice.PresentationParameters.BackBufferHeight);
            Vector3 BottomLeft = monoGameWindow.gameInput.GetWorldMouseCoords_XYPlane(this, new Vector3(0, monoGameWindow.GraphicsDevice.Viewport.Height, 0), planeDepth, monoGameWindow.GraphicsDevice.PresentationParameters.BackBufferWidth, monoGameWindow.GraphicsDevice.PresentationParameters.BackBufferHeight);

            return (TopLeft + TopRight + BottomRight + BottomLeft) * 0.25f;
        }

    }
}
